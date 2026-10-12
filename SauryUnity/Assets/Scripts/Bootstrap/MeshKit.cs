using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>Primitive kit helpers — smoother spheres, jointed limbs, dingy mats.</summary>
    public static class MeshKit
    {
        static Mesh _smoothSphere;

        public static GameObject Part(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 localPos,
            Vector3 localScale,
            Color color,
            bool collider = false,
            float smoothness = 0.22f,
            float metallic = 0.04f)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = localScale;
            if (type == PrimitiveType.Sphere)
                UpgradeSphereMesh(go);
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
            bool collider = false,
            float smoothness = 0.22f,
            float metallic = 0.04f)
        {
            var go = Part(parent, name, type, localPos, localScale, color, collider, smoothness, metallic);
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
            var go = Part(parent, name, type, localPos, localScale, surface, smoothness: 0.4f);
            PrimitiveMaterials.ApplyEmissive(go, surface, emission, intensity);
            return go;
        }

        /// <summary>Capsule limb with overlapping joint spheres so nothing floats.</summary>
        public static Transform LimbChain(
            Transform parent,
            string name,
            Vector3 localPos,
            Vector3 euler,
            float length,
            float radius,
            Color color)
        {
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);
            root.localPosition = localPos;
            root.localRotation = Quaternion.Euler(euler);

            // Shoulder / hip joint ball
            Part(root, "JointA", PrimitiveType.Sphere, Vector3.zero, Vector3.one * (radius * 2.15f), color, smoothness: 0.28f);
            // Bone
            Part(root, "Bone", PrimitiveType.Capsule,
                new Vector3(0f, -length * 0.5f, 0f),
                new Vector3(radius * 2f, length * 0.5f, radius * 2f),
                color, smoothness: 0.25f);
            // Elbow / knee ball
            Part(root, "JointB", PrimitiveType.Sphere,
                new Vector3(0f, -length, 0f), Vector3.one * (radius * 2.05f), color, smoothness: 0.28f);
            return root;
        }

        static void UpgradeSphereMesh(GameObject go)
        {
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null) return;
            if (_smoothSphere == null)
                _smoothSphere = BuildUvSphere(24, 16);
            filter.sharedMesh = _smoothSphere;
        }

        static Mesh BuildUvSphere(int longSegments, int latSegments)
        {
            var mesh = new Mesh { name = "SaurySmoothSphere" };
            int vertsU = longSegments + 1;
            int vertsV = latSegments + 1;
            var verts = new Vector3[vertsU * vertsV];
            var norms = new Vector3[verts.Length];
            var uvs = new Vector2[verts.Length];
            int i = 0;
            for (int y = 0; y < vertsV; y++)
            {
                float v = y / (float)latSegments;
                float pitch = (v - 0.5f) * Mathf.PI;
                float cy = Mathf.Sin(pitch);
                float cr = Mathf.Cos(pitch);
                for (int x = 0; x < vertsU; x++)
                {
                    float u = x / (float)longSegments;
                    float yaw = u * Mathf.PI * 2f;
                    var p = new Vector3(Mathf.Cos(yaw) * cr, cy, Mathf.Sin(yaw) * cr) * 0.5f;
                    verts[i] = p;
                    norms[i] = p.normalized;
                    uvs[i] = new Vector2(u, v);
                    i++;
                }
            }
            var tris = new int[longSegments * latSegments * 6];
            int t = 0;
            for (int y = 0; y < latSegments; y++)
            {
                for (int x = 0; x < longSegments; x++)
                {
                    int i0 = y * vertsU + x;
                    int i1 = i0 + 1;
                    int i2 = i0 + vertsU;
                    int i3 = i2 + 1;
                    tris[t++] = i0; tris[t++] = i2; tris[t++] = i1;
                    tris[t++] = i1; tris[t++] = i2; tris[t++] = i3;
                }
            }
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
