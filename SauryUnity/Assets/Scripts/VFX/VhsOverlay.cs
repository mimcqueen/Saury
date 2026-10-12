using UnityEngine;
using UnityEngine.UI;

namespace Saury.VFX
{
    /// <summary>Full-screen VHS pastiche on the show output.</summary>
    public class VhsOverlay : MonoBehaviour
    {
        public RawImage noise;
        public Image scanlines;
        public Image frame;
        Texture2D _noiseTex;

        void Awake()
        {
            _noiseTex = new Texture2D(128, 128, TextureFormat.RGB24, false);
            _noiseTex.filterMode = FilterMode.Point;
            if (noise != null) noise.texture = _noiseTex;
        }

        void Update()
        {
            if (_noiseTex == null) return;
            var pixels = _noiseTex.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                byte v = (byte)Random.Range(0, 40);
                pixels[i] = new Color32(v, v, v, 40);
            }
            _noiseTex.SetPixels32(pixels);
            _noiseTex.Apply(false);

            if (scanlines != null)
            {
                var c = scanlines.color;
                c.a = 0.12f + Mathf.Sin(Time.time * 7f) * 0.03f;
                scanlines.color = c;
            }

            if (frame != null)
            {
                float wobble = Mathf.Sin(Time.time * 1.7f) * 2f;
                frame.rectTransform.anchoredPosition = new Vector2(wobble, -wobble * 0.4f);
            }
        }
    }
}
