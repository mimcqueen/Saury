using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>URP-safe colored materials for procedural primitives.</summary>
    public static class PrimitiveMaterials
    {
        static Shader _shader;

        public static Material Color(Color c)
        {
            if (_shader == null)
            {
                _shader = Shader.Find("Universal Render Pipeline/Lit")
                          ?? Shader.Find("Universal Render Pipeline/Unlit")
                          ?? Shader.Find("Unlit/Color")
                          ?? Shader.Find("Standard")
                          ?? Shader.Find("Sprites/Default");
            }

            var mat = new Material(_shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", c);
            mat.color = c;
            return mat;
        }

        public static void Apply(GameObject go, Color c)
        {
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material = Color(c);
        }
    }
}
