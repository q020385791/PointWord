# PointWord

PointWord 是一個 Windows WinForms 小工具，用來產生「挖空字 + 重複文字填充」的 PNG 圖片。使用者輸入目標文字後，程式會把目標文字轉成遮罩，並在遮罩外的格子區域重複繪製填充文字，形成中間字形被挖空、背景色露出的效果。

## 功能特色

- 即時預覽：輸入或參數變更後會以 debounce 方式重新渲染。
- 目標文字遮罩：使用 `GraphicsPath` 建立反鋸齒文字遮罩。
- 格子填字：以 `GridCount x GridCount` 的正方形格子繪製填充文字。
- WholeText 模式：整段目標文字當成同一個遮罩。
- PerChar Blocks 模式：將多個目標字分別渲染成方塊後排版成一張預覽圖。
- 可調整字型、字體大小、目標字縮放、遮罩閾值、邊距比例、格子數與格子大小。
- 可選擇背景色與填充文字顏色。
- Debug 顯示：可顯示格線與遮罩覆蓋。
- 可複製目前預覽圖到剪貼簿。
- 可匯出目前預覽圖為 PNG。

## 環境需求

- Windows
- .NET SDK 10.0 或以上
- Visual Studio 2026 或可執行 .NET 10 專案的開發環境

目前主專案目標框架為 `net10.0-windows`，並使用 Windows Forms 與 `System.Drawing`，因此需要在 Windows 環境執行。

## 專案結構

```text
PointWord/
├─ PointWord.slnx
├─ PointWord/
│  ├─ PointWord.csproj
│  ├─ Program.cs
│  ├─ Form1.cs
│  └─ Form1.Designer.cs
├─ PointWord.Core/
│  ├─ Models/
│  │  ├─ RenderSettings.cs
│  │  └─ RenderResult.cs
│  ├─ Rendering/
│  │  ├─ AlphaMap.cs
│  │  ├─ MaskBuilder.cs
│  │  └─ PatternRenderer.cs
│  └─ Utils/
│     ├─ FontResolver.cs
│     └─ TargetTextSplitter.cs
└─ TextHoleArt/
   └─ 舊版或參考用專案
```

主要執行入口是 `PointWord/PointWord.csproj`。根目錄的 `PointWord.slnx` 目前引用 `PointWord` 與 `PointWord.Core` 兩個專案。

## 快速開始

在專案根目錄執行：

```powershell
dotnet restore PointWord.slnx
dotnet run --project PointWord/PointWord.csproj
```

也可以用 Visual Studio 開啟 `PointWord.slnx` 後直接執行 `PointWord` 專案。

## 使用方式

1. 在 `TargetText` 輸入要挖空的目標字，例如 `拳` 或 `拳王`。
2. 在 `FillText` 輸入要重複填滿畫面的文字，例如 `席話`。
3. 調整 `GridCount` 與 `CellSize(px)` 控制輸出解析度。
4. 調整 `TargetFontSize`、`TargetScale`、`MaskThreshold` 與 `PaddingRatio` 控制挖空字大小與遮罩判定。
5. 選擇 `WholeText` 或 `PerChar Blocks`：
   - `WholeText`：整段目標文字一起形成一個遮罩。
   - `PerChar Blocks`：每個字各自生成一個方塊，再組成一張圖。
6. 使用 `Background` 與 `Fill Color` 按鈕調整顏色。
7. 點擊 `Copy Preview` 可複製目前預覽。
8. 點擊 `Export PNG` 可將目前預覽匯出為 PNG。

## 預設參數

- `TargetText`: `拳`
- `FillText`: `席話`
- `GridCount`: `32`
- `CellSizePx`: `20`
- `TargetFontFamily`: `Microsoft JhengHei UI`
- `FillFontFamily`: `Microsoft JhengHei UI`
- `TargetFontSize`: `520`
- `TargetScale`: `1.0`
- `FillFontSize`: `11`
- `MaskThreshold`: `96`
- `PaddingRatio`: `0.08`
- `BackgroundColor`: `#CFEFB8`
- `FillTextColor`: 黑色

預設輸出尺寸為：

```text
GridCount * CellSizePx = 32 * 20 = 640px
```

因此 WholeText 模式預設會產生 `640 x 640` 的圖片。

## 核心渲染流程

1. 根據 `GridCount` 與 `CellSizePx` 建立正方形畫布。
2. 使用 `MaskBuilder` 將目標文字轉成透明背景、白色文字的 alpha mask。
3. 使用 `AlphaMap` 透過 `LockBits` 擷取遮罩 alpha 值，避免大量使用 `GetPixel`。
4. `PatternRenderer` 逐格取樣 cell 中心點：
   - alpha 大於等於 `MaskThreshold`：視為挖空區，不畫填充文字。
   - alpha 小於 `MaskThreshold`：在格子中置中繪製 `FillText`。
5. 視設定加上 debug 格線或遮罩覆蓋。
6. 將結果顯示在 WinForms `PictureBox`，並可匯出為 PNG。

## 常見問題

### 為什麼某些字顯示不出來？

程式會優先使用你選擇的字型，若系統沒有該字型，會 fallback 到 `Microsoft JhengHei UI`、`Microsoft JhengHei`、`Segoe UI Symbol` 或 `Segoe UI`。如果輸入的是罕見字或 emoji，仍可能受限於系統字型支援。

### 為什麼 PerChar 模式不是每個字輸出一張檔？

目前程式的 `PerChar Blocks` 會把每個字先渲染成獨立方塊，再組合成同一張預覽圖。`Export PNG` 會匯出目前畫面，所以 PerChar 模式目前會匯出一張組合圖。

### 匯出的 PNG 尺寸怎麼算？

WholeText 模式下，圖片寬高為：

```text
GridCount * CellSizePx
```

PerChar Blocks 模式下，寬高會依照字數、`Block Columns`、單格方塊大小與 `Block Spacing` 計算。
