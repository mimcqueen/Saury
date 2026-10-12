#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Saury.EditorTools
{
    /// <summary>
    /// Captures Game-view style PNGs of the procedural 3D blockout.
    /// Menu: Saury → Capture 3D Slice Screenshots
    /// </summary>
    public static class CaptureSliceViews
    {
        const string OutDir = "Captures/unity-slice";

        [MenuItem("Saury/Capture 3D Slice Screenshots")]
        public static void Capture()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog(
                    "Saury Capture",
                    "Enter Play Mode on Bootstrap.unity first, wait for the hub to build, then run this menu again.\n\n" +
                    "Screenshots write to " + OutDir,
                    "OK");
                return;
            }

            Directory.CreateDirectory(OutDir);
            var cam = Camera.main;
            if (cam == null)
            {
                EditorUtility.DisplayDialog("Saury Capture", "No MainCamera found.", "OK");
                return;
            }

            // Snapshot current + a few staged mounts if present
            CaptureCam(cam, Path.Combine(OutDir, "01-hub-lobby.png"));

            var mounts = new[]
            {
                ("Cam_HostPush", "02-host-desk.png"),
                ("Cam_Eyes", "03-audience-eyes.png"),
                ("Cam_BabyLane", "04-baby-lane.png"),
            };
            var savedPos = cam.transform.position;
            var savedRot = cam.transform.rotation;
            foreach (var (name, file) in mounts)
            {
                var t = GameObject.Find(name)?.transform;
                if (t == null) continue;
                cam.transform.SetPositionAndRotation(t.position, t.rotation);
                CaptureCam(cam, Path.Combine(OutDir, file));
            }
            cam.transform.SetPositionAndRotation(savedPos, savedRot);

            EditorUtility.RevealInFinder(OutDir);
            EditorUtility.DisplayDialog("Saury Capture", "Wrote PNGs to " + Path.GetFullPath(OutDir), "OK");
        }

        static void CaptureCam(Camera cam, string path)
        {
            int w = 1280, h = 720;
            var rt = new RenderTexture(w, h, 24);
            var prev = cam.targetTexture;
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            cam.targetTexture = prev;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }
    }
}
#endif
