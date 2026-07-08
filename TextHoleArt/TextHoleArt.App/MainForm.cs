using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using TextHoleArt.Core.Models;
using TextHoleArt.Core.Rendering;

namespace TextHoleArt.App;

/// <summary>
/// Main WinForms UI for configuring and previewing text-hole artwork.
/// </summary>
public sealed class MainForm : Form
{
    private const int RenderDebounceMs = 220;

    private readonly PatternRenderer patternRenderer = new();
    private readonly System.Windows.Forms.Timer renderDebounceTimer = new() { Interval = RenderDebounceMs };
    private readonly ColorDialog colorDialog = new() { FullOpen = true };

    private readonly TextBox txtTargetText = new()
    {
        Multiline = true,
        Height = 72,
        ScrollBars = ScrollBars.Vertical,
    };

    private readonly TextBox txtFillText = new();
    private readonly NumericUpDown numGridCount = CreateNumericInput(8, 128, 32, 0);
    private readonly NumericUpDown numCellSize = CreateNumericInput(4, 64, 20, 0);
    private readonly NumericUpDown numFillFontSize = CreateNumericInput(1, 96, 11, 1);
    private readonly ComboBox cmbFillFontFamily = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown numTargetFontSize = CreateNumericInput(24, 2000, 520, 0);
    private readonly NumericUpDown numTargetScale = CreateNumericInput(0.10M, 2.00M, 1.00M, 2, 0.05M);
    private readonly ComboBox cmbTargetFontFamily = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown numMaskThreshold = CreateNumericInput(0, 255, 96, 0);
    private readonly NumericUpDown numPaddingRatio = CreateNumericInput(0.00M, 0.20M, 0.08M, 2, 0.01M);

    private readonly Button btnBackgroundColor = new() { Text = "Background" };
    private readonly Button btnFillTextColor = new() { Text = "Fill Color" };
    private readonly Button btnCopyPreview = new() { Text = "Copy Preview" };

    private readonly RadioButton rbWholeText = new() { Text = "WholeText", Checked = true, AutoSize = true };
    private readonly RadioButton rbPerChar = new() { Text = "PerChar", AutoSize = true };

    private readonly ListBox lstPerChar = new()
    {
        Height = 88,
        SelectionMode = SelectionMode.One,
    };

    private readonly CheckBox chkDrawGridDebug = new() { Text = "Draw Grid", AutoSize = true };
    private readonly CheckBox chkShowMaskDebug = new() { Text = "Show Mask", AutoSize = true };

    private readonly PictureBox picturePreview = new()
    {
        Dock = DockStyle.Fill,
        SizeMode = PictureBoxSizeMode.Zoom,
        BorderStyle = BorderStyle.FixedSingle,
        BackColor = Color.White,
    };

    private readonly Label lblStatus = new()
    {
        AutoSize = false,
        Height = 24,
        Dock = DockStyle.Bottom,
        TextAlign = ContentAlignment.MiddleLeft,
        Text = "Ready",
    };

    private Color backgroundColor = RenderSettings.Default.BackgroundColor;
    private Color fillTextColor = RenderSettings.Default.FillTextColor;
    private Bitmap? currentPreviewBitmap;
    private bool isUpdatingPerCharList;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        PopulateFontFamilies();
        ApplyDefaultValues();
        BindEvents();
        RefreshPerCharList();
        UpdatePerCharSelectionState();
        UpdateColorButtonStyles();
        RequestRender();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            renderDebounceTimer.Dispose();
            colorDialog.Dispose();
            currentPreviewBitmap?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        Text = "TextHoleArt";
        ClientSize = new Size(1360, 860);
        MinimumSize = new Size(1080, 700);
        StartPosition = FormStartPosition.CenterScreen;

        SplitContainer splitContainer = new()
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 380,
            FixedPanel = FixedPanel.Panel1,
            Panel1MinSize = 320,
        };

        Controls.Add(splitContainer);

        Panel leftPanel = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(12),
        };

        splitContainer.Panel1.Controls.Add(leftPanel);

        TableLayoutPanel settingsTable = new()
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            GrowStyle = TableLayoutPanelGrowStyle.AddRows,
        };

        settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140f));
        settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        leftPanel.Controls.Add(settingsTable);

        AddLabeledRow(settingsTable, "TargetText", txtTargetText);
        AddLabeledRow(settingsTable, "FillText", txtFillText);
        AddLabeledRow(settingsTable, "GridCount", numGridCount);
        AddLabeledRow(settingsTable, "CellSize(px)", numCellSize);
        AddLabeledRow(settingsTable, "FillFontSize", numFillFontSize);
        AddLabeledRow(settingsTable, "FillFontFamily", cmbFillFontFamily);
        AddLabeledRow(settingsTable, "TargetFontSize", numTargetFontSize);
        AddLabeledRow(settingsTable, "TargetScale", numTargetScale);
        AddLabeledRow(settingsTable, "TargetFontFamily", cmbTargetFontFamily);
        AddLabeledRow(settingsTable, "MaskThreshold", numMaskThreshold);
        AddLabeledRow(settingsTable, "PaddingRatio", numPaddingRatio);

        FlowLayoutPanel modePanel = new()
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };

        modePanel.Controls.Add(rbWholeText);
        modePanel.Controls.Add(rbPerChar);
        AddLabeledRow(settingsTable, "Mode", modePanel);

        AddLabeledRow(settingsTable, "PerChar Pick", lstPerChar);

        FlowLayoutPanel debugPanel = new()
        {
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
        };

        debugPanel.Controls.Add(chkDrawGridDebug);
        debugPanel.Controls.Add(chkShowMaskDebug);
        AddLabeledRow(settingsTable, "Debug", debugPanel);

        FlowLayoutPanel colorPanel = new()
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
        };

        btnBackgroundColor.Width = 110;
        btnFillTextColor.Width = 110;
        colorPanel.Controls.Add(btnBackgroundColor);
        colorPanel.Controls.Add(btnFillTextColor);
        AddLabeledRow(settingsTable, "Colors", colorPanel);

        FlowLayoutPanel actionPanel = new()
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
        };

        btnCopyPreview.Width = 140;
        actionPanel.Controls.Add(btnCopyPreview);
        AddLabeledRow(settingsTable, "Actions", actionPanel);

        AddWideRow(settingsTable, lblStatus);

        Panel rightPanel = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
        };

        splitContainer.Panel2.Controls.Add(rightPanel);
        rightPanel.Controls.Add(picturePreview);

        ResumeLayout(performLayout: true);
    }

    private void BindEvents()
    {
        txtTargetText.TextChanged += (_, _) =>
        {
            RefreshPerCharList();
            RequestRender();
        };

        txtFillText.TextChanged += (_, _) => RequestRender();
        numGridCount.ValueChanged += (_, _) => RequestRender();
        numCellSize.ValueChanged += (_, _) => RequestRender();
        numFillFontSize.ValueChanged += (_, _) => RequestRender();
        numTargetFontSize.ValueChanged += (_, _) => RequestRender();
        numTargetScale.ValueChanged += (_, _) => RequestRender();
        numMaskThreshold.ValueChanged += (_, _) => RequestRender();
        numPaddingRatio.ValueChanged += (_, _) => RequestRender();

        cmbFillFontFamily.SelectedIndexChanged += (_, _) => RequestRender();
        cmbTargetFontFamily.SelectedIndexChanged += (_, _) => RequestRender();
        chkDrawGridDebug.CheckedChanged += (_, _) => RequestRender();
        chkShowMaskDebug.CheckedChanged += (_, _) => RequestRender();

        rbWholeText.CheckedChanged += (_, _) =>
        {
            UpdatePerCharSelectionState();
            RequestRender();
        };

        rbPerChar.CheckedChanged += (_, _) =>
        {
            UpdatePerCharSelectionState();
            RequestRender();
        };

        lstPerChar.SelectedIndexChanged += (_, _) =>
        {
            if (!isUpdatingPerCharList)
            {
                RequestRender();
            }
        };

        btnBackgroundColor.Click += (_, _) => PickBackgroundColor();
        btnFillTextColor.Click += (_, _) => PickFillColor();
        btnCopyPreview.Click += (_, _) => CopyPreviewToClipboard();

        renderDebounceTimer.Tick += (_, _) =>
        {
            renderDebounceTimer.Stop();
            RenderPreview();
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

        backgroundColor = defaults.BackgroundColor;
        fillTextColor = defaults.FillTextColor;

        rbPerChar.Checked = defaults.PerCharMode;
        rbWholeText.Checked = !defaults.PerCharMode;
        chkDrawGridDebug.Checked = defaults.DrawGridDebug;
        chkShowMaskDebug.Checked = defaults.ShowMaskDebug;
    }

    private void RefreshPerCharList()
    {
        string? selected = (lstPerChar.SelectedItem as PerCharItem)?.Value;

        isUpdatingPerCharList = true;
        lstPerChar.BeginUpdate();

        try
        {
            lstPerChar.Items.Clear();

            foreach (string character in GetTargetCharacters(txtTargetText.Text))
            {
                lstPerChar.Items.Add(new PerCharItem(character));
            }

            if (lstPerChar.Items.Count == 0)
            {
                return;
            }

            int selectedIndex = 0;
            if (!string.IsNullOrEmpty(selected))
            {
                for (int i = 0; i < lstPerChar.Items.Count; i++)
                {
                    if ((lstPerChar.Items[i] as PerCharItem)?.Value == selected)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            lstPerChar.SelectedIndex = selectedIndex;
        }
        finally
        {
            lstPerChar.EndUpdate();
            isUpdatingPerCharList = false;
        }
    }

    private void UpdatePerCharSelectionState()
    {
        bool enabled = rbPerChar.Checked;
        lstPerChar.Enabled = enabled;
    }

    private void RequestRender()
    {
        renderDebounceTimer.Stop();
        renderDebounceTimer.Start();
    }

    private void RenderPreview()
    {
        RenderSettings settings = BuildSettingsFromUi();
        string previewTargetText = ResolvePreviewTargetText(settings);

        if (string.IsNullOrWhiteSpace(previewTargetText))
        {
            SetPreviewBitmap(CreateBlankPreview(settings));
            lblStatus.Text = "TargetText is empty.";
            return;
        }

        try
        {
            RenderResult result = patternRenderer.Render(settings, previewTargetText);
            result.MaskBitmap?.Dispose();

            SetPreviewBitmap(result.OutputBitmap);
            lblStatus.Text = $"Rendered {DateTime.Now:HH:mm:ss}";
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
            BackgroundColor = backgroundColor,
            FillTextColor = fillTextColor,
            PerCharMode = rbPerChar.Checked,
            DrawGridDebug = chkDrawGridDebug.Checked,
            ShowMaskDebug = chkShowMaskDebug.Checked,
        };
    }

    private string ResolvePreviewTargetText(RenderSettings settings)
    {
        if (!settings.PerCharMode)
        {
            return settings.TargetText;
        }

        return (lstPerChar.SelectedItem as PerCharItem)?.Value ?? string.Empty;
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

    private void SetPreviewBitmap(Bitmap bitmap)
    {
        Bitmap? old = currentPreviewBitmap;
        currentPreviewBitmap = bitmap;
        picturePreview.Image = currentPreviewBitmap;
        old?.Dispose();
    }

    private static Bitmap CreateBlankPreview(RenderSettings settings)
    {
        int size = Math.Max(1, settings.GridCount * settings.CellSizePx);
        Bitmap bitmap = new(size, size, PixelFormat.Format32bppArgb);

        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(settings.BackgroundColor);

        using Font font = new("Segoe UI", Math.Max(12f, size / 24f), FontStyle.Regular, GraphicsUnit.Pixel);
        using SolidBrush brush = new(Color.FromArgb(180, Color.Black));
        using StringFormat centered = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        g.DrawString("TargetText required", font, brush, new RectangleF(0, 0, size, size), centered);
        return bitmap;
    }

    private static NumericUpDown CreateNumericInput(decimal min, decimal max, decimal value, int decimals, decimal increment = 1)
    {
        return new NumericUpDown
        {
            Minimum = min,
            Maximum = max,
            Value = value,
            DecimalPlaces = decimals,
            Increment = increment,
            ThousandsSeparator = false,
            Width = 140,
        };
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

    private static IEnumerable<string> GetTargetCharacters(string rawText)
    {
        string normalized = rawText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

        foreach (char ch in normalized)
        {
            if (ch == '\n')
            {
                continue;
            }

            yield return ch.ToString();
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

    private static void AddLabeledRow(TableLayoutPanel table, string labelText, Control editor)
    {
        Label label = new()
        {
            AutoSize = true,
            Text = labelText,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 6, 0, 10),
        };

        editor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        editor.Margin = new Padding(0, 3, 0, 10);

        int row = table.RowCount;
        table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(label, 0, row);
        table.Controls.Add(editor, 1, row);
    }

    private static void AddWideRow(TableLayoutPanel table, Control control)
    {
        control.Margin = new Padding(0, 4, 0, 2);

        int row = table.RowCount;
        table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.Controls.Add(control, 0, row);
        table.SetColumnSpan(control, 2);
    }

    private sealed record PerCharItem(string Value)
    {
        public override string ToString()
        {
            return Value switch
            {
                " " => "[space]",
                "\t" => "[tab]",
                _ => Value,
            };
        }
    }
}
