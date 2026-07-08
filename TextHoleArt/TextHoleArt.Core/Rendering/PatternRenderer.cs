using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TextHoleArt.Core.Models;
using TextHoleArt.Core.Utils;

namespace TextHoleArt.Core.Rendering;

/// <summary>
/// Renders the grid-based fill text pattern and applies text-hole masking.
/// </summary>
public sealed class PatternRenderer
{
    private readonly MaskBuilder maskBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternRenderer"/> class.
    /// </summary>
    public PatternRenderer()
        : this(new MaskBuilder())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternRenderer"/> class with a custom mask builder.
    /// </summary>
    /// <param name="maskBuilder">Mask builder used to generate target text masks.</param>
    public PatternRenderer(MaskBuilder maskBuilder)
    {
        this.maskBuilder = maskBuilder ?? throw new ArgumentNullException(nameof(maskBuilder));
    }

    /// <summary>
    /// Renders one artwork image from settings and target text.
    /// </summary>
    /// <param name="settings">Render settings.</param>
    /// <param name="targetText">Target text to use when creating hole regions.</param>
    /// <returns>A render result containing output and debug mask bitmap.</returns>
    public RenderResult Render(RenderSettings settings, string targetText)
    {
        ArgumentNullException.ThrowIfNull(settings);

        int gridCount = Math.Max(1, settings.GridCount);
        int cellSize = Math.Max(1, settings.CellSizePx);
        int canvasSize = gridCount * cellSize;
        string fillText = string.IsNullOrWhiteSpace(settings.FillText) ? " " : settings.FillText;

        Bitmap outputBitmap = new(canvasSize, canvasSize, PixelFormat.Format32bppArgb);
        Bitmap? maskBitmap = null;

        try
        {
            using Graphics g = Graphics.FromImage(outputBitmap);
            ConfigureGraphics(g);

            using SolidBrush backgroundBrush = new(settings.BackgroundColor);
            g.FillRectangle(backgroundBrush, 0, 0, canvasSize, canvasSize);

            RenderSettings normalizedSettings = settings with
            {
                GridCount = gridCount,
                CellSizePx = cellSize,
            };

            maskBitmap = maskBuilder.BuildMask(normalizedSettings, targetText);
            AlphaMap alphaMap = AlphaMap.FromBitmap(maskBitmap);

            using StringFormat centered = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoClip,
            };

            string fillFontName = FontResolver.ResolveFontFamilyName(
                settings.FillFontFamily,
                "Microsoft JhengHei UI",
                "Microsoft JhengHei",
                "Segoe UI Symbol",
                "Segoe UI");

            using Font fillFont = CreateCellFillFont(g, fillText, fillFontName, settings.FillFontSize, cellSize, centered);
            using SolidBrush fillBrush = new(settings.FillTextColor);

            for (int row = 0; row < gridCount; row++)
            {
                int y = row * cellSize;
                int sampleY = y + (cellSize / 2);

                for (int col = 0; col < gridCount; col++)
                {
                    int x = col * cellSize;
                    int sampleX = x + (cellSize / 2);

                    if (alphaMap.Sample(sampleX, sampleY) >= settings.MaskThreshold)
                    {
                        continue;
                    }

                    RectangleF cellRect = new(x, y, cellSize, cellSize);
                    g.DrawString(fillText, fillFont, fillBrush, cellRect, centered);
                }
            }

            if (settings.DrawGridDebug)
            {
                DrawGrid(g, gridCount, cellSize, canvasSize);
            }

            if (settings.ShowMaskDebug)
            {
                DrawMaskOverlay(g, alphaMap, gridCount, cellSize, settings.MaskThreshold);
            }

            return new RenderResult
            {
                OutputBitmap = outputBitmap,
                MaskBitmap = maskBitmap,
            };
        }
        catch
        {
            outputBitmap.Dispose();
            maskBitmap?.Dispose();
            throw;
        }
    }

    private static Font CreateCellFillFont(
        Graphics g,
        string fillText,
        string fillFontName,
        float requestedFontSize,
        int cellSize,
        StringFormat stringFormat)
    {
        float baseSize = Math.Max(1f, requestedFontSize);

        using Font probeFont = new(fillFontName, baseSize, FontStyle.Regular, GraphicsUnit.Pixel);
        SizeF measured = g.MeasureString(fillText, probeFont, int.MaxValue, stringFormat);

        float maxWidth = cellSize * 0.92f;
        float maxHeight = cellSize * 0.92f;

        float widthRatio = maxWidth / Math.Max(1f, measured.Width);
        float heightRatio = maxHeight / Math.Max(1f, measured.Height);
        float fitRatio = Math.Min(widthRatio, heightRatio);

        float finalSize = baseSize * Math.Min(1f, fitRatio);
        finalSize = Math.Max(1f, finalSize);

        return new Font(fillFontName, finalSize, FontStyle.Regular, GraphicsUnit.Pixel);
    }

    private static void DrawGrid(Graphics g, int gridCount, int cellSize, int canvasSize)
    {
        using Pen gridPen = new(Color.FromArgb(60, Color.Black), 1f);

        for (int i = 0; i <= gridCount; i++)
        {
            int p = i * cellSize;
            g.DrawLine(gridPen, p, 0, p, canvasSize);
            g.DrawLine(gridPen, 0, p, canvasSize, p);
        }
    }

    private static void DrawMaskOverlay(Graphics g, AlphaMap alphaMap, int gridCount, int cellSize, byte threshold)
    {
        using SolidBrush overlayBrush = new(Color.FromArgb(85, Color.Red));

        for (int row = 0; row < gridCount; row++)
        {
            int y = row * cellSize;
            int sampleY = y + (cellSize / 2);

            for (int col = 0; col < gridCount; col++)
            {
                int x = col * cellSize;
                int sampleX = x + (cellSize / 2);

                if (alphaMap.Sample(sampleX, sampleY) < threshold)
                {
                    continue;
                }

                g.FillRectangle(overlayBrush, x, y, cellSize, cellSize);
            }
        }
    }

    private static void ConfigureGraphics(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
    }
}
