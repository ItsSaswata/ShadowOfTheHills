using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float gamepadSensitivity = 300f;

    [Header("Vertical Clamp")]
    [SerializeField] private float verticalClamp = 60f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.05f;

    private PlayerControls controls;
    private Vector2 currentLookInput;
    private Vector2 smoothVelocity;
    private Vector2 targetLookInput;

    private float xRotation = 0f;
    private Transform playerBody;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Look.performed += ctx =>
        {
            Vector2 rawInput = ctx.ReadValue<Vector2>();

            // Apply sensitivity based on device
            if (ctx.control.device is Mouse)
                targetLookInput = rawInput * mouseSensitivity;
            else if (ctx.control.device is Gamepad)
                targetLookInput = rawInput * gamepadSensitivity;
        };

        controls.Player.Look.canceled += _ => targetLookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        controls.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        playerBody = transform.parent;
        if (playerBody == null)
            Debug.LogWarning("FirstPersonCameraController expects to be a child of the player body.");
    }

    private void Update()
    {
        // Smooth the input over time
        currentLookInput = Vector2.SmoothDamp(currentLookInput, targetLookInput, ref smoothVelocity, smoothTime);

        float mouseX = currentLookInput.x * Time.deltaTime;
        float mouseY = currentLookInput.y * Time.deltaTime;

        // Pitch
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
