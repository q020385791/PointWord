# AGENTS.md — WinForms「挖空字 + 文字填充」產生器（給 Codex 的實作指引）

## 1. 目標（What to build）

做一個 **WinForms** 小工具：

- 使用者輸入「目標字」（例如：`拳`），把它當成 **挖空（negative space）** 的遮罩。
- 在 **目標字反選區域**（也就是「不在目標字筆畫內」的區域）用「填充字串」（例如：`席話`）重複排版填滿。
- 畫面以 **X × X 的正方形格子**構成（使用者可調整 X 與格子大小），每個格子內放一段填充字（可置中、可縮放）。
- 支援輸入 **多個目標字**，即時預覽產出（建議提供 Per-Char / Whole-Text 模式）。
- 可匯出 PNG（單張或多張）。

參考效果：整張畫布滿滿的「席話」，但中間「拳」字形狀的區域是空的（背景色露出）。

---

## 2. 必備功能（Must-have）

1. **即時產出（Live Preview）**
   - TargetText / FillText / GridCount / CellSize / Font / 顏色等參數一變動，預覽立即更新
   - 需要做 debounce（例如 150~300ms）避免每次 keypress 都重繪造成卡頓

2. **輸入**
   - 目標字（TargetText）：允許多字（例如 `拳王` 或 `拳\n王`）
   - 填充字（FillText）：任意字串（例如 `席話`、`ABC`、`台中市`）
   - 正方形格子數：`GridCount`（X），例如 24、32、48
   - 格子像素：`CellSize`（px），例如 16、20、24
   - 目標字縮放：`TargetScale`（0.1~2.0）或用字體大小 + 自動 fit
   - 遮罩閾值：`MaskThreshold`（0~255，預設 64~128）

3. **輸出**
   - 匯出 PNG（背景色、填充字顏色、解析度依設定）
   - 若 TargetText 多字：支援
     - PerChar：每個字輸出一張
     - WholeText：整串字輸出一張（置中、多行支援）

---

## 3. 非目標（Non-goals）

- 不需要做成雲端服務
- 不需要複雜排版引擎（例如沿曲線、沿筆畫方向排字）
- 不需要 OpenGL / GPU 加速（先用 GDI+ 就好）

---

## 4. 技術選型（Recommended）

- .NET：`net8.0-windows`
- UI：WinForms
- 繪圖：`System.Drawing`（Windows only，WinForms OK）
- 文字遮罩：使用 `GraphicsPath.AddString` 生成向量路徑或先 rasterize 成 alpha mask bitmap
- 預覽：`PictureBox`（顯示 Bitmap）
- 匯出：`Bitmap.Save(path, ImageFormat.Png)`

---

## 5. 專案結構（Project layout）

建立 Solution：`TextHoleArt`

- `TextHoleArt.App`（WinForms 主程式）
- `TextHoleArt.Core`（類別庫：渲染、遮罩、設定、輸出）
- （可選）`TextHoleArt.Tests`（基本測試：設定序列化、輸出尺寸）

建議資料夾：

- `TextHoleArt.Core/Rendering`
- `TextHoleArt.Core/Models`
- `TextHoleArt.Core/Utils`

---

## 6. UI 規格（WinForms）

主視窗 `MainForm`：

- 左側：參數面板（Panel）
  - TextBox: `txtTargetText`
  - TextBox: `txtFillText`
  - NumericUpDown: `numGridCount`（X）
  - NumericUpDown: `numCellSize`（px）
  - NumericUpDown: `numFillFontSize`
  - ComboBox: `cmbFillFontFamily`
  - NumericUpDown: `numTargetFontSize`（或 `numTargetScale`）
  - ComboBox: `cmbTargetFontFamily`
  - TrackBar/Numeric: `numMaskThreshold`
  - ColorPicker Buttons: 背景色、填充字顏色（以及可選：debug mask 顏色）
  - RadioButton:
    - `rbWholeText`（整串當成一個遮罩）
    - `rbPerChar`（每字一張）
  - Button: `btnExportPng`
  - CheckBox（可選）：
    - `chkDrawGridDebug`（畫格線）
    - `chkShowMaskDebug`（顯示遮罩 overlay）
- 右側：預覽區（PictureBox）
  - `picturePreview.SizeMode = Zoom`
  - 顯示目前生成結果（若 PerChar 模式，可用 ListBox/TabControl 選字預覽）

事件：

- 所有 input control 的 `TextChanged/ValueChanged/SelectedIndexChanged` -> 觸發 `RequestRender()`
- `RequestRender()` 啟動一個 WinForms Timer debounce，Timer tick 才真正 render

---

## 7. 核心資料模型（Core Models）

`RenderSettings`（immutable 或 record）：

- `string TargetText`
- `string FillText`
- `int GridCount` // X
- `int CellSizePx` // 每格 px
- `string TargetFontFamily`
- `float TargetFontSize` // 或 TargetScale
- `string FillFontFamily`
- `float FillFontSize`
- `Color BackgroundColor`
- `Color FillTextColor`
- `byte MaskThreshold` // alpha threshold
- `bool PerCharMode`
- `float PaddingRatio` // 0~0.2，預設 0.08
- `bool DrawGridDebug`
- `bool ShowMaskDebug`

`RenderResult`：

- `Bitmap OutputBitmap`
- `Bitmap? MaskBitmap`（debug 用）

---

## 8. 渲染演算法（核心）

### 8.1 畫布尺寸

- `canvasPx = GridCount * CellSizePx`
- 產出 Bitmap：`canvasPx × canvasPx`

### 8.2 生成「目標字遮罩」

做一張 `maskBitmap`（同尺寸，32bpp ARGB）：

- 背景透明（alpha=0）
- 把 TargetText 用白色（alpha=255）畫上去（或填滿 path）
- 要開啟 anti-alias，讓邊緣有灰階 alpha（方便 threshold）

**建議用 GraphicsPath：**

1. 建 `GraphicsPath path`
2. `path.AddString(TargetText, fontFamily, style, emSize, layoutRect, stringFormat)`
3. 用 `g.FillPath(Brushes.White, path)` 填到 maskBitmap

**layoutRect** 要讓字置中並可自動 fit：

- 先用「很大字」畫 path，取得 path bounds，再用比例縮放到畫布內（扣 padding）
- 或用二分搜尋找 fontSize 讓文字 bounds <= canvas\*(1-padding)

### 8.3 用遮罩做「挖空」

對於每一個格子 cell（i, j）：

- 算 cell 的中心點 `cx, cy`（或取多個 sample 點）
- 讀取 maskBitmap 在該點的 alpha
- `isHole = (alpha >= MaskThreshold)`
  - 如果 `isHole == true`：此格不畫填充字（讓背景色露出）
  - 否則：在此格畫 `FillText`

### 8.4 在格子中畫填充字

- 在 outputBitmap 上用 `Graphics.DrawString(FillText, fillFont, fillBrush, rect, centeredFormat)`
- centeredFormat：
  - `Alignment = Center`
  - `LineAlignment = Center`
- FillText 可能超出格子：
  - 方案 A：允許超出（但建議字體 size 由使用者調）
  - 方案 B：自動縮放 FillText 讓它 fit cell（建議做，體感較好）
    - 每格計算一次太慢：可以只針對 FillText 字串預先計算縮放倍率並重用

### 8.5 Debug（可選）

- `DrawGridDebug`：畫格線（半透明）
- `ShowMaskDebug`：把 maskBitmap 以半透明紅 overlay 到 output 方便看遮罩是否對齊

---

## 9. 效能要求（Performance）

- 目標：GridCount=48、CellSize=20（960×960）仍能流暢預覽
- debounce：避免頻繁重繪
- 避免在每一格做昂貴的字體測量：
  - FillText 的 fit 計算可在整體 render 前做一次（估算縮放）
- `Bitmap.GetPixel` 很慢：
  - 讀 alpha 建議用 `LockBits` + unsafe/Span 方式批次讀取
  - 或先把 mask alpha 抽成 `byte[] alphaMap`

---

## 10. 建議類別分工（Clean-ish）

`TextHoleArt.Core/Rendering`

- `MaskBuilder`
  - `Bitmap BuildMask(RenderSettings settings, string targetText)`
- `AlphaMap`
  - `byte[] FromBitmap(Bitmap mask)`（用 LockBits 拿 alpha）
  - `byte Sample(int x, int y)`
- `PatternRenderer`
  - `Bitmap Render(RenderSettings settings, string targetText)`
  - 流程：建立 output -> 建 mask -> alphaMap -> 走格子 -> 畫字
- `ExportService`
  - `void ExportPng(RenderSettings settings, string outDir, ExportMode mode)`

`TextHoleArt.App`

- `MainForm`
  - 維護目前 settings
  - debounce timer
  - 呼叫 Core renderer 更新 PictureBox
  - 匯出按鈕呼叫 ExportService

---

## 11. 匯出規格（Export）

- 目的資料夾：使用 FolderBrowserDialog 或 SaveFileDialog
- 檔名：
  - WholeText：`output_{timestamp}.png`
  - PerChar：`output_{char}_{timestamp}.png`（非法字元要 sanitize）
- 匯出解析度：
  - 預設同預覽（GridCount\*CellSize）
  - 可加倍輸出（例如 2x）— 可選

---

## 12. 驗收條件（Acceptance Criteria）

1. 輸入：
   - TargetText=`拳`，FillText=`席話`，GridCount=32，CellSize=20  
     -> 能看到「席話」鋪滿，且中間「拳」字區域為背景色挖空
2. 改 TargetText / FillText / GridCount / CellSize 後 0.3 秒內更新預覽
3. PerChar 模式輸入 `拳王` 可匯出 2 張 png
4. 匯出 PNG 打開後尺寸正確、背景與挖空效果正確

---

## 13. 實作順序（Step plan for Codex）

1. 建 solution + projects（App/Core）
2. 做 `RenderSettings` + 基本 UI（控件 + 事件 + debounce）
3. 先用最直覺版本完成渲染（即使先用 GetPixel 也可）
4. 確認效果後，把 mask 讀取改成 `LockBits`（改善效能）
5. 加入 PerChar / WholeText 模式與匯出功能
6. 加入 Debug 選項（格線、mask overlay）
7. 收尾：例外處理（空字串、字體不存在 fallback）、UI 體驗（預設值）

---

## 14. 實作提示（Gotchas）

- `GraphicsPath.AddString` 的 `emSize` 不是 fontSize 直接概念；建議用大值後再依 bounds 縮放
- 注意 `TextRenderingHint` / `SmoothingMode` / `PixelOffsetMode`：
  - `SmoothingMode.AntiAlias`
  - `TextRenderingHint.AntiAliasGridFit`
- 多行 TargetText：WholeText 模式要用 `StringFormat` 支援換行、置中
- 若使用者輸入 emoji 或罕見字，字體可能缺字：要 fallback 到 `Segoe UI Symbol` 或提示

---

## 15. 預設參數（方便開箱）

- TargetText: `拳`
- FillText: `席話`
- GridCount: 32
- CellSizePx: 20
- TargetFont: `Microsoft JhengHei UI`（或 `Microsoft JhengHei`）
- FillFont: `Microsoft JhengHei UI`
- TargetFontSize: auto-fit（或 520 之類再縮放）
- FillFontSize: 10~12（依 cellSize）
- BackgroundColor: 淡綠（#CFEFB8 類似）
- FillTextColor: 黑
- MaskThreshold: 96
- PaddingRatio: 0.08

---

## 16. Codex 工作方式（要求）

- 不要把渲染邏輯寫在 Form 裡：Form 只負責 UI 與呼叫 Core
- 產出可讀、可維護的 C#（清楚命名、少魔法數字）
- 所有 public API 要有 XML summary
- 避免 UI thread 卡住：render 可同步但要 debounce；如果畫布很大可改用 Task.Run + cancellation token（可選）

完成後請提供：

- 可直接 `dotnet run` 啟動
- 一鍵匯出 PNG
- README（簡短操作說明）
