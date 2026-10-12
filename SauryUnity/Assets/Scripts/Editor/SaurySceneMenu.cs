#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Saury.EditorTools
{
    public static class SaurySceneMenu
    {
        [MenuItem("Saury/Open Bootstrap Scene")]
        public static void OpenBootstrap()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Bootstrap.unity");
        }

        [MenuItem("Saury/Play Vertical Slice Info")]
        public static void Info()
        {
            EditorUtility.DisplayDialog(
                "Saury! Vertical Slice",
                "Open Assets/Scenes/Bootstrap.unity and press Play.\n\n" +
                "Flow: Lobby → Make Dad → Pissing Contest → Baby Showcase → BBQ Brawl → Baby Showcase → Crown.\n\n" +
                "Host is CPU-only. AI baby uses mock generator until API keys exist.",
                "OK");
        }
    }
}
#endif
