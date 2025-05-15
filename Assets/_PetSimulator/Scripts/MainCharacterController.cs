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
        // Movement using joystick
        float horizontal = m_joystick.Horizontal;

        //Debug.Log(horizontal);
        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        transform.position = pos;
    }
}
