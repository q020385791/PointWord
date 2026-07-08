# TextHoleArt

WinForms tool for generating negative-space text art with repeated fill text.

## Projects

- `TextHoleArt.App`: WinForms UI (left settings panel, right live preview).
- `TextHoleArt.Core`: rendering logic and models (`RenderSettings`, `MaskBuilder`, `PatternRenderer`).

## Run

```bash
dotnet run --project TextHoleArt.App/TextHoleArt.App.csproj
```

## Current Features

- Debounced live preview (220ms) on input changes.
- WholeText / PerChar preview mode.
- Configurable grid, cell size, font, threshold, colors, padding.
- Optional debug overlays (grid and mask).
- Copy current preview bitmap to clipboard.

## Default Values

- `TargetText`: `拳`
- `FillText`: `席話`
- `GridCount`: `32`
- `CellSizePx`: `20`
- `MaskThreshold`: `96`

## Notes

- `System.Drawing` rendering is Windows-only.
- This step includes live preview + copy workflow; PNG export can be added next.
