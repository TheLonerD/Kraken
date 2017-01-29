using SharpDX;
using SharpDX.Direct3D9;
using System.Runtime.InteropServices;

namespace Kraken
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformedColoredTextured
    {
        #region Constants
        /// <summary>
        /// The fixed-function format of this vertex structure.
        /// </summary>
        public const VertexFormat Format = VertexFormat.PositionRhw | VertexFormat.Diffuse | VertexFormat.Texture1;

        /// <summary>
        /// The length of this vertex structure in bytes.
        /// </summary>
        public const int StrideSize = 7 * 4;
        #endregion

        #region Variables
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        /// <summary>The position of the vertex in screen-space</summary>
        public Vector3 Position;

        /// <summary>The reciprocal of homogeneous W (the depth-value)</summary>
        public float Rhw;

        /// <summary>A color by which the texture will be multiplied</summary>
        public int Color;

        /// <summary>The U-component of the texture coordinates</summary>
        public float Tu;

        /// <summary>The V-component of the texture coordinates</summary>
        public float Tv;

        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore MemberCanBePrivate.Global
        #endregion

        #region Properties
        /// <summary>The X-component of the position of the vertex in screen-space</summary>
        public float X { get { return Position.X; } set { Position.X = value; } }

        /// <summary>The Y-component of the position of the vertex in screen-space</summary>
        public float Y { get { return Position.Y; } set { Position.Y = value; } }

        /// <summary>The Z-component of the position of the vertex in screen-space</summary>
        public float Z { get { return Position.Z; } set { Position.Z = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new transformed, colored and textured vertex
        /// </summary>
        /// <param name="position">The position of the vertex in screen-space</param>
        /// <param name="rhw">The reciprocal of homogeneous W (the depth-value)</param>
        /// <param name="color">A color by which the texture will be multiplied</param>
        /// <param name="tu">The U-component of the texture coordinates</param>
        /// <param name="tv">The V-component of the texture coordinates</param>
        public TransformedColoredTextured(Vector3 position, float rhw, int color, float tu, float tv)
        {
            Position = position;
            Rhw = rhw;
            Color = color;
            Tu = tu;
            Tv = tv;
        }

        /// <summary>
        /// Creates a new transformed, colored and textured vertex
        /// </summary>
        /// <param name="xvalue">The X-component of the position of the vertex in screen-space</param>
        /// <param name="yvalue">The Y-component of the position of the vertex in screen-space</param>
        /// <param name="zvalue">The Z-component of the position of the vertex in screen-space</param>
        /// <param name="rhw">The reciprocal of homogeneous W (the depth-value)</param>
        /// <param name="color">A color by which the texture will be multiplied</param>
        /// <param name="tu">The U-component of the texture coordinates</param>
        /// <param name="tv">The V-component of the texture coordinates</param>
        public TransformedColoredTextured(float xvalue, float yvalue, float zvalue, float rhw, int color, float tu, float tv)
            : this(new Vector3(xvalue, yvalue, zvalue), rhw, color, tu, tv)
        { }
        #endregion

        #region ToString
        public override string ToString()
        {
            return "TransformedColoredTextured(position=" + Position + ", rhw=" + Rhw + ", " +
                   "color=" + Color + ", " + "tu=" + Tu + ", " + "tv=" + Tv + ")";
        }
        #endregion
    }
}
