using System.Drawing;

namespace TextHoleArt.Core.Models;

/// <summary>
/// Holds the generated output image and optional debug mask image.
/// </summary>
public sealed record RenderResult
{
    /// <summary>
    /// Gets the final rendered output bitmap.
    /// </summary>
    public required Bitmap OutputBitmap { get; init; }

    /// <summary>
    /// Gets the generated mask bitmap when available.
    /// </summary>
    public Bitmap? MaskBitmap { get; init; }
}
