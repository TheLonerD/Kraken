using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kraken
{
    public partial class ConfigForm : Form
    {
        private Kraken kraken;

        private Bitmap preview;

        private Timer timer;
        private int tick = 0;



        public ConfigForm(Kraken kraken)
        {
            this.kraken = kraken;
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 17;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            preview = new Bitmap(256, 224, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            pictureBox1.Image = preview;

            layer1.Value = kraken.Layer0.Entry;
            layer2.Value = kraken.Layer1.Entry;
            framerate.Value = kraken.Framerate;
            frameskip.Value = kraken.Frameskip;
            showfps.Checked = kraken.ShowFPS;
            randomize.Checked = kraken.Randomize;

            switch (kraken.Letterbox)
            {
                case 0:
                    letterbox.SelectedIndex = 0; break;
                case 16:
                    letterbox.SelectedIndex = 1; break;
                case 48:
                    letterbox.SelectedIndex = 2; break;
                case 64:
                    letterbox.SelectedIndex = 3; break;
            }
        }


        void timer_Tick(object sender, EventArgs e)
        {
            float alpha = 0.5f;
            if (kraken.Layer1Index == 0) alpha = 1.0f;
            kraken.Layer0.OverlayFrame(preview, kraken.Letterbox, tick, alpha, true);
            kraken.Layer1.OverlayFrame(preview, kraken.Letterbox, tick, 0.5f, false);
            tick += kraken.Frameskip;
            pictureBox1.Refresh();
        }


        private void framerate_ValueChanged(object sender, EventArgs e)
        {
            kraken.Framerate = (int)framerate.Value;
            timer.Interval = (int)(1000.0 / (int)framerate.Value);
        }
        private void framerate_KeyUp(object sender, KeyEventArgs e)
        {
            framerate_ValueChanged(sender, e);
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            ConfigFile cfg = new ConfigFile(Application.StartupPath + "\\Kraken.ini");
            cfg.SetInt("framerate", kraken.Framerate);
            cfg.SetInt("frameskip", kraken.Frameskip);
            cfg.SetInt("layer1", kraken.Layer0Index);
            cfg.SetInt("layer2", kraken.Layer1Index);
            cfg.SetInt("letterbox", kraken.Letterbox);
            cfg.SetInt("showfps", showfps.Checked ? 1 : 0);
            cfg.SetInt("randomize", randomize.Checked ? 1 : 0);
            cfg.Save();
            cfg.Close();
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frameskip_ValueChanged(object sender, EventArgs e)
        {
            kraken.Frameskip = (int)frameskip.Value;
        }

        private void frameskip_KeyUp(object sender, KeyEventArgs e)
        {
            frameskip_ValueChanged(sender, e);
        }

        private void layer1_ValueChanged(object sender, EventArgs e)
        {
            kraken.Layer0Index = (int)layer1.Value;
        }

        private void layer2_ValueChanged(object sender, EventArgs e)
        {
            kraken.Layer1Index = (int)layer2.Value;
        }

        private void letterbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (letterbox.SelectedIndex)
            {
                case 0: // fullscreen
                    kraken.Letterbox = 0; break;
                case 1: // 4:3
                    kraken.Letterbox = 16; break;
                case 2: // 2:1
                    kraken.Letterbox = 48; break;
                case 3: // 8:3
                    kraken.Letterbox = 64; break;
            }
        }

        private void blank1_Click(object sender, EventArgs e)
        {
            layer1.Value = 0;
        }

        private void blank2_Click(object sender, EventArgs e)
        {
            layer2.Value = 0;
        }

        private void randomize_CheckedChanged(object sender, EventArgs e)
        {
            kraken.Randomize = randomize.Checked;

            layer1.Enabled = !randomize.Checked;
            layer2.Enabled = !randomize.Checked;
            blank1.Enabled = !randomize.Checked;
            blank2.Enabled = !randomize.Checked;
        }
    }
}
