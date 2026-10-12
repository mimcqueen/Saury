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

            // Spotlights crush the world — almost no fill
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.012f, 0.01f, 0.014f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.01f, 0.008f, 0.012f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.045f;

            StageKit.BuildHubSet(root);

            // CPU Host — sleazy talk-show doll seated clearly behind desk
            var hostBuild = CharacterKit.BuildHost(root, new Vector3(0f, 0.55f, 2.85f));

            // Audience eyes — many pairs in the dark
            var eyesRoot = new GameObject("AudienceEyes").transform;
            eyesRoot.SetParent(root, false);
            eyesRoot.gameObject.AddComponent<AudienceEyes>().Build(eyesRoot, 28);

            // Hub dad dolls on stools
            for (int i = 0; i < 4; i++)
            {
                var creature = (CreatureId)(i % 6);
                var dad = CharacterKit.BuildDad(
                    root,
                    new Vector3(-2.7f + i * 1.8f, 0.48f, -1.5f),
                    creature,
                    name: $"HubDad_{creature}");
                dad.root.transform.localScale = Vector3.one * 0.88f;
                var rb = dad.root.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Camera — Liar's Bar intimacy
            var camGo = new GameObject("MainShowCamera");
            camGo.transform.SetParent(root, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.008f, 0.006f, 0.01f);
            cam.fieldOfView = 42f;
            cam.nearClipPlane = 0.05f;
            cam.allowHDR = true;
            camGo.tag = "MainCamera";
            camGo.AddComponent<AudioListener>();

            var director = camGo.AddComponent<StageCameraDirector>();
            director.mainCam = cam;
            director.lobbyOrbit = MakeMount(root, "Cam_LobbyOrbit", new Vector3(3.8f, 1.85f, -3.4f), new Vector3(0f, 1.35f, 1.2f));
            // Hero Host desk push-in (sells sleazy doll + mic + desk in one glance)
            director.hostPush = MakeMount(root, "Cam_HostPush", new Vector3(1.8f, 1.85f, -0.2f), new Vector3(0f, 1.65f, 2.75f));
            director.miniWide = MakeMount(root, "Cam_MiniWide", new Vector3(0f, 4.8f, -9f), new Vector3(0f, 0.6f, 0f));
            director.podium = MakeMount(root, "Cam_Podium", new Vector3(0f, 1.75f, -3.0f), new Vector3(0f, 1.25f, 0f));
            // Hero baby lane — 3/4 plinth + lane pomp
            director.babyLane = MakeMount(root, "Cam_BabyLane", new Vector3(2.6f, 2.15f, 7.4f), new Vector3(0f, 1.45f, 9.6f));
            director.eyesCut = MakeMount(root, "Cam_Eyes", new Vector3(0f, 1.35f, -4.0f), new Vector3(0f, 1.2f, -6.2f));
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
            frameImg.color = new Color(0.04f, 0.03f, 0.04f, 0.28f);
            var frt = frame.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(22, 22);
            frt.offsetMax = new Vector2(-22, -22);

            var scan = new GameObject("Scanlines");
            scan.transform.SetParent(canvas, false);
            var scanImg = scan.AddComponent<Image>();
            scanImg.color = new Color(0f, 0f, 0f, 0.14f);
            var srt = scan.GetComponent<RectTransform>();
            srt.anchorMin = Vector2.zero;
            srt.anchorMax = Vector2.one;
            srt.offsetMin = Vector2.zero;
            srt.offsetMax = Vector2.zero;

            var noise = new GameObject("Noise");
            noise.transform.SetParent(canvas, false);
            var raw = noise.AddComponent<RawImage>();
            raw.color = new Color(1f, 1f, 1f, 0.07f);
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
