using UnityEngine;


namespace TMKOC.PetSimulator
{

    [RequireComponent(typeof(Rigidbody))]
    public class MainCharacterController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpForce = 5f;

        [Header("Joystick Reference")]
        public TouchInputReader m_joystick;

        [SerializeField] private Animator m_animator;

        [Header("TEST")]
        [SerializeField] private float m_horizontal = 0f;
        [SerializeField] private float m_turnDuration = 0.933f;

        private readonly int _walkRight = Animator.StringToHash("IsWalkingRight");
        private readonly int _walkLeft = Animator.StringToHash("IsWalkingLeft");

        private Rigidbody rb;
        private bool isGrounded;

        private float movementDelayTimer = 0f;
        private float previousHorizontal = 0f;
        private bool wasIdle = true;

        private bool isDoingSomething = false;

        private void StopMovement()
        {
            isDoingSomething = true;
        }

        private void OnEnable()
        {
            PlayerView.OnReadyToBrush += StopMovement;
        }

        private void OnDisable()
        {
            PlayerView.OnReadyToBrush -= StopMovement;
        }


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            if (m_joystick == null)
            {
                Debug.LogError("Variable Joystick reference is missing!");
            }
        }

        private void Update()
        {
            if(isDoingSomething)
            {
                return;
            }

            // Get horizontal input
            m_horizontal = m_joystick.Horizontal;

            // Reset animation bools
            m_animator.SetBool(_walkLeft, false);
            m_animator.SetBool(_walkRight, false);

            // Check for transition from idle to movement (start walking)
            if (wasIdle && Mathf.Abs(m_horizontal) > 0.1f)
            {
                movementDelayTimer = m_turnDuration;
                wasIdle = false; // no longer idle
            }

            // If joystick is near 0, player is idle
            if (Mathf.Abs(m_horizontal) <= 0.1f)
            {
                wasIdle = true;
            }

            // Set walk animation bools
            if (m_horizontal > 0.1f)
            {
                m_animator.SetBool(_walkLeft, true); // facing cam, so inverted
            }
            else if (m_horizontal < -0.1f)
            {
                m_animator.SetBool(_walkRight, true);
            }

            // Count down delay
            if (movementDelayTimer > 0f)
            {
                movementDelayTimer -= Time.deltaTime;
            }

            // Apply movement only if delay is over
            if (movementDelayTimer <= 0f)
            {
                Vector3 pos = transform.position;
                pos.x += m_horizontal * moveSpeed * Time.deltaTime;
                transform.position = pos;
            }

            previousHorizontal = m_horizontal;
        }
    }

}