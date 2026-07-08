using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using PointWord.Core.Models;
using PointWord.Core.Utils;

namespace PointWord.Core.Rendering;

/// <summary>
/// Builds an anti-aliased text mask bitmap used as the hole reference.
/// </summary>
public sealed class MaskBuilder
{
    private const float MinTargetScale = 0.1f;
    private const float MaxTargetScale = 2.0f;

    /// <summary>
    /// Creates a transparent alpha mask where target text area has high alpha values.
    /// </summary>
    /// <param name="settings">Render settings that control canvas size and typography.</param>
    /// <param name="targetText">Target text used to draw the mask.</param>
    /// <returns>A 32-bit ARGB bitmap mask.</returns>
    public Bitmap BuildMask(RenderSettings settings, string targetText)
    {
        ArgumentNullException.ThrowIfNull(settings);

        int canvasSize = Math.Max(1, settings.GridCount * settings.CellSizePx);
        Bitmap maskBitmap = new(canvasSize, canvasSize, PixelFormat.Format32bppArgb);

        using Graphics g = Graphics.FromImage(maskBitmap);
        ConfigureGraphics(g);
        g.Clear(Color.Transparent);

        if (string.IsNullOrWhiteSpace(targetText))
        {
            return maskBitmap;
        }

        using GraphicsPath path = BuildTargetPath(settings, targetText, canvasSize);
        if (path.PointCount == 0)
        {
            return maskBitmap;
        }

        using SolidBrush whiteBrush = new(Color.White);
        g.FillPath(whiteBrush, path);

        return maskBitmap;
    }

    private static GraphicsPath BuildTargetPath(RenderSettings settings, string targetText, int canvasSize)
    {
        GraphicsPath combinedPath = new();

        string normalizedText = targetText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

        string[] lines = normalizedText.Split('\n');
        string fontName = FontResolver.ResolveFontFamilyName(
            settings.TargetFontFamily,
            "Microsoft JhengHei UI",
            "Microsoft JhengHei",
            "Segoe UI Symbol",
            "Segoe UI");

        using FontFamily fontFamily = new(fontName);
        using StringFormat stringFormat = new(StringFormat.GenericTypographic)
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near,
            FormatFlags = StringFormatFlags.NoClip,
        };

        float emSize = Math.Max(24f, settings.TargetFontSize);
        float lineHeight = emSize * fontFamily.GetLineSpacing(FontStyle.Regular) / fontFamily.GetEmHeight(FontStyle.Regular);
        float y = 0f;

        foreach (string line in lines)
        {
            string lineText = line.Length == 0 ? " " : line;

            using GraphicsPath linePath = new();
            linePath.AddString(lineText, fontFamily, (int)FontStyle.Regular, emSize, new PointF(0f, 0f), stringFormat);

            RectangleF lineBounds = linePath.GetBounds();
            if (lineBounds.Width <= 0f || lineBounds.Height <= 0f)
            {
                y += lineHeight;
                continue;
            }

            using Matrix normalize = new();
            normalize.Translate(-lineBounds.Left, y - lineBounds.Top);
            linePath.Transform(normalize);

            combinedPath.AddPath(linePath, connect: false);
            y += lineHeight;
        }

        if (combinedPath.PointCount == 0)
        {
            return combinedPath;
        }

        FitPathToCanvas(settings, combinedPath, canvasSize);
        return combinedPath;
    }

    private static void FitPathToCanvas(RenderSettings settings, GraphicsPath path, int canvasSize)
    {
        RectangleF bounds = path.GetBounds();
        if (bounds.Width <= 0f || bounds.Height <= 0f)
        {
            return;
        }

        float clampedPadding = Math.Clamp(settings.PaddingRatio, 0f, 0.2f);
        float padding = canvasSize * clampedPadding;
        float availableWidth = Math.Max(1f, canvasSize - (padding * 2f));
        float availableHeight = Math.Max(1f, canvasSize - (padding * 2f));

        float autoFitScale = Math.Min(availableWidth / bounds.Width, availableHeight / bounds.Height);
        float targetScale = Math.Clamp(settings.TargetScale, MinTargetScale, MaxTargetScale);
        float scale = autoFitScale * targetScale;

        using Matrix normalizeAndScale = new();
        normalizeAndScale.Translate(-bounds.Left, -bounds.Top);
        normalizeAndScale.Scale(scale, scale);
        path.Transform(normalizeAndScale);

        RectangleF scaledBounds = path.GetBounds();
        float centerX = ((canvasSize - scaledBounds.Width) * 0.5f) - scaledBounds.Left;
        float centerY = ((canvasSize - scaledBounds.Height) * 0.5f) - scaledBounds.Top;

        using Matrix centerTransform = new();
        centerTransform.Translate(centerX, centerY);
        path.Transform(centerTransform);
    }

    private static void ConfigureGraphics(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
    }
}

