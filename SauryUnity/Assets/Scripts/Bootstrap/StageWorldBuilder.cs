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
    /// <summary>Procedural Maury hub — Buckshot-table intimacy + character kits.</summary>
    public static class StageWorldBuilder
    {
        public static GameFlowController Build()
        {
            var root = new GameObject("SauryWorld").transform;

            // Crushing dark — Buckshot practicals only (ref 05)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.006f, 0.005f, 0.008f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.004f, 0.003f, 0.005f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.065f;

            StageKit.BuildHubSet(root);

            var hostBuild = CharacterKit.BuildHost(root, new Vector3(0f, 0.55f, 2.85f));

            var eyesRoot = new GameObject("AudienceEyes").transform;
            eyesRoot.SetParent(root, false);
            eyesRoot.gameObject.AddComponent<AudienceEyes>().Build(eyesRoot, 28);

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

            var camGo = new GameObject("MainShowCamera");
            camGo.transform.SetParent(root, false);
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.004f, 0.003f, 0.006f);
            cam.fieldOfView = 38f;
            cam.nearClipPlane = 0.05f;
            cam.allowHDR = true;
            camGo.tag = "MainCamera";
            camGo.AddComponent<AudioListener>();

            var director = camGo.AddComponent<StageCameraDirector>();
            director.mainCam = cam;

            // Intimate lobby orbit seed (TickLobbyOrbit rewrites each frame)
            director.lobbyOrbit = CamMount.Create(root, "Cam_LobbyOrbit",
                new Vector3(2.8f, 1.55f, -1.8f), new Vector3(0f, 1.45f, 2.35f), 38f).transform;

            // Buckshot desk push — subject lit, crushed blacks, Host readable (ref 05)
            director.hostPush = CamMount.Create(root, "Cam_HostPush",
                new Vector3(1.15f, 1.55f, 0.35f), new Vector3(0f, 1.55f, 2.85f), 36f).transform;

            // Make Dad: across-the-table intimacy on Host + marks
            director.makeDadTable = CamMount.Create(root, "Cam_MakeDadTable",
                new Vector3(0.15f, 1.35f, -2.1f), new Vector3(0f, 1.4f, 2.4f), 40f).transform;

            // Mini high-angle placeholder — AimMiniHighAngle relocates per arena
            director.miniWide = CamMount.Create(root, "Cam_MiniWide",
                new Vector3(0f, 9.2f, -7f), new Vector3(0f, 0.35f, 0.4f), 48f).transform;

            director.podium = CamMount.Create(root, "Cam_Podium",
                new Vector3(0.4f, 1.65f, -2.4f), new Vector3(0f, 1.35f, 1.5f), 40f).transform;

            // Bowling-alley pomp — sell the cradle / plinth
            director.babyLane = CamMount.Create(root, "Cam_BabyLane",
                new Vector3(3.4f, 2.55f, 5.2f), new Vector3(0f, 1.55f, 9.8f), 34f).transform;

            director.eyesCut = CamMount.Create(root, "Cam_Eyes",
                new Vector3(0f, 1.25f, -3.6f), new Vector3(0f, 1.15f, -6.2f), 32f).transform;

            director.CutTo(director.lobbyOrbit);

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
