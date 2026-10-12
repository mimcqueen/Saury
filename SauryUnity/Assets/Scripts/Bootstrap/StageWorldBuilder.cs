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
    /// <summary>Procedural Maury hub — real 3D set + character kits.</summary>
    public static class StageWorldBuilder
    {
        public static GameFlowController Build()
        {
            var root = new GameObject("SauryWorld").transform;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.05f, 0.045f, 0.055f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.03f);
            RenderSettings.fogDensity = 0.035f;

            StageKit.BuildHubSet(root);

            // CPU Host — modeled figure at desk
            var hostBuild = CharacterKit.BuildHost(root, new Vector3(0f, 0.95f, 2.35f));

            // Audience eyes (3D pairs)
            var eyesRoot = new GameObject("AudienceEyes").transform;
            eyesRoot.SetParent(root, false);
            eyesRoot.gameObject.AddComponent<AudienceEyes>().Build(eyesRoot, 16);

            // Hub dad dolls on stools (readable creature blockouts)
            for (int i = 0; i < 4; i++)
            {
                var creature = (CreatureId)(i % 6);
                var dad = CharacterKit.BuildDad(
                    root,
                    new Vector3(-2.6f + i * 1.7f, 0.45f, -1.4f),
                    creature,
                    name: $"HubDad_{creature}");
                dad.root.transform.localScale = Vector3.one * 0.85f;
                // Hub dolls are scenic — freeze physics
                var rb = dad.root.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Camera
            var camGo = new GameObject("MainShowCamera");
            camGo.transform.SetParent(root, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.015f, 0.012f, 0.02f);
            cam.fieldOfView = 48f;
            cam.nearClipPlane = 0.05f;
            camGo.tag = "MainCamera";
            camGo.AddComponent<AudioListener>();

            var director = camGo.AddComponent<StageCameraDirector>();
            director.mainCam = cam;
            director.lobbyOrbit = MakeMount(root, "Cam_LobbyOrbit", new Vector3(4.5f, 2.2f, -4.0f), new Vector3(0f, 1.3f, 1.0f));
            director.hostPush = MakeMount(root, "Cam_HostPush", new Vector3(0.45f, 1.85f, 0.55f), new Vector3(0f, 1.7f, 2.5f));
            director.miniWide = MakeMount(root, "Cam_MiniWide", new Vector3(0f, 5.0f, -9f), new Vector3(0f, 0.6f, 0f));
            director.podium = MakeMount(root, "Cam_Podium", new Vector3(0f, 1.9f, -3.2f), new Vector3(0f, 1.3f, 0f));
            director.babyLane = MakeMount(root, "Cam_BabyLane", new Vector3(0.2f, 2.0f, 2.8f), new Vector3(0f, 1.1f, 6.8f));
            director.eyesCut = MakeMount(root, "Cam_Eyes", new Vector3(0f, 1.5f, -4.2f), new Vector3(0f, 1.3f, -6.0f));
            director.CutTo(director.lobbyOrbit);

            // UI — keep thin so 3D reads first
            var canvasGo = new GameObject("ShowCanvas");
            canvasGo.transform.SetParent(root, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            BuildVhs(canvasGo.transform);
            var hud = ShowHud.Create(canvasGo.transform);

            var hostPresenter = hostBuild.root.AddComponent<HostPresenter>();
            hostPresenter.hostRoot = hostBuild.root.transform;
            hostPresenter.mouth = hostBuild.mouth;
            hostPresenter.head = hostBuild.head;
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
            frameImg.color = new Color(0.05f, 0.05f, 0.06f, 0.22f);
            var frt = frame.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(22, 22);
            frt.offsetMax = new Vector2(-22, -22);

            var scan = new GameObject("Scanlines");
            scan.transform.SetParent(canvas, false);
            var scanImg = scan.AddComponent<Image>();
            scanImg.color = new Color(0f, 0f, 0f, 0.1f);
            var srt = scan.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = Vector2.zero;
            srt.offsetMax = Vector2.zero;

            var noise = new GameObject("Noise");
            noise.transform.SetParent(canvas, false);
            var raw = noise.AddComponent<RawImage>();
            raw.color = new Color(1f, 1f, 1f, 0.05f);
            var nrt = noise.GetComponent<RectTransform>();
            nrt.anchorMin = Vector2.zero;
            nrt.anchorMax = Vector2.one;
            nrt.offsetMin = Vector2.zero;
            nrt.offsetMax = Vector2.zero;

            var overlay = canvas.gameObject.AddComponent<VhsOverlay>();
            overlay.noise = raw;
            overlay.scanlines = scanImg;
            overlay.frame = frameImg;
            frame.transform.SetAsFirstSibling();
            scan.transform.SetAsFirstSibling();
            noise.transform.SetAsFirstSibling();
            return overlay;
        }
    }
}
