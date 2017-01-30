using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace PKHack
{
    public class DistortionEffect
    {
        public enum Type
        {
            Invalid,
            Horizontal,
            HorizontalInterlaced,
            Vertical
        }

        private Type type;

        private ushort ampl;
        private ushort s_freq;
        private short ampl_accel;
        private short s_freq_accel;

        private byte start;
        private byte speed;

        private short compr;
        private short compr_accel;

        private const double C1 = 1 / 512.0;
        private const double C2 = 8.0 * Math.PI / (1024 * 256);
        private const double C3 = Math.PI / 60.0;

        /// <summary>
        /// Evaluates the distortion effect at the given destination line and
        /// time value and returns the computed offset value.
        ///
        /// If the distortion mode is horizontal, this offset should be interpreted
        /// as the number of pixels to offset the given line's starting x position.
        ///
        /// If the distortion mode is vertical, this offset should be interpreted as
        /// the y-coordinate of the line from the source bitmap to draw at the given
        /// y-coordinate in the destination bitmap.
        /// </summary>
        /// <param name="y">The y-coordinate of the destination line to evaluate for</param>
        /// <param name="t">The number of ticks since beginning animation</param>
        /// <returns>The distortion offset for the given (y,t) coordinates</returns>
        public int GetAppliedOffset(int y, int t)
        {
            // Compute "current" values of amplitude, frequency, and compression
            ushort amplitude = (ushort)(ampl + ampl_accel * t * 2);
            ushort frequency = (ushort)(s_freq + s_freq_accel * t * 2);
            short compression = (short)(compr + compr_accel * t * 2);

            // Compute the value of the sinusoidal line offset function
            int S = (int)(C1 * amplitude * Math.Sin(C2 * frequency * y + C3 * speed * t));

            if (type == Type.Horizontal)
            {
                return S;
            }
            if (type == Type.HorizontalInterlaced)
            {
                return (y % 2) == 0 ? -S : S;
            }
            if (type == Type.Vertical)
            {
                int L = (int)(y * (1 + compression / 256.0) + S) % 256;
                if (L < 0) L = 256 + L;
                if (L > 255) L = 256 - L;

                return L;
            }

            return 0;
        }

        /// <summary>
        /// Gets or sets the type of distortion effect to use.
        /// </summary>
        public Type Effect
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the amplitude of the distortion effect
        /// </summary>
        public ushort Amplitude
        {
            get { return ampl; }
            set { ampl = value; }
        }

        /// <summary>
        /// Gets or sets the spatial frequency of the distortion effect
        /// </summary>
        public ushort Frequency
        {
            get { return s_freq; }
            set { s_freq = value; }
        }

        /// <summary>
        /// The amount to add to the amplitude value every iteration.
        /// </summary>
        public short AmplitudeAcceleration
        {
            get { return ampl_accel; }
            set { ampl_accel = value; }
        }

        /// <summary>
        /// The amount to add to the frequency value each iteration.
        /// </summary>
        public short FrequencyAcceleration
        {
            get { return s_freq_accel; }
            set { s_freq_accel = value; }
        }

        /// <summary>
        /// Compression factor
        /// </summary>
        public short Compression
        {
            get { return compr; }
            set { compr = value; }
        }

        /// <summary>
        /// Change in the compression value every iteration
        /// </summary>
        public short CompressionAcceleration
        {
            get { return compr_accel; }
            set { compr_accel = value; }
        }

        /// <summary>
        /// Offset for starting time.
        /// </summary>
        public byte StartTime
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// Gets or sets the "speed" of the distortion.
        /// 0 = no animation, 127 = very fast, 255 = very slow for some reason
        /// </summary>
        public byte Speed
        {
            get { return speed; }
            set { speed = value; }
        }
    }

    public class Distorter
    {
        private Bitmap src;

        // There is some redundancy here: 'effect' is currently what is used
        // in computing frames, although really there should be a list of
        // four different effects ('dist') which are used in sequence.
        //
        // 'dist' is currently unused, but ComputeFrame should be changed to
        // make use of it as soon as the precise nature of effect sequencing
        // can be determined.
        //
        // The goal is to make Distorter a general-purpose BG effect class that
        // can be used to show either a single distortion effect, or to show the
        // entire sequence of effects associated with a background entry (including
        // scrolling and palette animation, which still need to be implemented).
        //
        // Also note that "current_dist" should not be used. Distorter should be
        // a "temporally stateless" class, meaning that all temporal effects should
        // be computed at once, per request, rather than maintaining an internal
        // tick count. (The idea being that it should be fast to compute any individual
        // frame. Since it is certainly possible to do this, there is no sense
        // requiring that all previous frames be computed before any given desired
        // frame.)
        private DistortionEffect effect = new DistortionEffect();

        private DistortionEffect[] dist = new DistortionEffect[4];
        private int current_dist = 1;


        public DistortionEffect[] Distortions
        {
            get { return dist; }
        }

        public DistortionEffect CurrentDistortion
        {
            get { return dist[current_dist]; }
        }

        public DistortionEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }


        public Bitmap Original
        {
            get { return src; }
            set
            {
                src = new Bitmap(value);
            }
        }


        public void OverlayFrame(Bitmap dst, int letterbox, int ticks, float alpha, bool erase)
        {
            ComputeFrame(dst, letterbox, ticks, alpha, erase);
        }

        public void RenderFrame(Bitmap dst, int ticks)
        {
            ComputeFrame(dst, 0, ticks, 1.0f, true);
        }

        // Computes a distortion of the source and overlays it on a destination bitmap
        // with specified alpha
        private unsafe void ComputeFrame(Bitmap dst, int letterbox, int ticks, float alpha, bool erase)
        {

            // Lock source and destination bitmap bits
            BitmapData dstData = dst.LockBits(new Rectangle(0, 0, dst.Width, dst.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            BitmapData srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int dstStride = dstData.Stride;
            int srcStride = srcData.Stride;

            byte* bdst = (byte*)dstData.Scan0;
            byte* bsrc = (byte*)srcData.Scan0;


            // Given the list of 4 distortions and the tick count, decide which
            // effect to use:

            // Basically, we have 4 effects, each possibly with a duration.
            //
            // Evaluation order is: 1, 2, 3, 0
            //
            // If the first effect is null, control transitions to the second effect.
            // If the first and second effects are null, no effect occurs.
            // If any other effect is null, the sequence is truncated.
            // If a non-null effect has a zero duration, it will not be switched
            // away from.
            //
            // Essentially, this configuration sets up a precise and repeating
            // sequence of between 0 and 4 different distortion effects. Once we
            // compute the sequence, computing the particular frame of which distortion
            // to use becomes easy; simply mod the tick count by the total duration
            // of the effects that are used in the sequence, then check the remainder
            // against the cumulative durations of each effect.
            //
            // I guess the trick is to be sure that my description above is correct.
            //
            // Heh.

            // First, count the total duration of the effects:
            /*int total = 0;
            for (int i = 0; i < 4; i++)
            {

            }*/

            for (int y = 0; y < 224; y++)
            {
                int S = effect.GetAppliedOffset(y, ticks);
                int L = y;

                if (Effect.Effect == DistortionEffect.Type.Vertical)
                    L = S;

                for (int x = 0; x < 256; x++)
                {
                    if (y < letterbox || y > 224 - letterbox)
                    {
                        bdst[x * 3 + y * dstStride + 2] = 0;
                        bdst[x * 3 + y * dstStride + 1] = 0;
                        bdst[x * 3 + y * dstStride + 0] = 0;
                        continue;
                    }

                    int dx = x;

                    if (Effect.Effect == DistortionEffect.Type.Horizontal
                        || Effect.Effect == DistortionEffect.Type.HorizontalInterlaced)
                    {
                        dx = (x + S) % 256;
                        if (dx < 0) dx = 256 + dx;
                        if (dx > 255) dx = 256 - dx;
                    }


                    /*
                     * Either copy or add to the destination bitmap
                     */
                    if (erase)
                    {
                        bdst[x * 3 + y * dstStride + 2] = (byte)(alpha * bsrc[dx * 3 + L * srcStride + 2]);
                        bdst[x * 3 + y * dstStride + 1] = (byte)(alpha * bsrc[dx * 3 + L * srcStride + 1]);
                        bdst[x * 3 + y * dstStride + 0] = (byte)(alpha * bsrc[dx * 3 + L * srcStride + 0]);
                    }
                    else
                    {
                        bdst[x * 3 + y * dstStride + 2] += (byte)(alpha * bsrc[dx * 3 + L * srcStride + 2]);
                        bdst[x * 3 + y * dstStride + 1] += (byte)(alpha * bsrc[dx * 3 + L * srcStride + 1]);
                        bdst[x * 3 + y * dstStride + 0] += (byte)(alpha * bsrc[dx * 3 + L * srcStride + 0]);
                    }
                }
            }


            dst.UnlockBits(dstData);
            src.UnlockBits(srcData);
        }
    }
}
