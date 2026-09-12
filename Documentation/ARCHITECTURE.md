# Architecture

Snapshot: 2026-08-26. Unity 6000.3.19f1. Runtime assembly: `PostmarkProtocol.Runtime`. Product namespace: `RehmanMughal.PostmarkProtocol`; child namespaces `Gameplay`, `Levels`, and `UI`. Editor recipes and validators use `.Editor`; test assemblies are Editor-only. The game has no account, server, remote data dependency, analytics, ads, IAP, or cloud-save service.

## Scenes and authored objects

| Scene | Responsibility |
|---|---|
| Bootstrap | Initialize local state and persistent audio, then enter MainDesk |
| MainDesk | Physical postal desk and authored menu, chapter ledger, level ledger, handbook, settings, credits and reset confirmation |
| Gameplay | Authored desk, camera, lights, parcel, mailbox, stamp impact object, all HUD/modal panels, and a route-content root |

`Assets/Editor/SceneAuthoring.cs` is an **Edit Mode authoring recipe** invoked through AnkleBreaker MCP. It saves actual scene GameObjects, materials and prefab assets. It refuses to run during Play Mode and is excluded from player compilation by its Editor folder. `new GameObject` calls in this recipe are authoring operations, not runtime UI generation.

`SceneUI` reads references to the saved hierarchy and binds controls; it does not construct the interface. `RoutePresentation.Load` instantiates only the authored RouteCell/RouteLink gameplay prefabs for the selected sheet and repositions the existing parcel/mailbox. All major UI and environment objects are inspectable in Edit Mode. `AudioAuthoring` similarly creates imported WAV assets, a real mixer and authored AudioSources before runtime.

## Dependency direction

`SceneUI → PlanState / RouteSimulator / LevelDefinition / SaveService / AudioService`.

`RoutePresentation → immutable simulation result frames → authored scene objects`.

The simulation does not call UI, audio, saving, physics or scene APIs. It uses `Vector2Int` and the supplied level data, so route outcomes are independent of frame rate and animation speed. Presentation finishes the result animation before `SceneUI.DeliveryFinished` records progress and shows the result panel.

## Core and level content

| Type | Responsibility |
|---|---|
| `Levels.LevelDefinition` | ScriptableObject containing dimensions, route cells, start/heading/destination, slots/ink restrictions, command stock, gates, hazards, gaps, checkpoints, parcel, budgets, par, optimal distance, lesson and known solution |
| `Gameplay.PlanState` | Validate placement, replacement, removal, ink locks, express restriction and stock/budget; reset clears permanent ink |
| `Gameplay.RouteSimulator` | Fresh tick-zero dispatch, persistent stamp execution on every visit, collision/timing/verification checks, bounded termination |
| `Gameplay.SimulationResult` | Success/reason, cell-distance steps, total ticks and ordered presentation frames |
| `Editor.LevelAuthoring` | Reproducibly author the 36 explicit route definitions into individual ScriptableObjects, preserving existing asset GUIDs |
| `Editor.LevelValidation` | Validate content, known solutions, deterministic replays, negative plans and mechanic rules |
| `Editor.LevelOptimality` | Exhaustive reachable-plan search proving minimum stamps/distance within legal stock, budget and time bounds; mutation audit |

No runtime random level generation, rotation-based level variants, separate per-level scenes, or giant level-selection switch is used. `LEVEL_DESIGN.md` records every sheet and exact command semantics.

## Input, UI and presentation

The authored EventSystem uses `InputSystemUIInputModule`. uGUI buttons, `StampDrag` pointer interfaces and `RouteCell.IPointerClickHandler` provide both tap-select/tap-place and drag/drop. The camera's PhysicsRaycaster supports route-cell clicks; explicit raycasts resolve drag drops. No keyboard input or hover is required. `PaperMotion` adds optional pointer feedback, and `PanelMotion` animates panels using unscaled time so pause/settings remain responsive.

`SceneUI.IsPlanning` requires the GameHUD and a non-running route. Dispatch disables planning controls; pause freezes presentation with `Time.timeScale = 0`; resume restores it. Restart clears the plan, whereas Review/TryAgain retains it. Scene exit restores time scale. `SafeArea` updates its RectTransform anchors when resolution or `Screen.safeArea` changes; the CanvasScaler uses a 1600×900 landscape reference.

`RoutePresentation` maps the maximum 8×6 grid onto the desk at 1.03 units per cell. A movement frame carries position, heading, integer tick, event name and duration. Positive durations are animated directly; zero-time commands receive a short presentation-only interval. The parcel follows the precomputed result; rendering cannot change delivery logic.

## Local persistence

`SaveRepository` stores version-2 JSON in a SHA-256 checksum envelope at `Application.persistentDataPath/postmark-save.json`. The checksum detects corruption; it is not encryption or authentication. Writes flush a temporary file and atomically replace the primary, with a last-good backup when the primary is trusted. Version-1 direct JSON migrates with defaults for new fields. Future-version records are loaded conservatively and not overwritten.

Normalization repairs invalid ranges, duplicate/out-of-range progress entries, null lists and non-finite volumes. Failed storage leaves current-session progress available and exposes a visible notice. Reset preserves audio/haptic preferences and updates both primary and backup so recovery cannot resurrect deliberately erased progress. `SaveService` provides the static application API; saves occur after delivery, tutorial completion, settings close, pause, exit and confirmed reset.

Progress contains sequential unlocks, Delivered/Efficient/Perfect seals, best stamp count, best steps and tutorial markers. Best results accumulate rather than being replaced by a worse replay.

## Audio and haptics

`AudioService` deduplicates authored instances and persists across scene changes. Three authored AudioSources route music, effects and UI through `PostalDesk.mixer`, with real Master/Music/SFX/UI buses and exposed dB parameters. The music slider controls Music; the effects slider controls SFX and UI; zero maps to −80 dB. Eighteen original synthesized effects and an original 24-second music loop are local assets.

The native iOS plugin `Assets/Plugins/iOS/PostmarkHaptics.mm` invokes UIKit light impact feedback; Android uses Unity vibration. Both obey the saved toggle and a 0.18-second throttle. Physical speaker, silent-switch and haptic behavior require device verification.

## Verification and build boundaries

Editor/NUnit checks are excluded from production. The latest observed integrated EditMode run, `e67520da5874`, passed 66/66 tests (51 gameplay, 15 services), including four differentiated-seal cases that exercise actual SaveRepository awards. The optional-slot content has all 36 exact route targets proven across 3,485 reachable partial plans, and 900 command mutations replayed deterministically. See the generated level reports and service tests for reproducible assertions.

AnkleBreaker is required Editor-only development tooling on port 7890; its local bridge is not game runtime networking. Use MCP tools for Unity operations, never direct bridge HTTP. Signed iOS archive/device validation require macOS/Xcode, authorized signing credentials and physical hardware; source configuration and Editor tests do not establish those release gates.
