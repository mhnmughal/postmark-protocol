# Services verification

Executed inside Unity 6000.3.19f1 through AnkleBreaker MCP on 2026-08-26.

## NUnit Edit Mode

Assembly: `PostmarkProtocol.Services.Tests`.
Job: `fd926004e2ef`; **15 passed / 0 failed / 0 skipped**, duration 0.85 seconds.

Checks cover new-save defaults, read/write/settings/tutorial persistence, version-one
migration, future-version preservation, checksum tampering, corrupt-primary recovery,
unavailable storage, missing/corrupt data, reset including backup data, all 36 unlocks,
seal criteria, replay best records, invalid IDs/data, and audio decibel mapping.

The first run found corruption exceptions were not included in the recovery catch.
The catch was corrected and all tests rerun successfully. No remaining service test failure.

## Audio assets

`AudioAuthoring.BuildAssets()` and `Validate()` executed through MCP:
18 original effect WAVs and one original ambient composition imported successfully.
All 19 WAVs have nonzero samples and peak amplitude below 25,000/32,767.
Mixer groups Master / Music / SFX / UI and all four exposed volume parameters verified.

`AudioAuthoring.ValidatePlayback()` provides a separate Play Mode integration check
for persistent loop playback, source routing, clip references, SFX/UI playback, and
real mute / half-volume parameter changes. Run after Bootstrap scene integration.

## Physical-device boundary

The original iOS native plugin calls UIKit light feedback at intensity 0.45, throttled
by the runtime and governed by the saved toggle. A Windows Editor cannot confirm
iPhone vibration, speaker output, silent-switch behavior, or iOS native linking.
Those checks must be completed on the final Xcode export and physical device.
