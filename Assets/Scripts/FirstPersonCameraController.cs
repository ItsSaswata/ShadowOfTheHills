using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [Range(0.0f,10.0f)]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float gamepadSensitivity = 300f;

    [Header("Vertical Clamp")]
    [SerializeField] private float verticalClamp = 60f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.05f;

    // Optional max input delta clamp to prevent spikes
    [SerializeField] private float maxInputDelta = 200f;

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

            // Apply sensitivity based on device type
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
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        controls.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        playerBody = transform.parent;
        if (playerBody == null)
        {
            Debug.LogWarning("FirstPersonCameraController expects to be a child of the player body.");
        }
    }

    private void Update()
    {
        // Smooth the input over time
        currentLookInput = Vector2.SmoothDamp(currentLookInput, targetLookInput, ref smoothVelocity, smoothTime);

        // Optionally clamp large deltas to prevent spikes
        float mouseX = Mathf.Clamp(currentLookInput.x, -maxInputDelta, maxInputDelta);
        float mouseY = Mathf.Clamp(currentLookInput.y, -maxInputDelta, maxInputDelta);

        // Pitch (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (look left/right)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
