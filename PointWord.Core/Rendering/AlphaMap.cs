using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PointWord.Core.Rendering;

/// <summary>
/// Provides fast alpha sampling for a bitmap by extracting alpha values into a contiguous buffer.
/// </summary>
public sealed class AlphaMap
{
    private readonly byte[] alphaBuffer;

    private AlphaMap(int width, int height, byte[] alphaBuffer)
    {
        Width = width;
        Height = height;
        this.alphaBuffer = alphaBuffer;
    }

    /// <summary>
    /// Gets the bitmap width represented by the alpha map.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the bitmap height represented by the alpha map.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Builds an <see cref="AlphaMap"/> from a bitmap using LockBits for performance.
    /// </summary>
    /// <param name="maskBitmap">Source bitmap containing alpha information.</param>
    /// <returns>A new alpha map extracted from the source bitmap.</returns>
    public static AlphaMap FromBitmap(Bitmap maskBitmap)
    {
        ArgumentNullException.ThrowIfNull(maskBitmap);

        using Bitmap argbBitmap = EnsureArgb32(maskBitmap);
        Rectangle rect = new(0, 0, argbBitmap.Width, argbBitmap.Height);
        BitmapData bitmapData = argbBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int stride = bitmapData.Stride;
            int strideAbs = Math.Abs(stride);
            int bytes = strideAbs * argbBitmap.Height;
            byte[] raw = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, raw, 0, bytes);

            byte[] alpha = new byte[argbBitmap.Width * argbBitmap.Height];

            for (int y = 0; y < argbBitmap.Height; y++)
            {
                int rowStart = stride >= 0
                    ? y * stride
                    : (argbBitmap.Height - 1 - y) * strideAbs;
                int alphaRowStart = y * argbBitmap.Width;

                for (int x = 0; x < argbBitmap.Width; x++)
                {
                    alpha[alphaRowStart + x] = raw[rowStart + (x * 4) + 3];
                }
            }

            return new AlphaMap(argbBitmap.Width, argbBitmap.Height, alpha);
        }
        finally
        {
            argbBitmap.UnlockBits(bitmapData);
        }
    }

    /// <summary>
    /// Samples one alpha value from the map.
    /// </summary>
    /// <param name="x">X coordinate in pixel space.</param>
    /// <param name="y">Y coordinate in pixel space.</param>
    /// <returns>The alpha value, or 0 if outside map bounds.</returns>
    public byte Sample(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return 0;
        }

        return alphaBuffer[(y * Width) + x];
    }

    private static Bitmap EnsureArgb32(Bitmap source)
    {
        if (source.PixelFormat == PixelFormat.Format32bppArgb)
        {
            return (Bitmap)source.Clone();
        }

        Bitmap converted = new(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(converted);
        g.DrawImage(source, 0, 0, source.Width, source.Height);
        return converted;
    }
}

