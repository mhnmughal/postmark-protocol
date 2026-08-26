# Package audit

Date: 2026-08-26. Scope: direct package cleanup and source/dependency review during integration. No compiled-player exclusion claim is made here.

## Completed removal

The following 18 unused direct dependencies were removed with Unity Package Manager `Client.AddAndRemove` executed through AnkleBreaker MCP. Each is absent from both `Packages/manifest.json` and `Packages/packages-lock.json` after package resolution. The final manifest contains 35 direct packages.

| Removed package | Reason |
|---|---|
| com.unity.multiplayer.center | Offline single-player game |
| com.unity.collab-proxy | Unused version-control integration |
| com.unity.visualscripting | Game uses authored C# |
| com.unity.2d.animation | No skeletal 2D animation |
| com.unity.2d.aseprite | No Aseprite source assets |
| com.unity.2d.psdimporter | No PSD source assets |
| com.unity.2d.spriteshape | No SpriteShape content |
| com.unity.2d.tilemap.extras | No extra tilemap tools used |
| com.unity.2d.tooling | Unused template 2D tool collection |
| com.unity.timeline | No Timeline sequences |
| com.unity.modules.unityanalytics | No analytics |
| com.unity.modules.unitywebrequestassetbundle | No remotely downloaded bundles |
| com.unity.modules.unitywebrequestaudio | Audio assets are local |
| com.unity.modules.unitywebrequestwww | No legacy WWW use |
| com.unity.modules.video | No video player; also removes its web-request dependency |
| com.unity.render-pipelines.universal | Production scenes use built-in forward rendering and Standard shader |
| com.unity.2d.sprite | Unused Sprite Editor package |
| com.unity.2d.tilemap | No tilemap-authored production scene |

## Retained dependencies

- Input System and UGUI: referenced by `Assets/Scripts/PostmarkProtocol.Runtime.asmdef` and used for touch/mouse/UI.
- Test Framework: Editor verification; the service test assembly has `includePlatforms: ["Editor"]` and `optionalUnityReferences: ["TestAssemblies"]`.
- AnkleBreaker Unity MCP 2.39.5, Git commit `9032874ff0b83a5941a01b2ab99599fcc61e90db`: required development tooling, not a player assembly.
- Core `unitywebrequest` and `unitywebrequesttexture` modules: retained because AnkleBreaker Editor news/update/devlog source directly compiles against `UnityWebRequest` and `UnityWebRequestTexture`. Removing them would break the required development tooling. See NETWORK_AUDIT.md for exact source boundaries.
- Engine modules supporting audio, UI, physics, image conversion, JSON persistence, rendering, and Editor tools remain.

## Template assets removed safely

After the gameplay agent's 62-test window ended, `AssetDatabase.GetDependencies` confirmed Bootstrap, MainDesk, and Gameplay referenced none of the eight template assets below. GUID searches identified only inter-template references, the old URP global-settings registration, and the default input-actions EditorBuildSettings config entry. The latter two were cleared using Unity APIs before deletion.

Moved to OS trash with `unity_asset_delete`, `permanent=false`:

- Assets/DefaultVolumeProfile.asset
- Assets/UniversalRenderPipelineGlobalSettings.asset
- Assets/Settings/UniversalRP.asset
- Assets/Settings/Renderer2D.asset
- Assets/Settings/Scenes/URP2DSceneTemplate.unity
- Assets/Settings/Lit2DSceneTemplate.scenetemplate
- Assets/Scenes/SampleScene.unity
- Assets/InputSystem_Actions.inputactions

No production scene was deleted or edited by this cleanup. A subsequent GUID scan found no remaining references in Assets or ProjectSettings. URP, render-pipelines.core, Shader Graph, and both Sprite/Tilemap editor packages are absent from the resolved lockfile.

## Remaining module rationale

| Module group | Reason retained / audit boundary |
|---|---|
| audio, jsonserialize, ui, imgui, uielements, physics | Game audio/persistence/UI and explicit UGUI, Input System, Test Framework, or UIElements dependencies |
| imageconversion, screencapture | Icon/capture and visual-QA workflow; texture web-request dependency |
| animation, ai, terrain, particlesystem | AnkleBreaker Editor code directly compiles against Animator/AnimationClip, NavMesh, Terrain, and ParticleSystem APIs |
| unitywebrequest, unitywebrequesttexture | AnkleBreaker Editor update/news/devlog compile dependencies; no game call sites |
| physics2d, tilemap, terrainphysics, cloth | Built-in engine capability/dependency set retained; Tilemap depends on Physics2D, TerrainPhysics on terrain/physics, Cloth on physics. MCP contains optional UMA cloth tooling. These are not claimed as gameplay features |
| accessibility, adaptiveperformance, androidjni, assetbundle, director, umbra, vectorgraphics, vehicles, vr, wind, xr | No game or MCP call site found by targeted source scan. Retained pending a full engine/package platform-dependency and stripped-player audit; do not claim each is necessary for runtime. Removing platform modules without that check could break editor or mobile package compilation |
| Rider / Visual Studio integration | Developer-only code-editor workflows; not a player feature |

No additional module was removed merely because its name looked unrelated. The final stripped-player dependency report remains the evidence needed to distinguish available engine capabilities from shipped code.

## Verification result

Package Manager resolved the removals and AnkleBreaker remained operational. After final URP/template removal, `unity_get_compilation_errors(severity="all")` returned count 0, isCompiling false: zero errors and warnings. The earlier transient SceneUI/RoutePresentation integration error was no longer present. Re-run the final player build after all remaining game changes.

No ad SDK, Firebase, remote configuration, cloud service, or third-party tracking package was present in the reviewed manifest. This is a source/package audit, not an App Store submission or device-network test.
