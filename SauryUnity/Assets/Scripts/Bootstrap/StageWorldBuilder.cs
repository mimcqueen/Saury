using Saury.Baby;
using Saury.CameraRig;
using Saury.Core;
using Saury.Dad;
using Saury.Host;
using Saury.Minigames;
using Saury.UI;
using Saury.VFX;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Bootstrap
{
    /// <summary>Procedural Maury hub stage — opens without hand-authored scene content.</summary>
    public static class StageWorldBuilder
    {
        public static GameFlowController Build()
        {
            var root = new GameObject("SauryWorld").transform;

            // Lighting — dingy stage
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.08f, 0.07f, 0.09f);
            var sun = new GameObject("KeySpot");
            sun.transform.SetParent(root, false);
            sun.transform.position = new Vector3(1.5f, 5.5f, -2f);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var key = sun.AddComponent<Light>();
            key.type = LightType.Spot;
            key.range = 20f;
            key.spotAngle = 55f;
            key.intensity = 3.2f;
            key.color = new Color(1f, 0.92f, 0.75f);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(root, false);
            fill.transform.position = new Vector3(-3f, 3f, 2f);
            var fillL = fill.AddComponent<Light>();
            fillL.type = LightType.Point;
            fillL.range = 12f;
            fillL.intensity = 0.6f;
            fillL.color = new Color(0.45f, 0.5f, 0.7f);

            // Floor + curtains
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "StageFloor";
            floor.transform.SetParent(root, false);
            floor.transform.localScale = new Vector3(16f, 0.2f, 12f);
            floor.transform.position = new Vector3(0f, -0.1f, 0f);
            PrimitiveMaterials.Apply(floor, new Color(0.12f, 0.1f, 0.11f));

            var back = GameObject.CreatePrimitive(PrimitiveType.Cube);
            back.name = "NicotineCurtain";
            back.transform.SetParent(root, false);
            back.transform.position = new Vector3(0f, 2.5f, 5.5f);
            back.transform.localScale = new Vector3(14f, 5f, 0.2f);
            PrimitiveMaterials.Apply(back, new Color(0.35f, 0.22f, 0.14f));

            // Host desk + figure
            var desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "HostDesk";
            desk.transform.SetParent(root, false);
            desk.transform.position = new Vector3(0f, 0.55f, 2.2f);
            desk.transform.localScale = new Vector3(2.4f, 1.1f, 1f);
            PrimitiveMaterials.Apply(desk, new Color(0.25f, 0.18f, 0.12f));

            var hostGo = new GameObject("CPU_Host");
            hostGo.transform.SetParent(root, false);
            hostGo.transform.position = new Vector3(0f, 1.55f, 2.5f);
            var torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.transform.SetParent(hostGo.transform, false);
            torso.transform.localScale = new Vector3(0.7f, 0.7f, 0.5f);
            PrimitiveMaterials.Apply(torso, new Color(0.55f, 0.2f, 0.22f));
            Object.Destroy(torso.GetComponent<Collider>());

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(hostGo.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            head.transform.localScale = Vector3.one * 0.55f;
            PrimitiveMaterials.Apply(head, new Color(0.85f, 0.72f, 0.62f));
            Object.Destroy(head.GetComponent<Collider>());

            var mouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mouth.name = "Mouth";
            mouth.transform.SetParent(head.transform, false);
            mouth.transform.localPosition = new Vector3(0f, -0.15f, 0.28f);
            mouth.transform.localScale = new Vector3(0.35f, 0.12f, 0.1f);
            PrimitiveMaterials.Apply(mouth, new Color(0.4f, 0.1f, 0.12f));
            Object.Destroy(mouth.GetComponent<Collider>());

            // Audience eyes
            var eyesRoot = new GameObject("AudienceEyes").transform;
            eyesRoot.SetParent(root, false);
            var eyes = eyesRoot.gameObject.AddComponent<AudienceEyes>();
            eyes.Build(eyesRoot, 18);

            // Marks / dad stools
            for (int i = 0; i < 4; i++)
            {
                var stool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                stool.name = $"DadMark_{i}";
                stool.transform.SetParent(root, false);
                stool.transform.position = new Vector3(-2.4f + i * 1.6f, 0.35f, -1.2f);
                stool.transform.localScale = new Vector3(0.6f, 0.35f, 0.6f);
                PrimitiveMaterials.Apply(stool, new Color(0.2f, 0.2f, 0.22f));
            }

            // Camera + mounts
            var camGo = new GameObject("MainShowCamera");
            camGo.transform.SetParent(root, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.02f, 0.03f);
            cam.fieldOfView = 50f;
            camGo.tag = "MainCamera";
            camGo.AddComponent<AudioListener>();

            var director = camGo.AddComponent<StageCameraDirector>();
            director.mainCam = cam;
            director.lobbyOrbit = MakeMount(root, "Cam_LobbyOrbit", new Vector3(4.2f, 2.1f, -4.2f), new Vector3(0f, 1.2f, 0.5f));
            director.hostPush = MakeMount(root, "Cam_HostPush", new Vector3(0.3f, 1.7f, 0.4f), new Vector3(0f, 1.6f, 2.5f));
            director.miniWide = MakeMount(root, "Cam_MiniWide", new Vector3(0f, 4.5f, -8f), new Vector3(0f, 0.5f, 0f));
            director.podium = MakeMount(root, "Cam_Podium", new Vector3(0f, 1.8f, -3.5f), new Vector3(0f, 1.2f, 0f));
            director.babyLane = MakeMount(root, "Cam_BabyLane", new Vector3(0f, 2.2f, 2.5f), new Vector3(0f, 1.2f, 6.5f));
            director.eyesCut = MakeMount(root, "Cam_Eyes", new Vector3(0f, 1.4f, -3.8f), new Vector3(0f, 1.2f, -5.5f));
            director.CutTo(director.lobbyOrbit);

            // UI canvas + VHS
            var canvasGo = new GameObject("ShowCanvas");
            canvasGo.transform.SetParent(root, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            var vhs = BuildVhs(canvasGo.transform);
            var hud = ShowHud.Create(canvasGo.transform);

            var hostPresenter = hostGo.AddComponent<HostPresenter>();
            hostPresenter.hostRoot = hostGo.transform;
            hostPresenter.mouth = mouth.transform;
            hostPresenter.cardText = hud.hostCard;
            hostPresenter.subtitleText = hud.hostSubtitle;

            var flowGo = new GameObject("GameFlow");
            flowGo.transform.SetParent(root, false);
            var flow = flowGo.AddComponent<GameFlowController>();
            flow.worldRoot = root;
            flow.host = hostPresenter;
            flow.cams = director;
            flow.hud = hud;
            flow.makeDad = flowGo.AddComponent<MakeDadController>();
            flow.babyGen = flowGo.AddComponent<MockBabyGenerator>();
            flow.showcase = flowGo.AddComponent<BabyShowcaseDirector>();
            flow.showcase.EnsureLane(root);
            flow.showcase.titleText = hud.babyTitle;
            flow.showcase.roastText = hud.babyRoast;
            flow.showcase.promptText = hud.babyPrompt;
            flow.pissing = flowGo.AddComponent<PissingContestMinigame>();
            flow.bbq = flowGo.AddComponent<BbqBrawlMinigame>();

            // Hide unused warning
            _ = vhs;

            return flow;
        }

        static Transform MakeMount(Transform parent, string name, Vector3 pos, Vector3 lookAt)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.position = pos;
            t.LookAt(lookAt);
            return t;
        }

        static VhsOverlay BuildVhs(Transform canvas)
        {
            var frame = new GameObject("VHSFrame");
            frame.transform.SetParent(canvas, false);
            var frameImg = frame.AddComponent<Image>();
            frameImg.color = new Color(0.05f, 0.05f, 0.06f, 0.35f);
            var frt = frame.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(18, 18);
            frt.offsetMax = new Vector2(-18, -18);

            var scan = new GameObject("Scanlines");
            scan.transform.SetParent(canvas, false);
            var scanImg = scan.AddComponent<Image>();
            scanImg.color = new Color(0f, 0f, 0f, 0.15f);
            var srt = scan.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = Vector2.zero;
            srt.offsetMax = Vector2.zero;

            var noise = new GameObject("Noise");
            noise.transform.SetParent(canvas, false);
            var raw = noise.AddComponent<RawImage>();
            raw.color = new Color(1f, 1f, 1f, 0.08f);
            var nrt = noise.GetComponent<RectTransform>();
            nrt.anchorMin = Vector2.zero;
            nrt.anchorMax = Vector2.one;
            nrt.offsetMin = Vector2.zero;
            nrt.offsetMax = Vector2.zero;

            var overlay = canvas.gameObject.AddComponent<VhsOverlay>();
            overlay.noise = raw;
            overlay.scanlines = scanImg;
            overlay.frame = frameImg;
            // Keep VHS behind readable text: move to first sibling
            frame.transform.SetAsFirstSibling();
            scan.transform.SetAsFirstSibling();
            noise.transform.SetAsFirstSibling();
            return overlay;
        }
    }
}
