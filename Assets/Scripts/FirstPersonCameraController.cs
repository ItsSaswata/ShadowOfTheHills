using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float verticalClamp = 60f;

    private PlayerControls controls;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private Transform playerBody;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to Look input
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
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
        // Apply input
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        // Rotate camera (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player body (yaw)
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }
}
