# Audio slider verification

UTC: 2026-08-28 11:00:41Z

Result: **PASS**; 6/6 scene/resolution checks.

Native Unity pointer-event checks in Editor Play Mode, not physical touch or subjective listening. Both sliders: 0, 1, 25, 50, 76, 94, 99 and 100 percent; taps, reverse drags, out-of-range clamping, matching track/fill/handle/label/save/mixer values, independent Music/SFX/UI buses, old fractional settings, file reload, real audio-source playback and layout checks. Original user repository restored at completion. No iOS build/export or signing.

- MainDesk sliders: 1136x640: PASS text extents and screen bounds
- MainDesk (1136, 640): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS
- Gameplay sliders: 1136x640: PASS text extents and screen bounds
- Gameplay (1136, 640): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS
- MainDesk sliders: 2778x1284: PASS text extents and screen bounds
- MainDesk (2778, 1284): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS
- Gameplay sliders: 2778x1284: PASS text extents and screen bounds
- Gameplay (2778, 1284): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS
- MainDesk sliders: 2732x2048: PASS text extents and screen bounds
- MainDesk (2732, 2048): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS
- Gameplay sliders: 2732x2048: PASS text extents and screen bounds
- Gameplay (2732, 2048): tap/drag, endpoint clamping, track/fill/thumb alignment, labels, independent mixer buses, legacy settings and file reload PASS

Regression suite: 68/68 NUnit tests passed (EditMode job 8fd6b8e50dbc, 0 failed/skipped). CompilationPipeline: 0 errors/warnings after the changes. Saved scene slider rectangles were also checked in Edit Mode; authored thumb/fill anchors retain the default 45%/80% positions.
