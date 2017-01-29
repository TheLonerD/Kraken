using PKHack;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kraken
{
    /// <summary>
    /// The BackgroundLayer class collects together the various elements of a battle background:
    ///  - BG Graphics
    ///  - BG Palette
    ///  - A Distorter object to compute transformations
    /// </summary>
    public class BackgroundLayer
    {
        private int entry;
        private BackgroundGraphics gfx;
        private BackgroundPalette pal;
        private Distorter distort;
        private Bitmap bmp;

        /// <summary>
        /// The index of the layer entry that was loaded
        /// </summary>
        public int Entry
        {
            get { return entry; }
        }

        /// <summary>
        /// Constructs a BackgroundLayer object by loading the specified entry from the specified ROM object
        /// </summary>
        public BackgroundLayer(Rom src, int entry)
        {
            distort = new Distorter();
            bmp = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
            LoadEntry(src, entry);
        }

        /// <summary>
        /// Renders a frame of the background animation into the specified Bitmap
        /// </summary>
        /// <param name="dst">Bitmap object into which to render</param>
        /// <param name="letterbox">Size in pixels of black borders at top and bottom of image</param>
        /// <param name="ticks">Time value of the frame to compute</param>
        /// <param name="alpha">Blending opacity</param>
        /// <param name="erase">Whether or not to clear the destination bitmap before rendering</param>
        public void OverlayFrame(Bitmap dst, int letterbox, int ticks, float alpha, bool erase)
        {
            distort.OverlayFrame(dst, letterbox, ticks, alpha, erase);
        }


        private void LoadGraphics(Rom src, int n)
        {
            gfx = (BackgroundGraphics)src.GetObject("BackgroundGraphics", n);
        }

        private void LoadPalette(Rom src, int n)
        {
            pal = (BackgroundPalette)src.GetObject("BackgroundPalette", n);
        }

        private void LoadEffect(Rom src, int n)
        {
            BattleBGEffect effect = (BattleBGEffect)src.GetObject("BattleBGEffect", n);
            distort.Effect.Amplitude = effect.Amplitude;
            distort.Effect.AmplitudeAcceleration = (short)effect.AmplitudeAcceleration;
            distort.Effect.Compression = effect.Compression;
            distort.Effect.CompressionAcceleration = effect.CompressionAcceleration;
            distort.Effect.Frequency = effect.Frequency;
            distort.Effect.FrequencyAcceleration = (short)effect.FrequencyAcceleration;
            distort.Effect.Speed = effect.Speed;
            if (effect.Type == 1)
                distort.Effect.Effect = DistortionEffect.Type.Horizontal;
            else if (effect.Type == 3)
                distort.Effect.Effect = DistortionEffect.Type.Vertical;
            else
                distort.Effect.Effect = DistortionEffect.Type.HorizontalInterlaced;
        }

        private void LoadEntry(Rom src, int n)
        {
            entry = n;
            BattleBG bg = (BattleBG)src.GetObject("BattleBG", n);

            // Set graphics / palette
            LoadGraphics(src, bg.GraphicsIndex);
            LoadPalette(src, bg.PaletteIndex);

            int e = bg.Animation;

            byte e1 = (byte)(e >> 24);
            byte e2 = (byte)(e >> 16);
            byte e3 = (byte)(e >> 8);
            byte e4 = (byte)(e);

            // This is probably crap
            if (e2 != 0)
                LoadEffect(src, e2);
            else
                LoadEffect(src, e1);

            InitializeBitmap();
        }

        private void InitializeBitmap()
        {
            bmp = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx.Draw(bmp, pal);
            distort.Original = bmp;
        }
    }
}
