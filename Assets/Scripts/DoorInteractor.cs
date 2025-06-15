using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask doorLayerMask;
    public GameObject interactPromptUI;

    private Camera cam;
    private Door_open currentDoor;
    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();

        // Listen for interact input
        inputActions.Player.Interact.performed += ctx => TryInteract();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        cam = Camera.main;

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    private void Update()
    {
        CheckForDoor();
    }

    private void CheckForDoor()
    {
        currentDoor = null;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // 👇 This line draws a visible ray in the Scene view (for debugging only)
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        // Cast the actual ray
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, doorLayerMask))
        {
            Door_open door = hit.collider.GetComponent<Door_open>();
            if (door != null)
            {
                currentDoor = door;
                if (interactPromptUI != null)
                    interactPromptUI.SetActive(true);
                return;
            }
        }

    }

    private void TryInteract()
    {
        if (currentDoor != null)
        {
            currentDoor.Interact();
        }
    }
}
