using System.Collections;
using UnityEngine;

namespace Saury.CameraRig
{
    /// <summary>
    /// Show camera language without Cinemachine:
    /// Buckshot-table intimacy on hub / Host, fixed high-angle on minis, bowling pomp on baby.
    /// </summary>
    public class StageCameraDirector : MonoBehaviour
    {
        public Camera mainCam;
        public Transform lobbyOrbit;
        public Transform hostPush;
        public Transform makeDadTable;
        public Transform miniWide;
        public Transform podium;
        public Transform babyLane;
        public Transform eyesCut;

        Transform _current;
        Transform _from;
        float _blend;
        float _blendDur;
        Vector3 _shake;
        float _fromFov = 42f;
        float _toFov = 42f;

        public void CutTo(Transform t)
        {
            _current = t;
            _from = null;
            _blend = 1f;
            _toFov = FovOf(t);
            if (mainCam != null) mainCam.fieldOfView = _toFov;
            Apply(1f);
        }

        public void BlendTo(Transform t, float dur = 0.55f)
        {
            _from = _current != null ? _current : (mainCam != null ? mainCam.transform : null);
            _fromFov = mainCam != null ? mainCam.fieldOfView : 42f;
            _current = t;
            _toFov = FovOf(t);
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

        /// <summary>Gang Beasts / party-arena fixed high camera — keeps all 4 dads readable.</summary>
        public void AimMiniHighAngle(Vector3 arenaWorldCenter, float arenaRadius = 7f)
        {
            if (miniWide == null) return;
            float height = 9.2f;
            float back = arenaRadius * 0.95f;
            var pos = arenaWorldCenter + new Vector3(0f, height, -back);
            var look = arenaWorldCenter + new Vector3(0f, 0.35f, 0.4f);
            var mount = miniWide.GetComponent<CamMount>();
            if (mount != null) mount.Place(pos, look, 48f);
            else
            {
                miniWide.position = pos;
                miniWide.LookAt(look);
            }
        }

        void LateUpdate()
        {
            if (mainCam == null || _current == null) return;
            if (_blend < 1f)
            {
                _blend += Time.deltaTime / _blendDur;
                float u = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(_blend));
                Apply(u);
                mainCam.fieldOfView = Mathf.Lerp(_fromFov, _toFov, u);
            }
            else
            {
                Apply(1f);
                mainCam.fieldOfView = _toFov;
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

        /// <summary>Intimate orbit around Host desk (Buckshot table energy — subject lit, void crushed).</summary>
        public void TickLobbyOrbit(float time)
        {
            if (lobbyOrbit == null) return;
            float ang = time * 10f;
            // Tight orbit around desk — not a wide grey void tour
            float r = 3.4f;
            lobbyOrbit.position = new Vector3(
                Mathf.Sin(ang * Mathf.Deg2Rad) * r,
                1.55f + Mathf.Sin(time * 0.35f) * 0.08f,
                0.6f + Mathf.Cos(ang * Mathf.Deg2Rad) * r * 0.55f);
            lobbyOrbit.LookAt(new Vector3(0f, 1.45f, 2.35f));
            var m = lobbyOrbit.GetComponent<CamMount>();
            if (m != null) m.fieldOfView = 38f;
        }

        static float FovOf(Transform t)
        {
            if (t == null) return 42f;
            var m = t.GetComponent<CamMount>();
            return m != null ? m.fieldOfView : 42f;
        }
    }
}
