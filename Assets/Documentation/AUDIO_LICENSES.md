# Audio provenance

All audio below was composed and synthesized locally on 2026-08-26 for Postmark Protocol.
Author designation: **project-generated original asset**. External source: **none**.
No third-party recordings, sample libraries, downloaded audio, or licensed melodies are used.
The project owner may use and distribute these original generated assets with the game.

The reproducible source is `Assets/Editor/AudioAuthoring.cs`, `BuildAssets()`.
All master files are mono PCM WAV, 44,100 Hz, 16 bit. Sound effects import as PCM;
the 24-second ambient composition imports as streaming Vorbis.

| Asset filename | Method of creation |
| --- | --- |
| pickup.wav | Decaying wooden resonance and seeded filtered noise |
| stamp.wav | Low rubber/wood impact resonance and short noise transient |
| remove.wav | Filtered noise with paper envelope |
| paper.wav | Modulated filtered noise with smooth paper envelope |
| dispatch.wav | Mechanical two-resonance click and noise transient |
| slide.wav | Quiet modulated filtered noise |
| turn.wav | Short resonant tick with seeded noise |
| wait.wav | Soft decaying clock resonance |
| reverse.wav | Smooth descending oscillator |
| jump.wav | Smooth ascending oscillator |
| mailbox.wav | Mechanical body and metal resonance |
| success.wav | Original ascending three-note bell phrase |
| failure.wav | Original descending two-note bell phrase |
| seal.wav | Restrained rubber stamp resonance |
| ui.wav | Soft short resonance and seeded noise |
| pause.wav | Soft short resonance and seeded noise |
| button.wav | Soft short resonance and seeded noise |
| page.wav | Modulated filtered paper noise |
| after_hours.wav | Original 32-beat plucked motif, C6 / A minor / Fmaj7 / Gsus2, 80 BPM |

## Routing and settings

`PostalDesk.mixer` has Master, Music, SFX and UI groups. `MasterVolume`, `MusicVolume`,
`SFXVolume` and `UIVolume` are actual exposed mixer volume parameters. The Music setting
controls Music; Sound Effects controls both SFX and UI. Zero maps to -80 dB.
The Bootstrap-authored AudioService and its three authored AudioSources persist between scenes.
Haptics uses an original UIKit bridge to Apple's light impact generator on iOS 13+,
with intensity 0.45; Android uses Unity's native vibration. It is throttled and obeys
the saved toggle. Devices without a haptic engine simply do not emit an impact.

## Verification

`AudioAuthoring.Validate()` checks every WAV for nonzero content and ample peak headroom,
imports, mixer group presence, and exposed parameters. Desktop audio playback can be verified
in Play Mode. iPhone speaker, silent-switch behavior and physical vibration require device QA;
Editor validation does not claim those hardware checks.
