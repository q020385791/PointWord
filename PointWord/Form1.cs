using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using PointWord.Core.Models;
using PointWord.Core.Rendering;
using PointWord.Core.Utils;

namespace PointWord;

public partial class Form1 : Form
{
    private const int RenderDebounceMs = 220;

    private readonly PatternRenderer patternRenderer = new();
    private readonly System.Windows.Forms.Timer renderDebounceTimer = new() { Interval = RenderDebounceMs };
    private readonly ColorDialog colorDialog = new() { FullOpen = true };

    private Color backgroundColor = RenderSettings.Default.BackgroundColor;
    private Color fillTextColor = RenderSettings.Default.FillTextColor;
    private Bitmap? currentPreviewBitmap;

    public Form1()
    {
        InitializeComponent();

        if (IsDesignMode())
        {
            return;
        }

        PopulateFontFamilies();
        ApplyDefaultValues();
        BindEvents();
        UpdateModeControlState();
        UpdateColorButtonStyles();
        RequestRender();
    }

    private void BindEvents()
    {
        txtTargetText.TextChanged += (_, _) => RequestRender();
        txtFillText.TextChanged += (_, _) => RequestRender();
        numGridCount.ValueChanged += (_, _) => RequestRender();
        numCellSize.ValueChanged += (_, _) => RequestRender();
        numFillFontSize.ValueChanged += (_, _) => RequestRender();
        numTargetFontSize.ValueChanged += (_, _) => RequestRender();
        numTargetScale.ValueChanged += (_, _) => RequestRender();
        numMaskThreshold.ValueChanged += (_, _) => RequestRender();
        numPaddingRatio.ValueChanged += (_, _) => RequestRender();
        numBlockColumns.ValueChanged += (_, _) => RequestRender();
        numBlockSpacing.ValueChanged += (_, _) => RequestRender();

        cmbFillFontFamily.SelectedIndexChanged += (_, _) => RequestRender();
        cmbTargetFontFamily.SelectedIndexChanged += (_, _) => RequestRender();
        chkDrawGridDebug.CheckedChanged += (_, _) => RequestRender();
        chkShowMaskDebug.CheckedChanged += (_, _) => RequestRender();

        rbWholeText.CheckedChanged += (_, _) =>
        {
            UpdateModeControlState();
            RequestRender();
        };

        rbPerChar.CheckedChanged += (_, _) =>
        {
            UpdateModeControlState();
            RequestRender();
        };

        btnBackgroundColor.Click += (_, _) => PickBackgroundColor();
        btnFillTextColor.Click += (_, _) => PickFillColor();
        btnCopyPreview.Click += (_, _) => CopyPreviewToClipboard();
        btnExportPng.Click += (_, _) => ExportPreviewPng();

        renderDebounceTimer.Tick += (_, _) =>
        {
            renderDebounceTimer.Stop();
            RenderPreview();
        };

        FormClosed += (_, _) =>
        {
            renderDebounceTimer.Dispose();
            colorDialog.Dispose();
            currentPreviewBitmap?.Dispose();
        };
    }

    private void PopulateFontFamilies()
    {
        InstalledFontCollection installedFonts = new();
        string[] families = installedFonts.Families
            .Select(fontFamily => fontFamily.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        cmbFillFontFamily.Items.Clear();
        cmbTargetFontFamily.Items.Clear();

        cmbFillFontFamily.Items.AddRange(families);
        cmbTargetFontFamily.Items.AddRange(families);

        SelectPreferredFont(cmbTargetFontFamily, RenderSettings.Default.TargetFontFamily, "Microsoft JhengHei", "Segoe UI");
        SelectPreferredFont(cmbFillFontFamily, RenderSettings.Default.FillFontFamily, "Microsoft JhengHei", "Segoe UI");
    }

    private void ApplyDefaultValues()
    {
        RenderSettings defaults = RenderSettings.Default;

        txtTargetText.Text = defaults.TargetText;
        txtFillText.Text = defaults.FillText;
        numGridCount.Value = defaults.GridCount;
        numCellSize.Value = defaults.CellSizePx;
        numFillFontSize.Value = (decimal)defaults.FillFontSize;
        numTargetFontSize.Value = (decimal)defaults.TargetFontSize;
        numTargetScale.Value = (decimal)defaults.TargetScale;
        numMaskThreshold.Value = defaults.MaskThreshold;
        numPaddingRatio.Value = (decimal)defaults.PaddingRatio;
        numBlockColumns.Value = defaults.BlockColumns <= 0 ? 8 : defaults.BlockColumns;
        numBlockSpacing.Value = defaults.BlockSpacingPx;

        backgroundColor = defaults.BackgroundColor;
        fillTextColor = defaults.FillTextColor;

        rbPerChar.Checked = defaults.PerCharMode;
        rbWholeText.Checked = !defaults.PerCharMode;
        chkDrawGridDebug.Checked = defaults.DrawGridDebug;
        chkShowMaskDebug.Checked = defaults.ShowMaskDebug;
    }

    private void UpdateModeControlState()
    {
        bool perCharMode = rbPerChar.Checked;
        numBlockColumns.Enabled = perCharMode;
        numBlockSpacing.Enabled = perCharMode;
    }

    private void RequestRender()
    {
        renderDebounceTimer.Stop();
        renderDebounceTimer.Start();
    }

    private void RenderPreview()
    {
        RenderSettings settings = BuildSettingsFromUi();

        try
        {
            RenderResult result;

            if (settings.PerCharMode)
            {
                IReadOnlyList<string> targetCharacters = TargetTextSplitter.SplitCharacters(settings.TargetText);
                if (targetCharacters.Count == 0)
                {
                    SetPreviewBitmap(CreateBlankPreview(settings, "TargetText required"));
                    lblStatus.Text = "TargetText is empty.";
                    return;
                }

                result = patternRenderer.RenderCharacterBlocks(settings, targetCharacters);
                lblStatus.Text = $"Rendered {targetCharacters.Count} blocks ({DateTime.Now:HH:mm:ss})";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(settings.TargetText))
                {
                    SetPreviewBitmap(CreateBlankPreview(settings, "TargetText required"));
                    lblStatus.Text = "TargetText is empty.";
                    return;
                }

                result = patternRenderer.Render(settings, settings.TargetText);
                lblStatus.Text = $"Rendered whole text ({DateTime.Now:HH:mm:ss})";
            }

            result.MaskBitmap?.Dispose();
            SetPreviewBitmap(result.OutputBitmap);
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Render failed: {ex.Message}";
        }
    }

    private RenderSettings BuildSettingsFromUi()
    {
        return new RenderSettings
        {
            TargetText = txtTargetText.Text,
            FillText = txtFillText.Text,
            GridCount = (int)numGridCount.Value,
            CellSizePx = (int)numCellSize.Value,
            FillFontSize = (float)numFillFontSize.Value,
            FillFontFamily = cmbFillFontFamily.SelectedItem as string ?? RenderSettings.Default.FillFontFamily,
            TargetFontSize = (float)numTargetFontSize.Value,
            TargetScale = (float)numTargetScale.Value,
            TargetFontFamily = cmbTargetFontFamily.SelectedItem as string ?? RenderSettings.Default.TargetFontFamily,
            MaskThreshold = (byte)numMaskThreshold.Value,
            PaddingRatio = (float)numPaddingRatio.Value,
            BlockColumns = rbPerChar.Checked ? (int)numBlockColumns.Value : 0,
            BlockSpacingPx = (int)numBlockSpacing.Value,
            BackgroundColor = backgroundColor,
            FillTextColor = fillTextColor,
            PerCharMode = rbPerChar.Checked,
            DrawGridDebug = chkDrawGridDebug.Checked,
            ShowMaskDebug = chkShowMaskDebug.Checked,
        };
    }

    private void PickBackgroundColor()
    {
        colorDialog.Color = backgroundColor;
        if (colorDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        backgroundColor = colorDialog.Color;
        UpdateColorButtonStyles();
        RequestRender();
    }

    private void PickFillColor()
    {
        colorDialog.Color = fillTextColor;
        if (colorDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        fillTextColor = colorDialog.Color;
        UpdateColorButtonStyles();
        RequestRender();
    }

    private void UpdateColorButtonStyles()
    {
        ApplyColorButtonStyle(btnBackgroundColor, backgroundColor);
        ApplyColorButtonStyle(btnFillTextColor, fillTextColor);
    }

    private void CopyPreviewToClipboard()
    {
        if (currentPreviewBitmap is null)
        {
            lblStatus.Text = "No preview image to copy.";
            return;
        }

        try
        {
            Image clipboardImage = (Image)currentPreviewBitmap.Clone();
            Clipboard.SetImage(clipboardImage);
            lblStatus.Text = "Preview copied to clipboard.";
        }
        catch (ExternalException ex)
        {
            lblStatus.Text = $"Clipboard busy: {ex.Message}";
        }
    }

    private void ExportPreviewPng()
    {
        if (currentPreviewBitmap is null)
        {
            lblStatus.Text = "No preview image to export.";
            return;
        }

        using SaveFileDialog saveDialog = new()
        {
            Title = "Export PNG",
            Filter = "PNG Image|*.png",
            DefaultExt = "png",
            AddExtension = true,
            FileName = $"pointword_{DateTime.Now:yyyyMMdd_HHmmss}.png",
        };

        if (saveDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            currentPreviewBitmap.Save(saveDialog.FileName, ImageFormat.Png);
            lblStatus.Text = $"Exported: {saveDialog.FileName}";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Export failed: {ex.Message}";
        }
    }

    private void SetPreviewBitmap(Bitmap bitmap)
    {
        Bitmap? old = currentPreviewBitmap;
        currentPreviewBitmap = bitmap;
        picturePreview.Image = currentPreviewBitmap;
        old?.Dispose();
    }

    private static Bitmap CreateBlankPreview(RenderSettings settings, string message)
    {
        int size = Math.Max(1, settings.GridCount * settings.CellSizePx);
        Bitmap bitmap = new(size, size, PixelFormat.Format32bppArgb);

        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(settings.BackgroundColor);

        using Font font = new("Segoe UI", Math.Max(12f, size / 24f), FontStyle.Regular, GraphicsUnit.Pixel);
        using SolidBrush brush = new(Color.FromArgb(180, Color.Black));
        using StringFormat centered = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        g.DrawString(message, font, brush, new RectangleF(0, 0, size, size), centered);
        return bitmap;
    }

    private static void SelectPreferredFont(ComboBox comboBox, params string[] preferredNames)
    {
        foreach (string name in preferredNames)
        {
            int index = comboBox.FindStringExact(name);
            if (index >= 0)
            {
                comboBox.SelectedIndex = index;
                return;
            }
        }

        if (comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }
    }

    private static void ApplyColorButtonStyle(Button button, Color color)
    {
        button.UseVisualStyleBackColor = false;
        button.BackColor = color;
        button.ForeColor = GetReadableTextColor(color);
    }

    private static Color GetReadableTextColor(Color background)
    {
        double luma = (0.299 * background.R) + (0.587 * background.G) + (0.114 * background.B);
        return luma >= 140d ? Color.Black : Color.White;
    }

    private static bool IsDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    }
}
