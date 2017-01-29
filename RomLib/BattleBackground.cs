using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using PKHack;

namespace PKHack
{
	public class BattleBG : RomObject 
	{
		/*
		 * Background data table: $CADCA1
		 * 
		 * 17 bytes per entry:
		 * ===================
		 * 0	Graphics/Arrangement index
		 * 1	Palette
		 * 2	Bits per pixel
		 * 3	Palette animation
		 * 4	Palette animation info
		 * 5	Palette animation info (UNKNOWN, number of palettes?)
		 * 6	UNKNOWN
		 * 7	Palette animation speed
		 * 8	Screen shift
		 * 9	Mov
		 * 10	Mov
		 * 11	Mov
		 * 12	Mov
		 * 13	Effects
		 * 14	Effects
		 * 15	Effects
		 * 16	Effects
		 * 
		 */

		private byte[] data = new byte[17];
		

		/// <summary>
		/// Index of the compresses graphics/arrangement to use for this background.
		/// </summary>
		public byte GraphicsIndex 
		{
			get { return data[0]; }
			set { data[0] = value; }
		}

		/// <summary>
		/// Index of the background palette to use.
		/// </summary>
		public byte PaletteIndex 
		{
			get { return data[1]; }
			set { data[1] = value; }
		}

		/// <summary>
		/// Must always be 2 or 4. (TODO: change this property's type to an enum)
		/// </summary>
		public byte BitsPerPixel 
		{
			get { return data[2]; }
			set { data[2] = value; }
		}

		/// <summary>
		/// Bytes 3 - 6 of BG data in big-endian order.
		/// Exact function unknown; believed to be related to palette animation.
		/// </summary>
		public int PaletteAnimationUnknown 
		{
			get
			{
				return (data[3] << 24) + (data[4] << 16) + (data[5] << 8) + data[6];
			}
			set
			{
				data[3] = (byte)(value >> 24);
				data[4] = (byte)(value >> 16);
				data[5] = (byte)(value >> 8);
				data[6] = (byte)(value);
			}
		}

		/// <summary>
		/// Controls the speed at which palette animation occurs.
		/// </summary>
		public byte PaletteAnimationSpeed 
		{
			get { return data[7]; }
			set { data[7] = value; }
		}

		/// <summary>
		/// Controls the direction in which the background translates.
		/// </summary>
		public byte ScreenShift 
		{
			get { return data[8]; }
			set { data[8] = value; }
		}

		/// <summary>
		/// Bytes 9-12 of BG data in big-endian order.
		/// Exact function unknown; related to background movement effects.
		/// </summary>
		public int Movement 
		{
			get
			{
				return (data[9] << 24) + (data[10] << 16) + (data[11] << 8) + data[12];
			}
			set
			{
				data[9] = (byte)(value >> 24);
				data[10] = (byte)(value >> 16);
				data[11] = (byte)(value >> 8);
				data[12] = (byte)(value);
			}
		}

		/// <summary>
		/// Bytes 13-16 of BG data in big-endian order.
		/// Exact function unknown; related to background animation effects.
		/// </summary>
		public int Animation 
		{
			get
			{
				return (data[13] << 24) + (data[14] << 16) + (data[15] << 8) + data[16];
			}
			set
			{
				data[13] = (byte)(value >> 24);
				data[14] = (byte)(value >> 16);
				data[15] = (byte)(value >> 8);
				data[16] = (byte)(value);
			}
		}

		
		public void Draw(Graphics g)
		{
			BackgroundGraphics gfx = (BackgroundGraphics)
				Parent.GetObject(typeof(BackgroundGraphics), GraphicsIndex);
			BackgroundPalette pal = (BackgroundPalette)
				Parent.GetObject(typeof(BackgroundPalette), PaletteIndex);

			Bitmap bmp = new Bitmap(256, 256, PixelFormat.Format24bppRgb);

			gfx.Draw(bmp, pal);
			g.DrawImage(bmp, 0, 0);
		}

		public void Draw(Bitmap bmp)
		{
			BackgroundGraphics gfx = (BackgroundGraphics)
				Parent.GetObject(typeof(BackgroundGraphics), GraphicsIndex);
			BackgroundPalette pal = (BackgroundPalette)
				Parent.GetObject(typeof(BackgroundPalette), PaletteIndex);

			gfx.Draw(bmp, pal);
		}

		public override void Read(int index)
		{
			Block main = Parent.ReadBlock(0xADEA1 + index*17);
			for (int i = 0; i < 17; i++)
				data[i] = main.ReadByte();
		}

		public override void Write(int index)
		{
			// We can just allocate a fixed block here:
			Block main = Parent.AllocateFixedBlock(17, 0xADEA1 + index * 17);
			for (int i = 0; i < 17; i++)
				main.Write(data[i]);
		}


		/// <summary>
		/// The handler for loading/saving all battle BGs
		/// </summary>
		public class Handler : RomObjectHandler
		{
			// This handler deals with background graphics and palettes as well,
			// even though those are technically separate "objects"

			public override void ReadClass(Rom rom)
			{
				// The only way to determine the bit depth of each BG palette is
				// to check the bit depth of the backgrounds that use it - so,
				// first we create an array to track palette bit depths:
				int[] palbits = new int[114];
				int[] gfxbits = new int[103];

				for (int i = 0; i < 327; i++)
				{
					BattleBG bg = new BattleBG();
					rom.Add(bg);
					bg.Read(i);

					// Now that the BG has been read, update the BPP entry for its palette
					// We can also check to make sure palettes are used consistently:
					int pal = bg.PaletteIndex;
					if (palbits[pal] != 0 && palbits[pal] != bg.BitsPerPixel)
						throw new Exception("Battle BG Palette Error - inconsistent bit depth");
					palbits[pal] = bg.BitsPerPixel;

					gfxbits[bg.GraphicsIndex] = bg.BitsPerPixel;
				}

				// Now load palettes
				for (int i = 0; i < 114; i++)
				{
					BackgroundPalette p = new BackgroundPalette();
					rom.Add(p);
					p.BitsPerPixel = palbits[i];
					p.Read(i);
				}

				// Load graphics
				for (int i = 0; i < 103; i++)
				{
					BackgroundGraphics g = new BackgroundGraphics();
					rom.Add(g);
					g.BitsPerPixel = gfxbits[i];
					g.Read(i);
				}
			}

			public override void WriteClass(Rom rom)
			{

			}
		}
	}

	public class BackgroundGraphics : RomGraphics
	{
		public override void Read(int index)
		{
			// Graphics pointer table entry
			Block gfxPtrBlock = Parent.ReadBlock(0xAD9A1 + index * 4);
			//int gfxPtr = Rom.SnesToHex(gfxPtrBlock.ReadInt());

			// Read graphics
			LoadGraphics(Parent.ReadBlock(Rom.SnesToHex(gfxPtrBlock.ReadInt())));


			// Arrangement pointer table entry
			Block arrPtrBlock = Parent.ReadBlock(0xADB3D + index * 4);
			int arrPtr = Rom.SnesToHex(arrPtrBlock.ReadInt());

			// Read and decompress arrangement
			Block arrBlock = Parent.ReadBlock(arrPtr);
			arr = arrBlock.Decomp();
		}

		public override void Write(int index)
		{
			
		}
	}

	public class BackgroundPalette : Palette
	{
		public override void Read(int index)
		{
			Block ptr = Parent.ReadBlock(0xADCD9 + index * 4);
			address = Rom.SnesToHex(ptr.ReadInt());

			Block data = Parent.ReadBlock(address);
			ReadPalette(data, bpp, 1);
		}

		public override void Write(int index)
		{
			
		}
	}
}