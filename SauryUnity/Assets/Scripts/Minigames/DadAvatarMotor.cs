using Saury.Core;
using UnityEngine;

namespace Saury.Minigames
{
    /// <summary>Soft Gang Beasts–lite body. Slot 0 = human WASD; other seats can be simple AI for solo demo.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public class DadAvatarMotor : MonoBehaviour
    {
        public int slot;
        public string displayName;
        public float moveForce = 28f;
        public float uprightTorque = 40f;
        public float maxSpeed = 6.5f;
        public KeyCode left = KeyCode.A;
        public KeyCode right = KeyCode.D;
        public KeyCode forward = KeyCode.W;
        public KeyCode back = KeyCode.S;
        public KeyCode action = KeyCode.Space;

        public bool aiControlled;
        public Transform aiTarget;
        public float aiActionInterval = 0.85f;

        Rigidbody _rb;
        public bool inputEnabled = true;
        public bool eliminated;
        float _aiActionCd;
        float _aiRetargetCd;
        Vector3 _aiWish;

        /// <summary>True when Space / action held (hose spray etc).</summary>
        public bool ActionHeld
        {
            get
            {
                if (!inputEnabled || eliminated) return false;
                if (aiControlled) return _aiActionCd < aiActionInterval * 0.35f;
                return Input.GetKey(action);
            }
        }

        public bool ActionDown
        {
            get
            {
                if (!inputEnabled || eliminated) return false;
                if (aiControlled) return _aiActionCd <= 0f;
                return Input.GetKeyDown(action);
            }
        }

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.None;
            _rb.mass = 2.2f;
            _rb.linearDamping = 1.4f;
            _rb.angularDamping = 2.5f;
        }

        void FixedUpdate()
        {
            if (eliminated || !inputEnabled || _rb == null) return;

            Vector3 wish = Vector3.zero;
            if (aiControlled)
                wish = TickAiWish();
            else
            {
                if (Input.GetKey(left)) wish += Vector3.left;
                if (Input.GetKey(right)) wish += Vector3.right;
                if (Input.GetKey(forward)) wish += Vector3.forward;
                if (Input.GetKey(back)) wish += Vector3.back;
                wish = wish.normalized;
            }

            _rb.AddForce(wish * moveForce, ForceMode.Acceleration);
            var flat = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flat.magnitude > maxSpeed)
            {
                flat = flat.normalized * maxSpeed;
                _rb.linearVelocity = new Vector3(flat.x, _rb.linearVelocity.y, flat.z);
            }

            var upright = Vector3.Cross(transform.up, Vector3.up);
            _rb.AddTorque(upright * uprightTorque, ForceMode.Acceleration);

            if (!aiControlled && Input.GetKeyDown(action))
                Hop();
        }

        void Update()
        {
            if (!aiControlled || eliminated || !inputEnabled) return;
            _aiActionCd -= Time.deltaTime;
            if (_aiActionCd <= 0f)
            {
                Hop();
                _aiActionCd = aiActionInterval + Random.Range(-0.2f, 0.35f);
            }
        }

        void Hop()
        {
            if (_rb == null) return;
            _rb.AddForce(Vector3.up * 7f + transform.forward * 4f, ForceMode.VelocityChange);
        }

        Vector3 TickAiWish()
        {
            _aiRetargetCd -= Time.fixedDeltaTime;
            if (_aiRetargetCd <= 0f || _aiWish.sqrMagnitude < 0.01f)
            {
                _aiRetargetCd = Random.Range(0.6f, 1.4f);
                if (aiTarget != null)
                {
                    var to = aiTarget.position - transform.position;
                    to.y = 0f;
                    // Approach with jitter so they don't stack
                    _aiWish = (to.normalized + Random.insideUnitSphere).normalized;
                    _aiWish.y = 0f;
                    _aiWish.Normalize();
                }
                else
                {
                    float ang = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    _aiWish = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
                }
            }
            return _aiWish;
        }

        public void ConfigureKeysForSlot(int s)
        {
            slot = s;
            switch (s)
            {
                case 0:
                    left = KeyCode.A; right = KeyCode.D; forward = KeyCode.W; back = KeyCode.S; action = KeyCode.Space;
                    break;
                case 1:
                    left = KeyCode.LeftArrow; right = KeyCode.RightArrow; forward = KeyCode.UpArrow; back = KeyCode.DownArrow; action = KeyCode.RightControl;
                    break;
                case 2:
                    left = KeyCode.J; right = KeyCode.L; forward = KeyCode.I; back = KeyCode.K; action = KeyCode.RightShift;
                    break;
                default:
                    left = KeyCode.F; right = KeyCode.H; forward = KeyCode.T; back = KeyCode.G; action = KeyCode.Return;
                    break;
            }

            // Solo demo: P1 human, others AI
            aiControlled = DemoSettings.AiOtherSeats && DemoSettings.demoMode && s > 0;
        }
    }
}
