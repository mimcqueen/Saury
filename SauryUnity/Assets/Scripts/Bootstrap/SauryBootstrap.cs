using UnityEngine;
using UnityEngine.SceneManagement;

namespace Saury.Bootstrap
{
    /// <summary>
    /// Drop on an empty Bootstrap scene (or let RuntimeInitializeOnLoad create it).
    /// Builds the Maury hub + wires the vertical-slice show flow.
    /// </summary>
    public class SauryBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBoot()
        {
            if (Object.FindAnyObjectByType<SauryBootstrap>() != null) return;
            if (Object.FindAnyObjectByType<Core.GameFlowController>() != null) return;
            var go = new GameObject("SauryBootstrap");
            go.AddComponent<SauryBootstrap>();
        }

        void Start()
        {
            if (Object.FindAnyObjectByType<Core.GameFlowController>() != null) return;
            ClearTemplateSceneDebris();
            var flow = StageWorldBuilder.Build();
            flow.KickOff();
        }

        static void ClearTemplateSceneDebris()
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            foreach (var go in roots)
            {
                if (go == null) continue;
                if (go.name == "SauryBootstrap" || go.name == "SauryWorld") continue;
                Object.Destroy(go);
            }
        }
    }
}
