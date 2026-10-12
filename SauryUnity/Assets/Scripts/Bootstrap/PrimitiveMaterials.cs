using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>URP-safe dingy materials for procedural blockouts.</summary>
    public static class PrimitiveMaterials
    {
        static Shader _lit;

        static Shader LitShader()
        {
            if (_lit == null)
            {
                _lit = Shader.Find("Universal Render Pipeline/Lit")
                       ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                       ?? Shader.Find("Standard")
                       ?? Shader.Find("Unlit/Color");
            }
            return _lit;
        }

        public static Material Color(Color c, float smoothness = 0.18f, float metallic = 0.05f)
        {
            var mat = new Material(LitShader());
            ApplyColor(mat, c);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            return mat;
        }

        public static Material Emissive(Color c, Color emission, float intensity = 2.5f)
        {
            var mat = Color(c, 0.35f, 0.1f);
            var e = emission * intensity;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", e);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            return mat;
        }

        public static void Apply(GameObject go, Color c, float smoothness = 0.18f, float metallic = 0.05f)
        {
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material = Color(c, smoothness, metallic);
        }

        public static void ApplyEmissive(GameObject go, Color c, Color emission, float intensity = 2.5f)
        {
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material = Emissive(c, emission, intensity);
        }

        static void ApplyColor(Material mat, Color c)
        {
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
            mat.color = c;
        }
    }
}
