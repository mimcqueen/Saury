using UnityEngine;

namespace Saury.CameraRig
{
    /// <summary>Named camera beat with optional FOV (Liar's Bar intimacy vs party high-angle).</summary>
    public class CamMount : MonoBehaviour
    {
        public float fieldOfView = 42f;

        public static CamMount Create(Transform parent, string name, Vector3 pos, Vector3 lookAt, float fov)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.LookAt(lookAt);
            var m = go.AddComponent<CamMount>();
            m.fieldOfView = fov;
            return m;
        }

        public void Place(Vector3 pos, Vector3 lookAt, float? fov = null)
        {
            transform.position = pos;
            transform.LookAt(lookAt);
            if (fov.HasValue) fieldOfView = fov.Value;
        }
    }
}
