using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>Low-level primitive part helper — no collider by default for visual kits.</summary>
    public static class MeshKit
    {
        public static GameObject Part(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPos,
            Vector3 localScale,
            Color color,
            bool collider = false,
            float smoothness = 0.18f,
            float metallic = 0.05f)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = localScale;
            PrimitiveMaterials.Apply(go, color, smoothness, metallic);
            if (!collider)
            {
                var col = go.GetComponent<Collider>();
                if (col != null) Object.Destroy(col);
            }
            return go;
        }

        public static GameObject PartRot(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPos,
            Vector3 euler,
            Vector3 localScale,
            Color color,
            bool collider = false)
        {
            var go = Part(parent, name, type, localPos, localScale, color, collider);
            go.transform.localRotation = Quaternion.Euler(euler);
            return go;
        }

        public static GameObject GlowPart(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPos,
            Vector3 localScale,
            Color surface,
            Color emission,
            float intensity = 3f)
        {
            var go = Part(parent, name, type, localPos, localScale, surface);
            PrimitiveMaterials.ApplyEmissive(go, surface, emission, intensity);
            return go;
        }
    }
}
