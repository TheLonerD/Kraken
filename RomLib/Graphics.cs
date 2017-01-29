using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using PKHack;


namespace PKHack
{
	public abstract class Palette : RomObject
	{
		protected Color[][] colors;
		protected int bpp;

		/// <summary>
		/// Gets an array of colors representing one of this palette's subpalettes.
		/// </summary>
		/// <param name="pal">The index of the subpalette to retrieve.</param>
		/// <returns>An array containing the colors of the specified subpalette.</returns>
		public Color[] this[int pal]
		{
			get
			{
				return colors[pal];
			}
		}

		/// <summary>
		/// Gets or sets the bit depth of this palette.
		/// </summary>
		public int BitsPerPixel
		{
			get { return bpp; }
			set { bpp = value; }
		}

		/// <summary>
		/// Draws a graphical display of the colors in the palette to the specified
		/// GDI graphics object.
		/// </summary>
		/// <param name="g">The Graphics object on which to draw the palette.</param>
		public void DrawPalette(Graphics g, int x, int y)
		{
			Pen p = new Pen(Color.Black, 1.0f);

			int w = 12;
			for (int i = 0; i < 1; i++)
			{
				g.DrawRectangle(p, x, y-1, colors[i].Length * w, w+1);

				for (int j = 0; j < colors[i].Length; j++)
				{
					Brush b = new SolidBrush(colors[i][j]);
					g.FillRectangle(b, x + j * w, y, w, w);
				}
				y += w;
			}
		}

		/// <summary>
		/// Internal function - reads palette data from the given block into
		/// this palette's colors array.
		/// </summary>
		/// <param name="block">Block to read palette data from.</param>
		/// <param name="bpp">Bit depth; must be either 2 or 4.</param>
		/// <param name="count">Number of subpalettes to read.</param>
		protected void ReadPalette(Block block, int bpp, int count)
		{
			if (bpp != 2 && bpp != 4)
				throw new Exception("Palette error: Incorrect color depth specified.");

			if (count < 1)
				throw new Exception("Palette error: Must specify positive number of subpalettes.");

			colors = new Color[count][];
			for (int pal = 0; pal < count; pal++)
			{
				colors[pal] = new Color[(int)Math.Pow(2, bpp)];
				for (int i = 0; i < (int)Math.Pow(2, bpp); i++)
				{
					int clr16 = block.ReadShort();
					int b = ((clr16 >> 10) & 31) * 8;
					int g = ((clr16 >> 5) & 31) * 8;
					int r = (clr16 & 31) * 8;
					colors[pal][i] = Color.FromArgb(r, g, b);
				}
			}
		}
	}

	public abstract class RomGraphics : RomObject
	{
		protected byte[] arr;
		protected byte[] gfx;
		protected int bpp;

		protected int width = 32;
		protected int height = 32;

		// A cache of tiles from the raw graphics data
		protected List<byte[][]> tiles;

		public int BitsPerPixel
		{
			get { return bpp; }
			set { bpp = value; }
		}

		public int Width
		{
			get { return width; }
		}

		public int Height
		{
			get { return height; }
		}


		/// <summary>
		/// Internal function - builds the tile array from the gfx buffer.
		/// </summary>
		protected void BuildTiles()
		{
			int n = gfx.Length / (8 * bpp);

			tiles = new List<byte[][]>();
			
			for (int i = 0; i < n; i++)
			{
				tiles.Add(new byte[8][]);

				int o = i * 8 * bpp;

				for (int x = 0; x < 8; x++)
				{
					tiles[i][x] = new byte[8];
					for (int y = 0; y < 8; y++)
					{
						int c = 0;
						for (int bp = 0; bp < bpp; bp++)
							c += (((gfx[o + y * 2 + ((bp / 2) * 16 + (bp & 1))]) & (1 << 7 - x)) >> 7 - x) << bp;
						tiles[i][x][y] = (byte)c;
					}
				}
			}
		}


		protected unsafe void DrawTile(BitmapData bmp, int x, int y, byte[][] tile, Color[] pal, bool vflip, bool hflip)
		{
			int stride = bmp.Stride;
			byte* scan = (byte*)bmp.Scan0;

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int px = x + (hflip ? 7 - i : i);
					int py = y + (vflip ? 7 - j : j);
					scan[px * 3 + py * stride + 2] = pal[tile[i][j]].R;
					scan[px * 3 + py * stride + 1] = pal[tile[i][j]].G;
					scan[px * 3 + py * stride + 0] = pal[tile[i][j]].B;
				}
			}
		}


		public void Draw(Bitmap bmp, Palette pal)
		{
			// Do not attempt to draw with an insufficient palette
			if (pal.BitsPerPixel < this.BitsPerPixel)
				return;

			BitmapData dat = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					int n = j*32+i;
					int block = arr[n*2] + (arr[n*2 + 1] << 8);
					int tile = block & 0x3FF;
					bool vflip = (block & 0x8000) != 0;
					bool hflip = (block & 0x4000) != 0;
					int subpal = (block >> 10) & 7;

					DrawTile(dat, i*8, j*8, tiles[tile], pal[subpal], vflip, hflip);
				}
			}

			bmp.UnlockBits(dat);
		}

		private unsafe void DrawTile(byte* bmp, int stride, int n, Palette pal, int dx, int dy, bool blend)
		{
			int block = (arr[n * 2]) + (arr[n * 2 + 1] << 8);

			int tile = block & 0x3FF;
			bool vflip = (block & 0x8000) != 0;
			bool hflip = (block & 0x4000) != 0;
			int palette = (block >> 10) & 7;

			int o = tile * bpp * 8; // bpp*8 = 16 bytes for 2bpp, 32 for 4bpp

			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int c = 0;
					for (int bp = 0; bp < bpp; bp++)
						c += (((gfx[o + j * 2 + ((bp / 2) * 16 + (bp & 1))]) & (1 << 7 - i)) >> 7 - i) << bp;

					int x = dx + (hflip ? 7 - i : i);
					int y = dy + (vflip ? 7 - j : j);
					int p = x * 3 + y * stride;
					if (blend)
					{
						bmp[p + 2] = (byte)(bmp[p + 2] / 2 + pal[0][c].R / 2);
						bmp[p + 1] = (byte)(bmp[p + 1] / 2 + pal[0][c].G / 2);
						bmp[p] = (byte)(bmp[p] / 2 + pal[0][c].B / 2);
					}
					else
					{
						bmp[p + 2] = pal[0][c].R;
						bmp[p + 1] = pal[0][c].G;
						bmp[p] = pal[0][c].B;
					}
				}
			}
		}


		/// <summary>
		/// Internal function - reads graphics from the specified block
		/// and builds tileset.
		/// </summary>
		/// <param name="block">The block to read graphics data from</param>
		protected void LoadGraphics(Block block)
		{
			gfx = block.Decomp();
			BuildTiles();
		}


		/// <summary>
		/// Internal function - reads arrangement from specified block
		/// </summary>
		/// <param name="block">The block to read arrangement data from</param>
		protected void LoadArrangement(Block block)
		{
			arr = block.Decomp();
		}
	}
}