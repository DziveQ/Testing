using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    [SerializeField] private Transform head; // Assign "Head" (Child of "Body")
    [SerializeField] private Transform playerCamera; // Assign Main Camera (Child of "Head")

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintTransitSpeed = 2f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float mouseSensitivity = 2f; // Mouse sensitivity

    private float verticalVelocity;
    private float speed;
    private float verticalLookRotation = 0f; // Tracks up/down movement

    [Header("Input")]
    private float moveInput;
    private float turnInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS-style movement
        Cursor.visible = false;
    }

    void Update()
    {
        InputManagment();
        Movement();
        MouseLook();
    }

    private void Movement()
    {
        GroundMovement();
    }

    private void GroundMovement()
    {
        Vector3 move = transform.forward * moveInput + transform.right * turnInput; // Move relative to camera

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = Mathf.Lerp(speed, sprintSpeed, Time.deltaTime * sprintTransitSpeed);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, Time.deltaTime * sprintTransitSpeed);
        }

        move.y = 0;
        move *= speed;
        move.y = VerticalForceCalculation();
        controller.Move(move * Time.deltaTime);
    }

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }

    private void InputManagment()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player left and right
        transform.Rotate(Vector3.up * mouseX);

        // Tilt the head up/down
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Limit to 90° up/down

        // Apply rotation to the Head (not the camera directly)
        head.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }
}
