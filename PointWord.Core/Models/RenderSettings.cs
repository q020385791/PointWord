using System.Drawing;

namespace PointWord.Core.Models;

/// <summary>
/// Defines all input parameters for generating one text-hole artwork bitmap.
/// </summary>
public sealed record RenderSettings
{
    /// <summary>
    /// Gets the target text used to build the hole mask.
    /// </summary>
    public string TargetText { get; init; } = "拳";

    /// <summary>
    /// Gets the repeated fill text rendered in each visible grid cell.
    /// </summary>
    public string FillText { get; init; } = "席話";

    /// <summary>
    /// Gets the number of cells per side (X) in an X by X square grid.
    /// </summary>
    public int GridCount { get; init; } = 32;

    /// <summary>
    /// Gets the pixel size of each grid cell.
    /// </summary>
    public int CellSizePx { get; init; } = 20;

    /// <summary>
    /// Gets the preferred target font family name.
    /// </summary>
    public string TargetFontFamily { get; init; } = "Microsoft JhengHei UI";

    /// <summary>
    /// Gets the base target font size used when constructing the vector path.
    /// </summary>
    public float TargetFontSize { get; init; } = 520f;

    /// <summary>
    /// Gets an extra scale factor applied after auto-fit.
    /// </summary>
    public float TargetScale { get; init; } = 1f;

    /// <summary>
    /// Gets the preferred fill text font family name.
    /// </summary>
    public string FillFontFamily { get; init; } = "Microsoft JhengHei UI";

    /// <summary>
    /// Gets the fill text font size in pixels before fit adjustments.
    /// </summary>
    public float FillFontSize { get; init; } = 11f;

    /// <summary>
    /// Gets the output background color.
    /// </summary>
    public Color BackgroundColor { get; init; } = Color.FromArgb(0xCF, 0xEF, 0xB8);

    /// <summary>
    /// Gets the fill text color.
    /// </summary>
    public Color FillTextColor { get; init; } = Color.Black;

    /// <summary>
    /// Gets the alpha threshold (0 to 255) used to classify mask pixels as hole.
    /// </summary>
    public byte MaskThreshold { get; init; } = 96;

    /// <summary>
    /// Gets whether rendering is per-character mode.
    /// </summary>
    public bool PerCharMode { get; init; }

    /// <summary>
    /// Gets the number of columns used when composing per-character square blocks.
    /// Set to 0 to place all blocks in one row.
    /// </summary>
    public int BlockColumns { get; init; } = 0;

    /// <summary>
    /// Gets the pixel spacing between per-character square blocks.
    /// </summary>
    public int BlockSpacingPx { get; init; } = 16;

    /// <summary>
    /// Gets the canvas padding ratio used by target text auto-fit.
    /// </summary>
    public float PaddingRatio { get; init; } = 0.08f;

    /// <summary>
    /// Gets whether grid lines are rendered for debugging.
    /// </summary>
    public bool DrawGridDebug { get; init; }

    /// <summary>
    /// Gets whether mask overlay is rendered for debugging.
    /// </summary>
    public bool ShowMaskDebug { get; init; }

    /// <summary>
    /// Gets a reusable default settings object.
    /// </summary>
    public static RenderSettings Default { get; } = new();
}

