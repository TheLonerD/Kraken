using PKHack;
using Screensavers;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Kraken
{
    /// <summary>
    /// The actual screensaver class
    /// </summary>
    public class Kraken : Screensaver
    {
        private Rom data;
        private BackgroundLayer layer0;
        private BackgroundLayer layer1;
        private int letterbox;
        private int frameskip = 1;
        private bool showfps;
        private bool randomize;

        private Bitmap bmp;
        private int tick = 0;

        // DirectX stuff
        private Device[] display;
        private Direct3D direct3d;
        private SharpDX.Direct3D9.Font font = null;
        TransformedColoredTextured[] vertexes = new TransformedColoredTextured[4];

        // Entry point
        [STAThread]
        static void Main()
        {
            // ROM startup stuff
            try
            {
                // Manually register types; no RomLib plugins
                Rom.RegisterType("BattleBGEffect", typeof(BattleBGEffect), typeof(BattleBGEffect.Handler));
                Rom.RegisterType("BattleBG", typeof(BattleBG), typeof(BattleBG.Handler));
                Rom.RegisterType("BackgroundGraphics", typeof(BackgroundGraphics), null);
                Rom.RegisterType("BackgroundPalette", typeof(BackgroundPalette), null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error initializing ROM library.\n\n" + e.Message);
                return;
            }

            Screensaver ss = new Kraken();
            ss.Run();
        }


        /// <summary>
        /// Constructs a new instance of the screensaver
        /// </summary>
        public Kraken()
        {
            try
            {
                data = new Rom();
                System.IO.Stream ss = Assembly.GetExecutingAssembly().GetManifestResourceStream("Kraken.bgs.dat");
                data.Open(ss);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error opening background animation data.\n\n" + e.Message);
            }

            try
            {
                LoadConfig();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading configuration.\n\n" + e.Message);
            }

            Initialize += Kraken_Initialize;
            Update += Kraken_Update;
            Exit += Kraken_Exit;
        }

        /// <summary>
        /// Initializes the screensaver
        /// </summary>
        void Kraken_Initialize(object sender, EventArgs e)
        {
            InitDirect3D();
            InitVertexes();
            bmp = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Cleans up on screensaver exit
        /// </summary>
        void Kraken_Exit(object sender, EventArgs e)
        {
            foreach (Device d in display)
                d.Dispose();
        }




        #region Public properties

        /// <summary>
        /// Gets the BackgroundLayer object for the first layer
        /// </summary>
        public BackgroundLayer Layer0
        {
            get { return layer0; }
        }

        /// <summary>
        /// Gets the BackgroundLayer object for the second layer
        /// </summary>
        public BackgroundLayer Layer1
        {
            get { return layer1; }
        }

        /// <summary>
        /// Gets or sets the size of the black borders at the top and bottom of the animation
        /// </summary>
        public int Letterbox
        {
            get { return letterbox; }
            set { letterbox = value; }
        }

        /// <summary>
        /// Gets or sets the animation index loaded for the first layer
        /// </summary>
        public int Layer0Index
        {
            get { return layer0.Entry; }
            set
            {
                layer0 = new BackgroundLayer(data, value);
            }
        }

        /// <summary>
        /// Gets or sets the animation index loaded for the second layer
        /// </summary>
        public int Layer1Index
        {
            get { return layer1.Entry; }
            set
            {
                layer1 = new BackgroundLayer(data, value);
            }
        }


        /// <summary>
        /// Whether an FPS counter should be displayed onscreen
        /// </summary>
        public bool ShowFPS
        {
            get { return showfps; }
            set { showfps = value; }
        }

        /// <summary>
        /// How many distortion frames to skip each tick
        /// </summary>
        public int Frameskip
        {
            get { return frameskip; }
            set { frameskip = value; }
        }

        /// <summary>
        /// Randomize battle backgrounds every launch
        /// </summary>
        public bool Randomize
        {
            get { return randomize; }
            set { randomize = value; }
        }

        #endregion



        /// <summary>
        /// Initializes the Direct3D components used
        /// </summary>
        private void InitDirect3D()
        {
            direct3d = new Direct3D();
            display = new Device[Windows.Count];

            for (int i = 0; i < Windows.Count; i++)
            {
                Window window = Windows[i];
                PresentParameters pp = GetPP(window.DeviceIndex);
                window.DoubleBuffer = false;

                /*
                 * First, try creating a hardware device
                 */
                try
                {
                    display[i] = new Device(direct3d,
                       window.DeviceIndex, DeviceType.Hardware,
                       window.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                    continue;
                }
                catch (Exception) { }

                /*
                 * Try creating a software device if the hardware device fails
                 */
                try
                {
                    display[i] = new Device(direct3d,
                       window.DeviceIndex, DeviceType.Software,
                       window.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                }
                catch (Exception)
                {
                    MessageBox.Show("Couldn't create D3D device!");
                    Application.Exit();
                }
            }
            font = new SharpDX.Direct3D9.Font(
               display[0], new FontDescription() { FaceName = SystemFonts.DefaultFont.Name });
        }


        /// <summary>
        /// Creates a PresentParameters for the given device index
        /// </summary>
        private PresentParameters GetPP(int deviceIndex)
        {
            PresentParameters pp = new PresentParameters();

            if (this.Mode != ScreensaverMode.Normal)
                pp.Windowed = true;
            else
            {
                pp.Windowed = false;
                pp.BackBufferCount = 1;
                pp.BackBufferWidth =
                    direct3d.Adapters[deviceIndex].CurrentDisplayMode.Width;
                pp.BackBufferHeight =
                   direct3d.Adapters[deviceIndex].CurrentDisplayMode.Height;
                pp.BackBufferFormat =
                   direct3d.Adapters[deviceIndex].CurrentDisplayMode.Format;
            }

            pp.SwapEffect = SwapEffect.Flip;
            return pp;
        }


        /// <summary>
        /// Creates a texture from a bitmap. Much, much faster than Texture.FromBitmap.
        /// </summary>
        /// <param name="bmp"></param>
        Texture TextureFromBitmap(Bitmap bmp)
        {
            Texture texture = new Texture(display[0], bmp.Width, bmp.Height, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            DataRectangle a = texture.LockRectangle(0, LockFlags.None);
            BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                uint* to = (uint*)a.DataPointer;
                uint* from = (uint*)bd.Scan0.ToPointer();
                for (int i = 0; i < bd.Height * bd.Width; ++i)
                {
                    to[i] = from[i];
                }
            }
            texture.UnlockRectangle(0);
            bmp.UnlockBits(bd);
            return texture;
        }


        /// <summary>
        /// Sets up a quad of vertices
        /// </summary>
        private void InitVertexes()
        {
            vertexes[0].Position = new Vector3(0, 0, 0);
            vertexes[0].Rhw = 1.0f;
            vertexes[0].Tu = 0;
            vertexes[0].Tv = 0;
            vertexes[0].Color = -1;

            vertexes[1].Position = new Vector3(512, 0, 0);
            vertexes[1].Rhw = 1.0f;
            vertexes[1].Tu = 1;
            vertexes[1].Tv = 0;
            vertexes[1].Color = -1;

            vertexes[2].Position = new Vector3(512, 512, 0);
            vertexes[2].Rhw = 1.0f;
            vertexes[2].Tu = 1;
            vertexes[2].Tv = 0.875f;
            vertexes[2].Color = -1;

            vertexes[3].Position = new Vector3(0, 512, 0);
            vertexes[3].Rhw = 1.0f;
            vertexes[3].Tu = 0;
            vertexes[3].Tv = 0.875f;
            vertexes[3].Color = -1;
        }


        /// <summary>
        /// Stretches the textured quad to a given size
        /// </summary>
        private void StretchVertexes(int width, int height)
        {
            int h = height;
            int w = width;
            int x = 0;
            int y = 0;
            vertexes[0].Position = new Vector3(x, y, 0);
            vertexes[1].Position = new Vector3(x + w, y, 0);
            vertexes[2].Position = new Vector3(x + w, y + h, 0);
            vertexes[3].Position = new Vector3(x, y + h, 0);
        }


        /// <summary>
        /// Called for each frame; updates display
        /// </summary>
        void Kraken_Update(object sender, EventArgs e)
        {
            // Compute the frame to display
            float alpha = 0.5f;
            if (layer1.Entry == 0) alpha = 1.0f;
            layer0.OverlayFrame(bmp, letterbox, tick, alpha, true);
            layer1.OverlayFrame(bmp, letterbox, tick, 0.5f, false);
            //distort.ComputeFrame(bmp, tick, 0.1f);
            tick += Frameskip;

            // Create a texture from the frame
            Texture texture = TextureFromBitmap(bmp);

            // Render on each display device
            for (int i = 0; i < display.Length; i++)
            {
                Device d = display[i];
                Window w = Windows[i];
                try
                {
                    d.TestCooperativeLevel();

                    // Clear the screen and draw crap
                    d.Clear(ClearFlags.Target, ColorBGRA.FromRgba(System.Drawing.Color.Black.ToArgb()), 0, 0);
                    d.BeginScene();

                    StretchVertexes(w.Size.Width, w.Size.Height);

                    d.VertexFormat = TransformedColoredTextured.Format;
                    d.SetTexture(0, texture);
                    d.DrawUserPrimitives(PrimitiveType.TriangleFan, 2, vertexes);

                    if (showfps)
                        font.DrawText(null, "FPS: " + AchievedFramerate, 0, 0,
                                  ColorBGRA.FromRgba(System.Drawing.Color.White.ToArgb()));

                    d.EndScene();
                    d.Present();
                }
                catch (Exception)
                {

                }
            }
            texture.Dispose();

            // Eh, sort of a hack. Prevent memory from bloating too much.
            if ((tick % 60) == 0)
            {
                GC.Collect();
            }
        }



        /// <summary>
        /// Shows the settings dialog box
        /// </summary>
        protected override void ShowSettingsDialog()
        {
            ConfigForm f = new ConfigForm(this);
            f.ShowDialog();
        }


        /// <summary>
        /// Loads screensaver parameters from the configuration file
        /// </summary>
        public void LoadConfig()
        {
            // Load the default configuration here
            ConfigFile cfg = new ConfigFile(Application.StartupPath + "\\Kraken.ini");
            int rate = cfg.GetInt("framerate");
            if (rate < 1 || rate > 100) rate = 30;
            Framerate = rate;

            frameskip = cfg.GetInt("frameskip");
            if (frameskip < 1 || frameskip > 10) frameskip = 1;

            int n0 = cfg.GetInt("layer1");
            if (n0 < 0 || n0 > 326) n0 = 270;

            int n1 = cfg.GetInt("layer2");
            if (n1 < 0 || n1 > 326) n1 = 269;

            letterbox = cfg.GetInt("letterbox");
            if (letterbox < 0 || letterbox > 100) letterbox = 16;

            int bshow = cfg.GetInt("showfps");
            showfps = bshow != 0;
            if (bshow == -1) showfps = false;

            int brandomize = cfg.GetInt("randomize");
            randomize = brandomize != 0;
            if (brandomize == -1) randomize = false;

            if (randomize)
            {
                n0 = NextInt(1, 327);
                n1 = NextInt(1, 327);
            }

            try
            {
                layer0 = new BackgroundLayer(data, n0);
                layer1 = new BackgroundLayer(data, n1);
            }
            catch (Exception e)
            {
                MessageBox.Show("Config error: couldn't load background layers.\n\n" + e.Message);
            }
        }

        private static int NextInt(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            return new Random(result).Next(min, max);
        }
    }
}
