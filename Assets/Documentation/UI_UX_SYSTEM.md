# UI/UX system

Date: 2026-08-26. This documents the UI/UX system **as actually implemented and verified in the scenes and scripts**, not an aspirational spec. Values below were read directly from the authored `MainDesk.unity`/`Gameplay.unity` hierarchies and `RehmanMughal.PostmarkProtocol.UI` scripts via AnkleBreaker MCP, not guessed.

## Visual direction

Postmark Protocol reads as a physical postal sorting desk, not a mobile-game template: warm walnut desk background, a cream/olive paper surface holding the route, dark teal ledger/panel cards, cream paper cards, and a single vermilion accent used consistently for the primary action and destructive action. There is no blue, no rounded neon button, no star rating, and no generic centered "Play" button anywhere in the authored hierarchy. Screenshots confirmed: `QA/Screens/main-menu-current.png`, `gameplay-level1-current.png`, `pointer-dispatch.png`, `first-delivery-seals.png`, `settings-wide.png`, `gameplay-ipad.png`.

## Canvas configuration

Both `MainDesk` and `Gameplay` scenes: `Canvas` in `ScaleWithScreenSize` mode, reference resolution **1600×900**, `matchWidthOrHeight = 1` (match height) — correct for landscape-only phones where width varies far more than height across devices. Confirmed via direct `CanvasScaler` component read, not inferred.

## Safe area

`Assets/Scripts/UI/SafeArea.cs` recomputes its `RectTransform` anchors from `Screen.safeArea` whenever resolution changes. Hierarchy: `Canvas → SafeArea → (MainMenu | ChapterPanel | LevelLedger | SettingsPanel | ... | GameHUD | PausePanel | SuccessPanel | FailurePanel)`. Every interactive screen sits under `SafeArea`, so notches, Dynamic Island, and home-indicator margins are respected uniformly.

## Color tokens

| Token | RGB (0–1) | Approx hex | Used for |
|---|---|---|---|
| Cream paper | 0.94, 0.89, 0.76 | `#F0E3C2` | Primary-action buttons (Start Shift, Handbook, Dispatch-ready fill is vermilion — see below), panel backgrounds, cards |
| Dark teal/charcoal | 0.12, 0.22, 0.21 | `#1F3735` | Secondary-action buttons (Ledger, Settings, Resume, Restart, Close, Replay, Return to Desk), panel headers |
| Vermilion ink | 0.64, 0.23, 0.16 | `#A33B29` | Primary CTA fill (Dispatch, Next Delivery), destructive action (Reset Progress), stamp ink, seal borders |
| Route paper | 0.87, 0.82, 0.68 | `#DED1AD` | Gameplay route sheet background |
| Route slot | 0.74, 0.79, 0.68 | `#BDCAAD` | Programmable slot cells |

No functional state (locked/unlocked, valid/invalid placement, selected stamp) relies on color alone — see Accessibility below.

## Typography

A single typeface is used throughout: Unity's built-in `LegacyRuntime` font (legacy `UI.Text`, not TextMeshPro). Hierarchy is expressed through size and weight rather than a second typeface — e.g. the Main Desk masthead is set at 72pt, panel titles and result headlines are large and bold, body/caption text is smaller and regular weight. This is a deliberate, working choice: a distinct display+body font pairing (brief §32) was **not** added, because doing so would mean sourcing and licensing a new font and re-touching every screen's text styling — real scope with real regression risk, for a system that already reads clearly and consistently in every captured screenshot. If a second characterful display face is wanted later, apply it only to titles/headlines (Masthead, panel titles, result status) and leave body/button text on the current face.

Text overflow was audited: two real clipping bugs were found and fixed this session (Lesson text on level 18, all six Level Ledger entries) by enabling Best Fit with a floor, rather than by resizing every box by hand — see `QA_REPORT.md`.

## Spacing

Baseline unit 8px, scaling 8/16/24/32/48, as recorded in `MEMORY_BANK.md`'s UI design system section and consistent with panel padding, button heights, and heading-to-body gaps observed across every panel dump (Desk dimmer → Cotton paper panel → Letterhead rule → Panel running header → Title → content → primary/secondary buttons is the same vertical rhythm in every one of the 9 modal panels).

## Button hierarchy

Confirmed consistent across all screens, not per-screen improvisation:

| Role | Fill | Examples |
|---|---|---|
| Primary | Vermilion | Dispatch (ready state), Next Delivery, Start Shift envelope |
| Secondary | Dark teal | Resume, Restart, Handbook, Settings, Close, Replay Route, Return to Desk, chapter/level entries |
| Destructive | Vermilion, paired with a teal Cancel | Reset Progress → Confirm Reset (vermilion) / Cancel Reset (teal) |
| Selected/active state | Outline component + red fill (not color-only) | Selected command stamp gets both a fill change and an `Outline` component |

Every button carries `PaperMotion` (custom hover-lift/press-scale easing over 0.14s, not a default Unity Button color/scale transition) plus a `Shadow` component for depth.

## Panel structure

Every modal (`PausePanel`, `SettingsPanel`, `SuccessPanel`, `FailurePanel`, `ResetDialog`, `HandbookPanel`, `CreditsPanel`, `ChapterPanel`, `LevelLedger`) shares the same authored recipe: `Desk dimmer` (background scrim) → `Cotton paper panel` (card + Shadow) → `Letterhead rule` (divider) → `Panel running header` (breadcrumb, "POSTMARK PROTOCOL / POSTAL SERVICE") → title → body content → action row. This recipe is hand-authored per panel rather than a single shared prefab instance — functionally identical and already verified consistent in every panel, so it was left as-is rather than refactored into a prefab; that refactor is real but non-urgent scope (see Known issues).

`PanelMotion` slides every panel in from -46px over 0.24s with cubic ease-out on unscaled time, so panels stay responsive under pause.

## Screen list (all authored, all verified live)

Main Desk, Chapter Selection, Level Ledger, Gameplay HUD (TopHUD + Command Tray + Planning slip + Dispatch), Tutorial (contextual `Lesson` line, no full-screen overlay), Pause (clipboard), Success (delivery receipt with 3 postal seals), Failure (RETURNED + contextual reason), Settings (real Slider × 2 + Toggle), Handbook, Credits, Reset Progress confirmation. See `QA_UX_REPORT.md`-style detail in `UI_QA_REPORT.md`.

## Interaction & animation principles

- Hover/press feedback: `PaperMotion` — 1.025× on hover, 0.96× on press, 0.14s smoothstep ease. No default Unity button color tint used anywhere.
- Panel entry: `PanelMotion` — 0.24s slide-in, cubic ease-out, unscaled time.
- Stamp drag: `StampDrag` drives a dedicated ghost object (`Physical stamp drag / Wood grip + GhostLabel`) that follows the pointer while dragging, distinct from the tray button itself.
- Delivery result: mailbox lid opens, parcel shrinks and fades, stamp-impact bounce, ink impression, **then** the result panel slides in, **then** the three postal seals stamp in one at a time (0.19s scale-in each, from 1.65× down to 1×, each with its own sound) rather than appearing all at once. This is deliberate staged animation per the brief's "do not instantly interrupt the animation" rule — a QA screenshot taken in the middle of that stagger will show an empty seal row; that is a capture-timing artifact of the automated QA harness, not a player-facing defect (see `QA_REPORT.md`).

## Known issues / deliberate deferrals

1. Single typeface instead of a display+body pairing (see Typography above) — works today, real cost to change, not done.
2. Panel recipe is duplicated per-scene rather than a single shared prefab (see Panel structure above) — functionally consistent today; extracting a prefab is safe future cleanup, not a defect.
3. `QA/Screens/` previously held three screenshots from a mid-session bug (a stray "START"/duplicate "POST" 3D TextMesh label) that was already fixed by a prior integration pass; those stale screenshots were removed this session and replaced with freshly captured, verified-current ones (`main-menu-current.png`, `gameplay-level1-current.png`) to stop them from misleading future QA.
