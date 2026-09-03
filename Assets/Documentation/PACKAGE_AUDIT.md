# Package audit

Updated: 2026-09-03. Scope: direct Unity Package Manager dependencies and current project/tooling references.

## Result

The project now has **9 direct dependencies**, reduced from the original 53. Unity Package Manager resolved the manifest and lockfile. AnkleBreaker remains operational and Unity reports no current compilation errors or warnings.

### Retained direct dependencies

| Package | Current use |
|---|---|
| `com.anklebreaker.unity-mcp` | Required Editor integration; its assemblies are Editor-only |
| `com.unity.inputsystem` | Authored EventSystem and touch/mouse input |
| `com.unity.ugui` | Production UI, controls and EventSystem components |
| `com.unity.modules.audio` | AudioSource, AudioClip and AudioMixer runtime |
| `com.unity.modules.jsonserialize` | Versioned local save serialization |
| `com.unity.modules.physics` | Authored BoxCollider and PhysicsRaycaster components |
| `com.unity.modules.imageconversion` | Icon and screenshot PNG processing tools |
| `com.unity.modules.screencapture` | Editor visual-capture tooling |
| `com.unity.test-framework` | Editor-only automated test assemblies |

The resolved lockfile also contains five required transitive dependencies: NUnit, IMGUI, UI, UIElements and HierarchyCore. They are not direct manifest entries.

## Removed direct dependencies

The original cleanup removed 18 template or unused feature packages: multiplayer center, collaboration proxy, visual scripting, seven unused 2D authoring packages, Timeline, Analytics, four specialized web-request modules, Video and Universal Render Pipeline. The unused URP/sample assets were also removed after dependency inspection.

The 2026-09-03 cleanup removed these remaining unnecessary direct roots:

- IDE integrations: Rider and Visual Studio.
- Unused engine features: Accessibility module API, Adaptive Performance, AI/NavMesh, Android JNI, Animation module API, AssetBundle, Cloth, Director, Particle System, Physics 2D, Terrain, Terrain Physics, Tilemap, Umbra, Vector Graphics, Vehicles, VR, Wind and XR.
- Unused networking modules: UnityWebRequest and UnityWebRequestTexture.
- Redundant direct roots: IMGUI, UI and UIElements; Unity keeps these only as transitive dependencies of retained packages.

No production asset or scene was removed in this package pass. The retained package set follows current source, assembly, prefab and scene references; it does not keep speculative modules for possible future features.

## Current dependency boundary

`Packages/manifest.json` and `Packages/packages-lock.json` are the canonical dependency records. The manifest contains no advertising, analytics, multiplayer, cloud, remote-configuration or runtime-network package. Game runtime source contains no network API call site. AnkleBreaker remains development tooling only.
