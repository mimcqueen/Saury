using UnityEngine;

namespace Saury.Minigames
{
    /// <summary>Soft Gang Beasts–lite body: mushy capsule + springy upright + grab impulse.</summary>
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

        Rigidbody _rb;
        public bool inputEnabled = true;
        public bool eliminated;

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
            if (Input.GetKey(left)) wish += Vector3.left;
            if (Input.GetKey(right)) wish += Vector3.right;
            if (Input.GetKey(forward)) wish += Vector3.forward;
            if (Input.GetKey(back)) wish += Vector3.back;
            wish = wish.normalized;

            _rb.AddForce(wish * moveForce, ForceMode.Acceleration);
            var flat = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flat.magnitude > maxSpeed)
            {
                flat = flat.normalized * maxSpeed;
                _rb.linearVelocity = new Vector3(flat.x, _rb.linearVelocity.y, flat.z);
            }

            // Soft upright spring — Gang Beasts mush, not fighting-game precision
            var upright = Vector3.Cross(transform.up, Vector3.up);
            _rb.AddTorque(upright * uprightTorque, ForceMode.Acceleration);

            if (Input.GetKeyDown(action))
                _rb.AddForce(Vector3.up * 7f + transform.forward * 4f, ForceMode.VelocityChange);
        }

        public void ConfigureKeysForSlot(int s)
        {
            slot = s;
            // Same-PC hotseat stubs: P1 WASD+Space, P2 arrows+RCtrl, P3 IJKL+RShift, P4 TFGH+Enter
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
        }
    }
}
