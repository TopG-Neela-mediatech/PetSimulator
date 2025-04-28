using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MainCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Joystick Reference")]
    public VariableJoystick m_joystick;

    private Rigidbody rb;
    private bool isGrounded;

    // Animator animator; // Optional, for animation

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (m_joystick == null)
        {
            Debug.LogError("Variable Joystick reference is missing!");
        }

        // animator = GetComponent<Animator>(); // Optional
    }

    void Update()
    {
        // Movement using joystick
        float horizontal = m_joystick.Horizontal;

        //Debug.Log(horizontal);
        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        transform.position = pos;
    }

    // Public method for UI button to call
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Optional animation
            // animator.SetTrigger("Jump");

            isGrounded = false;
        }
    }

}
