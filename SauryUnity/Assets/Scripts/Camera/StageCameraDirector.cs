using System.Collections;
using UnityEngine;

namespace Saury.CameraRig
{
    /// <summary>Liar's Bar–ish cut language without requiring Cinemachine at runtime bootstrap.</summary>
    public class StageCameraDirector : MonoBehaviour
    {
        public Camera mainCam;
        public Transform lobbyOrbit;
        public Transform hostPush;
        public Transform miniWide;
        public Transform podium;
        public Transform babyLane;
        public Transform eyesCut;

        Transform _current;
        Transform _from;
        float _blend;
        float _blendDur;
        Vector3 _shake;

        public void CutTo(Transform t)
        {
            _current = t;
            _from = null;
            _blend = 1f;
            Apply(1f);
        }

        public void BlendTo(Transform t, float dur = 0.55f)
        {
            _from = _current != null ? _current : (mainCam != null ? mainCam.transform : null);
            _current = t;
            _blend = 0f;
            _blendDur = Mathf.Max(0.05f, dur);
        }

        public IEnumerator WhipTo(Transform t, float dur = 0.28f)
        {
            BlendTo(t, dur);
            yield return new WaitForSeconds(dur);
            Punch(0.12f);
        }

        public void Punch(float amount = 0.08f)
        {
            _shake = Random.insideUnitSphere * amount;
        }

        void LateUpdate()
        {
            if (mainCam == null || _current == null) return;
            if (_blend < 1f)
            {
                _blend += Time.deltaTime / _blendDur;
                Apply(Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(_blend)));
            }
            else
            {
                Apply(1f);
            }

            if (_shake.sqrMagnitude > 0.00001f)
            {
                mainCam.transform.position += _shake;
                _shake = Vector3.Lerp(_shake, Vector3.zero, Time.deltaTime * 8f);
            }
        }

        void Apply(float t)
        {
            if (_from != null && t < 1f)
            {
                mainCam.transform.position = Vector3.Lerp(_from.position, _current.position, t);
                mainCam.transform.rotation = Quaternion.Slerp(_from.rotation, _current.rotation, t);
            }
            else
            {
                mainCam.transform.position = _current.position;
                mainCam.transform.rotation = _current.rotation;
            }
        }

        public void TickLobbyOrbit(float time)
        {
            if (lobbyOrbit == null) return;
            float ang = time * 12f;
            lobbyOrbit.position = new Vector3(Mathf.Sin(ang * Mathf.Deg2Rad) * 4.2f, 2.1f, Mathf.Cos(ang * Mathf.Deg2Rad) * 4.2f);
            lobbyOrbit.LookAt(new Vector3(0f, 1.2f, 0f));
        }
    }
}
