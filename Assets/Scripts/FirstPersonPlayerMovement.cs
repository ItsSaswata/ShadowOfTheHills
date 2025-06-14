using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float groundCheckRadius = 0.4f;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundMask;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 8f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float standingCameraY = 1.6f;
    public float crouchingCameraY = 0.8f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    private Transform groundCheck;

    private float crouchLerp = 0f;
    private float crouchTarget = 0f;
    private float crouchVelocity = 0f; // For smoothing

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(transform);
        groundCheck.localPosition = Vector3.down * (controller.height / 2f);

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;
        controls.Player.Crouch.performed += _ => ToggleCrouch();
        controls.Player.Sprint.performed += _ => ToggleSprint();

        if (cameraTransform != null)
        {
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = standingCameraY;
            cameraTransform.localPosition = camPos;
        }

        controller.height = standingHeight;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        GroundCheck();
        HandleCrouchTransition();
        MovePlayer();
        ApplyGravity();
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.down * (controller.height / 2f - groundCheckRadius);
        isGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void HandleCrouchTransition()
    {
        // Lerp smoothly
        crouchLerp = Mathf.Lerp(crouchLerp, crouchTarget, Time.deltaTime * crouchTransitionSpeed);

        float currentHeight = Mathf.Lerp(standingHeight, crouchHeight, crouchLerp);
        float currentCameraY = Mathf.Lerp(standingCameraY, crouchingCameraY, crouchLerp);

        float heightDiff = currentHeight - controller.height;
        controller.height = currentHeight;
        transform.position += Vector3.up * (heightDiff / 2f);

        if (cameraTransform != null)
        {
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = currentCameraY;
            cameraTransform.localPosition = camPos;
        }

        groundCheck.localPosition = Vector3.down * (controller.height / 2f);
    }

    private void MovePlayer()
    {
        float speed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ToggleCrouch()
    {
        if (isCrouching)
        {
            if (CanStandUp())
            {
                isCrouching = false;
                crouchTarget = 0f;
            }
        }
        else
        {
            isCrouching = true;
            isSprinting = false;
            crouchTarget = 1f;
        }
    }

    private void ToggleSprint()
    {
        if (isCrouching) return;
        isSprinting = !isSprinting;
    }

    private bool CanStandUp()
    {
        float extraHeight = standingHeight - Mathf.Lerp(standingHeight, crouchHeight, crouchLerp);
        Vector3 start = transform.position + Vector3.up * (controller.height / 2f);
        return !Physics.SphereCast(start, controller.radius * 0.9f, Vector3.up, out _, extraHeight + 0.1f, groundMask);
    }
}
