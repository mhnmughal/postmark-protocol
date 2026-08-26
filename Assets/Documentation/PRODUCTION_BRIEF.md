# CODEX MASTER PROMPT

## Build Postmark Protocol From Scratch as a Complete Production-Ready Unity Game

You are the **Lead Unity Game Development Agent** responsible for building a complete, polished, production-ready mobile puzzle game called:

# POSTMARK PROTOCOL

**Tagline:** Stamp the instructions. Send the route.

You must develop the entire game from scratch inside Unity using **AnkleBreaker MCP**.

This is not a prototype task.

You are responsible for:

* project bootstrap
* AnkleBreaker MCP configuration
* MCP connectivity verification
* architecture
* gameplay systems
* complete UI/UX
* environment
* visual design
* level design
* 36 playable levels
* animation
* VFX
* sound
* settings
* persistence
* tutorial
* accessibility considerations
* game icon
* iOS configuration
* bundle identifier
* performance
* automated testing
* manual QA
* visual QA
* App Store readiness
* cleanup
* documentation
* memory bank
* AI rules
* final audit

Do not declare the project complete while any acceptance criterion in this prompt fails.

Do not stop after creating placeholder systems.

Do not stop after creating only several sample levels.

Do not leave TODOs, stub buttons, fake settings, temporary screens, placeholder mechanics, broken navigation, unfinished art, unused debug controls, or incomplete levels.

Continue implementing, testing, inspecting, fixing, and retesting until the game satisfies the entire completion specification.

---

# 1. ABSOLUTE DEVELOPMENT RULES

The following rules are mandatory.

## 1.1 Start From a Fresh Project

This game must be developed as an independent product.

Do not:

* clone another game
* reskin another game
* copy another game's hierarchy
* reuse another game's UI layout
* duplicate another game's level system
* reuse another game's navigation
* reuse another game's icon
* reuse another game's gameplay code
* import unnecessary template packs
* use purchased game templates
* use starter-game frameworks
* add unrelated SDKs

Use a clean architecture specifically designed for Postmark Protocol.

---

# 2. ANKLEBREAKER MCP MUST BE FIRST

Before gameplay development begins:

1. Inspect the current Unity project.
2. Configure AnkleBreaker MCP correctly.
3. Add any project-local configuration required for AnkleBreaker.
4. Establish communication with Unity Editor.
5. Verify the connection.
6. Perform a simple test operation through MCP.
7. Confirm that Unity reflects the MCP change.
8. Remove the temporary test object if created.
9. Record MCP setup information inside `MEMORY_BANK.md`.

Do not begin production development until MCP communication has been verified.

If MCP initially fails:

* inspect configuration
* inspect package/settings
* inspect connection state
* repair it
* retry
* verify again

Do not simply bypass AnkleBreaker and perform the whole development without it.

---

# 3. CREATE PROJECT MEMORY SYSTEM

At the project root create:

`MEMORY_BANK.md`

and:

`AI_RULES.md`

These files are mandatory.

---

# 4. MEMORY_BANK.md

`MEMORY_BANK.md` is the persistent project memory for every AI agent working on this game.

It must contain at minimum:

## Project Identity

* Game: Postmark Protocol
* Genre
* target platforms
* orientation
* bundle identifier
* Unity version
* game architecture
* scene structure

## Game Design

* core mechanic
* control scheme
* progression
* command stamps
* ink rules
* parcel types
* chapters
* scoring/seal system
* win conditions
* failure conditions

## Technical Architecture

Record:

* important scripts
* managers
* ScriptableObjects
* services
* save system
* audio system
* level system
* UI system
* scene responsibilities

## UI Design System

Record:

* margins
* typography
* button sizes
* panel rules
* corner radii
* icon conventions
* responsive behavior
* safe-area rules
* landscape/portrait decisions

## Environment Design

Record:

* camera position
* lighting
* desk setup
* paper dimensions
* prop placement
* playable-area dimensions
* materials
* visual hierarchy

## Completed Work

Maintain checklists showing which:

* systems
* screens
* chapters
* levels
* audio assets
* tests
* audits

are finished.

## Known Problems

Every discovered bug must temporarily be added here.

Remove it from the known issues section once verified as fixed.

## Important Decisions

Whenever architecture or game-design decisions change, update this file.

The memory bank must remain accurate throughout development.

---

# 5. AI_RULES.md

Create a strict AI development rules document.

It must instruct all current and future agents to follow these rules.

Include at minimum:

### Rule 1

Never replace production systems with placeholders.

### Rule 2

Never silently remove a requirement.

### Rule 3

Never create buttons that do nothing.

### Rule 4

Never create fake settings.

### Rule 5

Never leave compiler warnings caused by our code without investigating them.

### Rule 6

Never introduce network functionality.

### Rule 7

Never introduce analytics.

### Rule 8

Never introduce advertisements.

### Rule 9

Never introduce tracking.

### Rule 10

Never introduce third-party SDKs unless explicitly authorized.

### Rule 11

Never add remote configuration.

### Rule 12

Never create hidden gameplay modes.

### Rule 13

Never expose debug tools in production.

### Rule 14

Never generate the primary production UI entirely at runtime.

### Rule 15

Important UI must exist as authored GameObjects in the Unity scene hierarchy.

### Rule 16

Environment composition must exist visibly in the Unity scene hierarchy.

### Rule 17

Respect existing architecture before modifying systems.

### Rule 18

Update `MEMORY_BANK.md` after major changes.

### Rule 19

Test changes after implementing them.

### Rule 20

Never claim something works without verification.

### Rule 21

Never mark the game complete if an acceptance criterion remains unfinished.

### Rule 22

Maintain responsive UI and safe-area support.

### Rule 23

All shipped audio and graphical assets must have a clear legal origin.

### Rule 24

Do not copy assets from other games or projects.

### Rule 25

Keep Postmark Protocol mechanically and visually internally consistent.

---

# 6. PARALLEL SPECIALIST AGENTS

Use multiple specialist agents where parallel work is safe.

The Lead Agent remains responsible for integration.

Create responsibilities conceptually equivalent to:

## Agent A: MCP + Project Bootstrap

Responsible for:

* AnkleBreaker configuration
* Unity connection
* folder architecture
* project rules
* memory bank initialization
* platform configuration

---

## Agent B: Gameplay Architecture

Responsible for:

* route simulation
* command execution
* envelope state
* movement
* stamp placement
* collision behavior
* dispatch simulation
* success/failure logic

---

## Agent C: Level Design

Responsible for:

* 36 complete levels
* difficulty progression
* validation
* par stamp counts
* chapter mechanics
* solvability

---

## Agent D: UI/UX

Responsible for:

* main menu
* chapter selection
* level selection
* gameplay HUD
* pause
* settings
* handbook
* results
* confirmation dialogs
* responsive layout

---

## Agent E: Environment + Art Direction

Responsible for:

* physical postal desk
* paper
* envelope
* stamp props
* ink pads
* clips
* lighting
* shadows
* materials
* scene composition

---

## Agent F: Animation + Feedback

Responsible for:

* rubber stamp motion
* envelope movement
* mailbox animation
* menu transitions
* paper movement
* seal animation
* failure feedback
* haptics

---

## Agent G: Sound Design

Responsible for:

* original sound effects
* original ambient/music loop if appropriate
* mixer
* volumes
* settings
* sound testing

---

## Agent H: Typography + UI Polish

Responsible for auditing:

* text alignment
* truncation
* wrapping
* font sizing
* hierarchy
* spacing
* button labels
* contrast
* panel alignment
* safe areas

---

## Agent I: iOS + Icon

Responsible for:

* bundle identifier
* product name
* application identifier
* player settings
* orientations
* icons
* build settings
* iPhone/iPad behavior
* safe area

---

## Agent J: QA

Responsible for:

* automated tests
* gameplay tests
* navigation tests
* save tests
* audio tests
* visual inspection
* performance checks
* level validation

---

## Agent K: Final Compliance Audit

Responsible for detecting:

* hidden features
* unused alternate gameplay
* unused debug menus
* network usage
* unwanted SDKs
* analytics
* tracking
* remote content
* placeholder UI
* broken links
* unfinished screens
* metadata/functionality mismatches

Agents may work in parallel only when their modifications do not conflict.

The Lead Agent must merge, inspect, test, and resolve all integrations.

---

# 7. GAME IDENTITY

Game title:

# Postmark Protocol

Bundle Identifier:

`com.rehmanmughal.postmarkprotocol`

Internal product name:

`PostmarkProtocol`

Company/developer namespace:

`RehmanMughal.PostmarkProtocol`

Use clean namespaces throughout the C# project.

Example:

```csharp
namespace RehmanMughal.PostmarkProtocol.Gameplay
{
}
```

---

# 8. TARGET PLATFORM

Primary target:

* iOS
* iPhone
* iPad

Game should remain architecture-compatible with Android, but iOS production quality is the immediate priority.

---

# 9. ORIENTATION

Use:

## Landscape

Design the whole game specifically around landscape orientation.

Do not merely stretch a portrait interface.

Support appropriate landscape orientations according to final iOS configuration.

Audit safe areas on devices with:

* notch
* Dynamic Island
* rounded corners
* home indicator
* older rectangular displays

---

# 10. COMPLETELY OFFLINE

Postmark Protocol must function without internet connectivity.

Do not implement:

* server access
* authentication
* account creation
* analytics
* advertisements
* IAP
* web views
* telemetry
* tracking
* ATT
* cloud saves
* remote config
* downloadable content
* external APIs
* remote leaderboards

The game must remain fully playable in airplane mode.

---

# 11. CORE GAME CONCEPT

Postmark Protocol is a tactile 2.5D programming puzzle.

Each level takes place on a physical postal route sheet placed on a desk.

An envelope begins at a starting point.

A delivery mailbox or postal destination exists elsewhere.

The player does NOT directly move the envelope.

Instead, the player places physical instruction stamps onto designated programmable route spaces.

The player then presses:

# DISPATCH

The envelope automatically follows the programmed commands.

The fundamental loop is:

### Inspect

Understand the route.

### Stamp

Place commands.

### Dispatch

Run the route.

### Observe

Watch the simulated delivery.

### Correct

Change instructions if necessary.

### Deliver

Reach the destination.

---

# 12. COMMAND STAMPS

Implement these seven commands:

## TURN LEFT

Rotate movement direction 90 degrees left.

## TURN RIGHT

Rotate movement direction 90 degrees right.

## WAIT

Pause movement for one timing cycle.

## REVERSE

Rotate movement direction 180 degrees.

## FAST

Increase movement speed for the applicable segment.

## SLOW

Reduce movement speed.

## JUMP

Cross one designated small obstacle/gap.

Every command needs:

* distinct physical stamp
* distinct icon
* readable label
* visual feedback
* sound feedback
* gameplay validation

---

# 13. PHYSICAL STAMP INTERACTION

Stamps must feel tactile.

The interaction must include:

1. player selects/grabs stamp
2. stamp lifts slightly
3. shadow strengthens
4. stamp follows pointer/finger
5. valid slot highlights subtly
6. stamp moves downward
7. physical compression animation
8. impact
9. slight camera impulse
10. haptic feedback
11. ink imprint appears
12. stamp returns to holder

Do not simply make the stamp behave like a generic UI icon.

---

# 14. INK SYSTEM

Create three ink behaviors.

## Standard Ink

Command executes normally.

## Double Ink

Command executes twice.

## Permanent Ink

Command remains locked for the current puzzle configuration where applicable.

The game must clearly communicate ink behavior visually.

Avoid relying only on color.

Use:

* icon
* pattern
* text
* stamp-border treatment

where needed.

---

# 15. PARCEL TYPES

Implement four parcel behaviors.

## Standard Letter

Normal movement.

## Fragile Parcel

Cannot collide with designated bump/hazard sections.

## Express Mail

Cannot use WAIT commands.

## Registered Mail

Must pass specified verification checkpoints before final delivery.

Each parcel must have:

* unique visual appearance
* readable label
* gameplay behavior
* tutorial introduction

---

# 16. LEVEL STRUCTURE

Create:

# 36 COMPLETE HANDCRAFTED LEVELS

Six chapters.

Six levels per chapter.

No duplicated level layouts with only superficial modifications.

All levels must be solvable.

---

# 17. CHAPTER 1: LOCAL ROUTE

Levels 1 to 6.

Teach:

* dispatch
* left turn
* right turn
* command slots
* route reading
* stamp replacement
* successful delivery

Difficulty must increase gently.

Level 1 must be nearly impossible to misunderstand.

Do not use a large tutorial text wall.

---

# 18. CHAPTER 2: EXPRESS DESK

Levels 7 to 12.

Introduce:

* WAIT
* FAST
* SLOW
* timed route obstacles
* moving gates

Require basic synchronization.

---

# 19. CHAPTER 3: RETURN OFFICE

Levels 13 to 18.

Introduce:

* REVERSE
* directional reasoning
* route revisiting
* dead-end recovery
* looping paths

---

# 20. CHAPTER 4: INK LAB

Levels 19 to 24.

Introduce:

* standard ink
* double ink
* permanent ink
* combined command effects

Levels must focus on reasoning rather than simply increasing movement speed.

---

# 21. CHAPTER 5: PRIORITY SERVICE

Levels 25 to 30.

Introduce:

* fragile parcels
* express parcels
* registered parcels
* verification checkpoints
* route constraints

---

# 22. CHAPTER 6: NIGHT SHIFT

Levels 31 to 36.

Combine all learned mechanics.

Use a nighttime desk presentation.

Final levels should require mastery of:

* command sequence
* timing
* ink
* parcel restrictions
* checkpoints
* limited stamp availability

Level 36 must function as a satisfying final challenge.

---

# 23. LEVEL DATA ARCHITECTURE

Use ScriptableObjects for level configuration.

Create something equivalent to:

`LevelDefinition`

Include:

* level ID
* chapter
* level name
* grid/route configuration
* start position
* start direction
* destination
* available commands
* command limits
* programmable slots
* obstacles
* moving obstacle timing
* parcel type
* checkpoints
* ink restrictions
* par count
* tutorial trigger
* visual modifiers

Do not hardcode every level into giant switch statements.

---

# 24. LEVEL VALIDATION

Create an Editor validation system.

For all levels verify:

* unique ID
* valid start
* valid destination
* valid slots
* available commands
* command limit
* checkpoints
* parcel compatibility
* references
* no missing assets

Where feasible, implement deterministic route simulations to verify known solutions.

Every one of the 36 levels must be manually play-tested as well.

---

# 25. COMPLETION REWARDS

Do NOT use generic three-star scoring.

Create postal seals.

## Delivered Seal

Award for completing the level.

## Efficient Seal

Award for meeting the par stamp target.

## Perfect Route Seal

Award for achieving the optimal route requirement.

Represent these as physical postal seals/stamps.

---

# 26. SAVE SYSTEM

Completely local.

Persist:

* unlocked chapter
* unlocked level
* completed levels
* Delivered seal
* Efficient seal
* Perfect Route seal
* best command count
* tutorial state
* music volume
* sound volume
* haptics
* settings state

Use a versioned local save model.

Gracefully recover from corrupted or unavailable save data.

Do not lose progress because one save field changes.

---

# 27. SCENE ARCHITECTURE

Keep scenes purposeful.

Recommended structure:

## Bootstrap

Persistent application services.

## MainDesk

Main menu and navigation environment.

## Gameplay

All gameplay levels loaded through level definitions.

Do not create 36 separate scenes unless there is a strong technical reason.

---

# 28. CRITICAL HIERARCHY REQUIREMENT

The production UI and major environment must be **manually authored into Unity scene hierarchies using AnkleBreaker MCP**.

Do NOT create the complete interface using runtime C# like:

```csharp
new GameObject(...)
```

inside `Awake()` or `Start()`.

Do not build the entire desk procedurally when entering Play Mode.

When the Unity scene is opened in Edit Mode, the hierarchy must visibly contain the major composition.

---

# 29. MAIN DESK HIERARCHY

Create an organized hierarchy approximately like:

```text
MainDesk
├── Systems
├── Environment
│   ├── DeskSurface
│   ├── DeskMat
│   ├── MainEnvelope
│   ├── PostalLedger
│   ├── Handbook
│   ├── SettingsGear
│   ├── InkPad
│   ├── PaperClips
│   ├── Pencil
│   ├── DecorativeMail
│   └── DeskLamp
├── Lighting
├── CameraRig
├── UI
│   ├── SafeArea
│   ├── MainMenu
│   ├── ChapterPanel
│   ├── LevelLedger
│   ├── SettingsPanel
│   ├── HandbookPanel
│   ├── CreditsPanel
│   └── DialogLayer
└── Audio
```

Names may improve during implementation, but organization must remain professional.

---

# 30. GAMEPLAY HIERARCHY

Create something equivalent to:

```text
Gameplay
├── Systems
├── CameraRig
├── Lighting
├── Environment
│   ├── Desk
│   ├── RoutePaper
│   ├── Mailbox
│   ├── StampRack
│   ├── InkPads
│   └── Props
├── Route
│   ├── RouteNodes
│   ├── ProgrammableSlots
│   ├── Obstacles
│   └── Checkpoints
├── Parcel
├── Effects
├── UI
│   └── SafeArea
│       ├── TopHUD
│       ├── CommandTray
│       ├── DispatchControl
│       ├── PauseButton
│       ├── TutorialLayer
│       └── ResultLayer
└── Audio
```

Again, these must be authored objects visible in Edit Mode.

---

# 31. DYNAMIC OBJECT RULE

Dynamic runtime creation is acceptable for genuine gameplay objects such as:

* command tokens
* route indicators
* temporary particles
* level-specific obstacles
* simulation effects

But reusable production objects should come from authored prefabs.

UI architecture must not be created from scratch every time the game runs.

---

# 32. UNIQUE UI/UX DIRECTION

Do not use a generic mobile-game interface.

Avoid:

* giant rounded rectangle Play button
* generic blue popup panels
* generic star results screen
* floating buttons with no environmental relationship
* identical centered menu used in common hypercasual templates

Everything should feel like part of a postal work desk.

---

# 33. MAIN MENU

The main menu is a physical desk.

Primary objects:

### Start Shift

Represented by a physical envelope.

### Level Ledger

Represented by an open postal ledger.

### Handbook

Represented by a small manual/book.

### Settings

Represented by a desk gear/tag or appropriate physical object.

### Credits

Placed subtly inside the handbook/settings experience.

Interactive objects should react to:

* hover where applicable
* touch
* selection
* press

with appropriate movement and feedback.

---

# 34. LEVEL SELECT

Create a physical postal ledger.

The ledger displays six chapters.

Each chapter should have a distinctive page header.

Example:

`CHAPTER 01`

`LOCAL ROUTE`

Show six delivery entries.

Each entry shows:

* level number
* completion state
* earned postal seals

Locked entries should be visually clear without looking disabled by accident.

---

# 35. GAMEPLAY HUD

Keep gameplay screen clean.

Required controls:

* pause
* level identifier
* remaining commands if relevant
* stamp tray
* dispatch button

Do not cover the playable paper unnecessarily.

---

# 36. DISPATCH BUTTON

Make Dispatch a distinctive physical control.

For example:

A postal dispatch lever/button on the side of the desk.

Do not use a generic floating rectangle saying "GO".

Interaction must have:

* press animation
* sound
* visual state
* disabled state while route is running

---

# 37. PAUSE MENU

Use a physical clipboard sliding into view.

Options:

* Resume Route
* Restart Delivery
* Instructions
* Return to Desk

All buttons must work.

---

# 38. SETTINGS

Implement functional settings:

## Music

Volume slider/toggle.

## Sound Effects

Volume slider/toggle.

## Haptics

Toggle.

## Reset Progress

Must require confirmation.

## Credits

Shows development and audio attribution information.

Settings must persist between sessions.

---

# 39. TEXT AND TYPOGRAPHY AUDIT

This is mandatory.

Every screen must be inspected for:

* text clipping
* incorrect anchors
* overlapping text
* inconsistent capitalization
* poor hierarchy
* line-break errors
* insufficient padding
* unsafe-edge placement
* text outside panels
* button label misalignment

No text may visually touch panel borders.

No label may appear accidentally off-center.

---

# 40. UI ALIGNMENT SYSTEM

Use consistent spacing tokens.

Suggested baseline:

* 8 px small unit
* 16 px standard
* 24 px section
* 32 px large
* 48 px major separation

Scale appropriately through Canvas Scaler.

Use consistent:

* button heights
* panel padding
* icon size
* text baselines
* heading spacing

Avoid arbitrary one-off numbers without justification.

---

# 41. RESPONSIVE UI

Test at multiple aspect ratios.

At minimum simulate:

* older 16:9 iPhone
* modern iPhone
* wider iPhone
* iPad
* iPad Pro-style aspect ratio

Nothing may:

* overlap
* clip
* leave the screen
* become unusably small
* become excessively stretched

---

# 42. ENVIRONMENT DESIGN

Create a polished warm physical desk aesthetic.

The environment should contain carefully placed, manually authored objects such as:

* wooden desk
* route paper
* envelopes
* rubber stamps
* ink pads
* paper clips
* pencil
* postal labels
* postal ledger
* desk lamp
* small mail stack

Do not clutter the playable area.

The environment should support the gameplay visually.

---

# 43. MATERIALS

Create a coherent material set.

Examples:

* warm wood
* cream paper
* red postal ink
* blue postal ink
* dark permanent ink
* rubber stamp base
* wooden stamp handle
* metallic paper clip
* muted mailbox material

Avoid excessive saturation.

Avoid a cheap neon hypercasual appearance.

---

# 44. LIGHTING

Use warm desk lighting.

Requirements:

* readable gameplay
* soft directional shadows
* stamps clearly separated from paper
* environment not too dark
* Night Shift chapter visibly different but still readable

Optimize shadows for mobile performance.

---

# 45. CAMERA

Use a slight 2.5D top-down angle.

The route paper must remain easy to read.

Do not overuse perspective.

No unnecessary camera movement.

Use very subtle:

* transitions
* focus
* stamping impulse
* completion framing

---

# 46. ANIMATION

Required animation set:

* stamp pickup
* stamp hover
* stamp placement
* stamp impact
* ink appearance
* envelope start
* envelope movement
* envelope turning
* wait
* reverse
* jump
* mailbox opening
* mailbox delivery
* Returned failure
* Delivered completion
* seal application
* ledger movement
* clipboard pause transition
* settings transition

Animations must be polished and consistently timed.

---

# 47. FAILURE PRESENTATION

Do not use:

`GAME OVER`

Instead show a postal response such as:

# RETURNED

With contextual reason:

* Route Blocked
* Fragile Parcel Damaged
* Verification Missed
* Invalid Express Route
* Delivery Failed

Provide:

* Try Again
* Review Route

---

# 48. SUCCESS PRESENTATION

When the envelope reaches its destination:

1. mailbox opens
2. envelope enters
3. delivery sound plays
4. stamp descends
5. `DELIVERED` appears as an ink impression
6. earned seals appear
7. progression saves
8. Next Delivery becomes available

Do not instantly interrupt the animation.

---

# 49. TUTORIAL

Tutorial must be integrated into gameplay.

Level 1:

### Step 1

Highlight the required command stamp.

Text:

`Pick up the Turn Right stamp.`

### Step 2

Highlight the slot.

Text:

`Stamp the marked route space.`

### Step 3

Highlight Dispatch.

Text:

`Dispatch the letter.`

After this, tutorials should be contextual.

Never display a multi-paragraph tutorial before gameplay begins.

---

# 50. AUDIO RULE

All shipped game audio must be legally safe.

Preferred approach:

# Generate original audio locally.

Create original/procedural sounds for:

* stamp pickup
* stamp impact
* paper movement
* envelope slide
* dispatch
* mailbox
* UI selection
* button
* page turn
* success
* failure
* seal reward

If music is used, create an original subtle postal-desk ambient loop.

Do not download random sounds.

If any external audio is absolutely required, it must be:

* CC0
* public domain
* or appropriately licensed for commercial redistribution

and its exact source/license must be documented.

When provenance cannot be verified, do not ship the asset.

---

# 51. AUDIO DOCUMENTATION

Create:

`Assets/Documentation/AUDIO_LICENSES.md`

For original assets write:

* asset filename
* method of creation
* date
* author designation: project-generated original asset
* external source: none

For externally licensed files, include:

* creator
* source
* license
* attribution requirement
* original URL

Do not leave provenance ambiguous.

---

# 52. AUDIO MIXER

Create proper audio channels:

* Master
* Music
* SFX
* UI

Settings must alter the appropriate AudioMixer groups.

Do not implement fake volume controls that only change UI values.

---

# 53. HAPTICS

Use restrained haptic feedback for:

* stamp impact
* dispatch
* successful delivery
* important error

Respect the Haptics setting.

Do not vibrate constantly.

---

# 54. GAME ICON

Create a completely original production icon.

Do not use the game title as the primary icon graphic.

Concept:

A tactile postal stamp pressing onto an envelope route symbol.

Visual composition:

* bold cream envelope
* distinctive red rubber-stamp impression
* directional route arrow
* warm dark postal desk background
* large readable shapes
* strong silhouette
* minimal small details

It must remain recognizable at small sizes.

---

# 55. APP ICON TECHNICAL RULES

Generate a high-quality master icon at:

`1024 x 1024`

No transparency in final App Store icon.

No transparent edges.

Do not rely on tiny text.

Avoid placing critical artwork too near the edges.

Integrate the icon properly into Unity/iOS Player Settings and generated Xcode asset configuration.

Inspect the icon at:

* 1024 px
* 180 px
* 120 px
* 60 px

Ensure it remains readable.

---

# 56. SPLASH / STARTUP

Use a minimal professional startup.

Avoid long logos.

Avoid fake publisher screens.

Avoid unnecessary loading delays.

Transition smoothly to Main Desk.

---

# 57. CODE ARCHITECTURE

Prefer clear modular systems.

Suggested folders:

```text
Assets/
├── Art/
├── Audio/
├── Data/
├── Documentation/
├── Materials/
├── Prefabs/
│   ├── Environment/
│   ├── Gameplay/
│   └── UI/
├── Scenes/
├── Scripts/
│   ├── Application/
│   ├── Audio/
│   ├── Gameplay/
│   ├── Levels/
│   ├── Persistence/
│   ├── UI/
│   └── Utilities/
├── Settings/
├── Shaders/
├── Tests/
└── VFX/
```

Keep classes focused.

Avoid giant god managers.

---

# 58. EVENT FLOW

Prefer controlled events/state transitions.

Example gameplay states:

* Loading
* Planning
* StampDragging
* Ready
* Dispatching
* Success
* Failure
* Paused

Prevent conflicting inputs between states.

---

# 59. INPUT

Use a mobile-safe input implementation.

Support:

* touch
* mouse in editor

Interactions must work without hover dependency.

Do not require keyboard input.

---

# 60. ACCESSIBILITY / USABILITY

At minimum:

* do not communicate crucial mechanics exclusively through color
* keep text readable
* maintain strong contrast
* maintain minimum touch target sizes
* avoid extremely rapid mandatory reactions
* provide music/SFX controls
* allow haptics to be disabled

---

# 61. PERFORMANCE TARGET

Target smooth mobile performance.

Aim for:

* stable 60 FPS on reasonable supported iPhones
* low garbage generation during gameplay
* no repeated Find calls every frame
* no expensive UI rebuild loops
* no unnecessary realtime lights
* no uncontrolled particle counts
* no heavy Update methods when event-driven behavior is possible

Profile the game.

---

# 62. NO NETWORK AUDIT

Before completion, inspect the project for:

* `UnityWebRequest`
* `HttpClient`
* sockets
* networking packages
* web views
* analytics
* cloud services
* advertising packages
* telemetry
* Firebase
* remote configuration
* external API calls

Unless Unity itself has editor-only functionality, production runtime should have none of these.

Document results.

---

# 63. PACKAGE AUDIT

Inspect Package Manager configuration.

Remove unnecessary packages.

Do not leave packages unrelated to the game.

Do not include:

* ad SDKs
* analytics SDKs
* multiplayer packages
* cloud packages
* remote config
* tracking frameworks

Keep only what the game genuinely needs.

---

# 64. iOS PLAYER SETTINGS

Configure production settings.

Product Name:

`Postmark Protocol`

Bundle Identifier:

`com.rehmanmughal.postmarkprotocol`

Version:

`1.0.0`

Build:

`1`

Scripting Backend:

`IL2CPP`

Architecture:

`ARM64`

Graphics API:

Use stable mobile-compatible configuration.

Configure:

* correct orientation
* icon
* splash behavior
* safe areas
* stripping level
* target iOS compatibility
* appropriate frame rate

Do not modify settings randomly.

---

# 65. PRIVACY

The build should require no personal data.

Do not request:

* location
* contacts
* microphone
* camera
* photos
* tracking
* Bluetooth
* health data

unless gameplay genuinely requires it.

For this game none of these should be necessary.

---

# 66. EDITOR TOOLS

Create useful internal Editor tooling where it improves quality.

Examples:

* level validator
* level browser
* route simulation checker
* save reset utility
* screenshot framing utility

Editor/debug tools must not be exposed in production UI.

Use proper compile guards when appropriate.

---

# 67. AUTOMATED TESTING

Create Unity tests covering important logic.

At minimum:

### Command tests

Verify:

* TurnLeft
* TurnRight
* Reverse
* Wait
* Fast
* Slow
* Jump

### Ink tests

Verify:

* standard
* double
* permanent

### Parcel tests

Verify restrictions.

### Save tests

Verify:

* new save
* write
* read
* upgrade/version
* reset

### Progression tests

Verify level unlocking.

### Route tests

Verify representative known solutions.

---

# 68. MANUAL QA MATRIX

Play through every level.

For each of the 36 levels test:

* normal completion
* incorrect command
* restart
* pause
* resume
* returning to menu
* replay
* earned seals
* save persistence

Record results in:

`Assets/Documentation/QA_REPORT.md`

---

# 69. UI QA

Inspect every screen visually.

Capture or inspect representative resolutions.

Audit:

* anchors
* alignment
* safe areas
* clipping
* readability
* button size
* panel spacing
* text spacing
* icon alignment

Fix every discovered layout issue.

---

# 70. AUDIO QA

Verify:

* Music setting
* SFX setting
* Haptics setting
* persistence
* mixer routing
* no clipping
* no excessively loud SFX
* no missing references
* sounds work after scene transitions

---

# 71. SAVE QA

Test:

1. start fresh
2. finish multiple levels
3. quit
4. relaunch
5. verify progress
6. change settings
7. relaunch
8. verify settings
9. reset progress
10. confirm reset
11. restart game
12. verify clean state

---

# 72. FINAL BUILD QA

Create a development build and inspect.

Then create a production-ready iOS build/export.

Fix:

* compiler errors
* missing references
* build errors
* broken materials
* missing shaders
* scene configuration
* icon configuration
* orientation problems

---

# 73. CONSOLE CLEANUP

Before final completion:

Unity Console must contain:

* zero errors caused by the game
* zero persistent exceptions
* no meaningful unresolved warnings from our code

Investigate warnings instead of ignoring them.

---

# 74. APP REVIEW CLEANUP AUDIT

Search the entire project for:

* hidden gameplay modes
* test menus
* debug buttons
* secret screens
* unused alternate game scenes
* placeholder features
* inaccessible UI
* misleading text
* fake shop buttons
* fake online functionality
* inaccessible settings
* dormant monetization code

Remove anything that should not ship.

---

# 75. COMPLETE FEATURE CONSISTENCY

The final build must accurately match what the game presents.

Do not show menu items for features that do not exist.

Do not describe functionality not present in the build.

Do not leave strings such as:

* Coming Soon
* Multiplayer
* Shop
* Login
* Online
* Daily Reward

unless actually implemented and required.

For this version, none of those should exist.

---

# 76. GAME TEXT POLISH

Review all player-facing English manually.

Text must feel natural and concise.

Examples:

Good:

`Dispatch the letter.`

`Route blocked.`

`Verification point missed.`

`Perfect Route`

Avoid awkward text such as:

`Click Button To Start Gameplay`

or:

`You Failed Due To Wrong Selecting`

---

# 77. CREDITS

Create a clean Credits section.

Include:

* Game title
* development credit
* original game audio statement
* software/framework attribution where legally required

Do not add unnecessary personal information.

---

# 78. DOCUMENTATION

Create:

```text
Assets/Documentation/
├── ARCHITECTURE.md
├── GAME_DESIGN.md
├── LEVEL_DESIGN.md
├── QA_REPORT.md
├── AUDIO_LICENSES.md
├── IOS_RELEASE_CHECKLIST.md
└── FINAL_AUDIT.md
```

---

# 79. ARCHITECTURE.md

Document:

* system architecture
* key classes
* dependency direction
* scene structure
* save architecture
* level architecture
* audio architecture
* UI architecture

---

# 80. GAME_DESIGN.md

Document the final implemented game, not an outdated original plan.

---

# 81. LEVEL_DESIGN.md

List all 36 levels with:

* objective
* mechanics
* parcel
* available stamps
* par
* intended learning goal
* intended solution summary

---

# 82. IOS_RELEASE_CHECKLIST.md

Record:

* bundle ID
* version
* build
* icon verified
* orientation verified
* privacy permissions
* package audit
* network audit
* test status
* build status

---

# 83. FINAL_AUDIT.md

At final completion record:

### Gameplay

PASS/FAIL

### 36 levels

PASS/FAIL

### Navigation

PASS/FAIL

### UI

PASS/FAIL

### Environment

PASS/FAIL

### Audio

PASS/FAIL

### Save system

PASS/FAIL

### Offline verification

PASS/FAIL

### Third-party SDK audit

PASS/FAIL

### Network audit

PASS/FAIL

### Icon

PASS/FAIL

### iOS settings

PASS/FAIL

### Performance

PASS/FAIL

### Tests

PASS/FAIL

### Console

PASS/FAIL

### Manual QA

PASS/FAIL

### Final build

PASS/FAIL

No final completion declaration is allowed if any required field remains FAIL.

---

# 84. VERSION CONTROL

If the project is already inside a Git repository:

Use frequent meaningful local commits.

Suggested checkpoints:

* MCP/bootstrap
* architecture
* gameplay core
* UI foundation
* environment
* chapter 1
* chapter 2
* chapter 3
* chapter 4
* chapter 5
* chapter 6
* audio
* iOS
* QA fixes
* final release audit

Use descriptive commit messages.

Do not push to a remote repository.

Do not create or initialize a new repository unless explicitly authorized.

Never commit temporary build folders.

---

# 85. DO NOT DESTROY WORK

Before major architecture modifications:

* inspect current implementation
* understand dependencies
* preserve working systems
* make controlled changes

Avoid destructive rewrites unless there is a strong reason.

---

# 86. ITERATIVE VISUAL INSPECTION

After completing each important screen:

1. enter Play Mode
2. navigate to screen
3. visually inspect
4. inspect hierarchy
5. inspect anchors
6. inspect text
7. inspect aspect ratio
8. fix issues
9. inspect again

Do not wait until the end to discover layout problems.

---

# 87. ITERATIVE GAMEPLAY INSPECTION

For every new mechanic:

1. implement
2. create test scenario
3. run
4. inspect behavior
5. test failure
6. test success
7. test restart
8. inspect console
9. fix
10. integrate into levels

---

# 88. COMPLETION GATE

The game is COMPLETE only when all of the following are true:

* [ ] AnkleBreaker MCP configured
* [ ] MCP connection verified
* [ ] MEMORY_BANK.md created and current
* [ ] AI_RULES.md created
* [ ] fresh project architecture established
* [ ] core stamp mechanic complete
* [ ] all 7 command stamps functional
* [ ] all 3 ink rules functional
* [ ] all 4 parcel types functional
* [ ] 36 levels implemented
* [ ] 36 levels manually tested
* [ ] all levels solvable
* [ ] tutorial complete
* [ ] Main Desk complete
* [ ] level ledger complete
* [ ] handbook complete
* [ ] gameplay HUD complete
* [ ] pause complete
* [ ] settings complete
* [ ] credits complete
* [ ] local save system complete
* [ ] environment polished
* [ ] manually authored scene hierarchy verified
* [ ] UI manually authored in hierarchy verified
* [ ] typography polished
* [ ] responsive layout verified
* [ ] safe areas verified
* [ ] animations polished
* [ ] VFX polished
* [ ] original/license-safe SFX complete
* [ ] audio mixer complete
* [ ] music/SFX controls functional
* [ ] haptics functional
* [ ] game icon complete
* [ ] icon integrated
* [ ] bundle identifier configured
* [ ] iOS Player Settings configured
* [ ] offline operation verified
* [ ] network audit passed
* [ ] third-party SDK audit passed
* [ ] package audit passed
* [ ] automated tests passed
* [ ] manual QA passed
* [ ] performance checked
* [ ] Unity Console clean
* [ ] production debug tools removed
* [ ] App Review cleanup audit passed
* [ ] iOS build/export succeeds
* [ ] documentation complete
* [ ] FINAL_AUDIT.md contains only PASS for required categories

---

# 89. AUTONOMOUS FIX LOOP

Whenever an acceptance criterion fails:

1. identify root cause
2. update memory bank if necessary
3. implement fix
4. compile
5. test
6. inspect visually if applicable
7. rerun relevant tests
8. update documentation
9. repeat until PASS

Do not merely document a bug and continue to final completion.

Fix it.

---

# 90. WHEN SOMETHING IS AMBIGUOUS

Prefer the solution that is:

1. simplest for the player
2. easiest to understand
3. visually coherent with the postal-desk theme
4. technically maintainable
5. mobile performant
6. offline
7. free of unnecessary dependencies
8. appropriate for App Store distribution

Do not interrupt development for minor aesthetic decisions that can be resolved professionally.

---

# 91. DEFINITION OF "COMPLETE"

A project containing scripts and a working first level is not complete.

A game containing a menu and several sample levels is not complete.

A game with placeholder art is not complete.

A game with broken settings is not complete.

A game with 30 of 36 levels is not complete.

A game that only works in the Unity Editor is not complete.

A game with unresolved iOS build errors is not complete.

A game with an unfinished icon is not complete.

A game with failing tests is not complete.

A game with major visual alignment issues is not complete.

A game with unresolved production exceptions is not complete.

Complete means a player can install the build, launch it, learn it, play through all 36 levels, earn progress, quit, return, continue playing, adjust settings, finish the game, and encounter a coherent polished experience without requiring internet access.

---

# 92. BEGIN NOW

Start with:

1. inspect repository/project
2. inspect Unity version
3. configure AnkleBreaker MCP
4. verify the MCP connection
5. create `AI_RULES.md`
6. create `MEMORY_BANK.md`
7. establish folder architecture
8. configure base project settings
9. create the manually authored Bootstrap/MainDesk/Gameplay scene foundations
10. begin parallel specialist development

Continue through implementation, integration, QA, fixes, iOS configuration, and final release audit.

Do not stop at planning.

Do not provide only instructions for how the user could build the game.

Actually modify the Unity project through the available development tools.

Do not declare success based on assumptions.

Verify everything.

Your final response is allowed only after either:

### A. FULL SUCCESS

Every required completion gate passes.

Provide a concise final report covering:

* implemented systems
* number of levels
* QA result
* build result
* bundle identifier
* icon status
* offline/network audit
* remaining known issues, which should be `None`

or:

### B. A TRUE EXTERNAL BLOCKER

An external blocker means something genuinely impossible to resolve from within the available local tools, such as missing OS-level signing credentials.

In that case:

* complete every task that does not depend on the blocker
* document the blocker precisely
* do not misrepresent the game as fully complete
* provide exact evidence of what remains blocked

Minor implementation bugs, layout issues, compiler errors, missing references, unfinished levels, incomplete assets, test failures, or fixable Unity issues are NOT external blockers.

They must be fixed before stopping.

# BUILD POSTMARK PROTOCOL.
