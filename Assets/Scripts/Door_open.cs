using UnityEngine;
using DG.Tweening; // Import DOTween namespace

public class Door_open : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Is the door currently open?")]
    public bool isOpen = false;
    
    [Tooltip("Should this door rotate +90 degrees? If false, it will rotate -90 degrees")]
    public bool rotatePositive = true;
    
    [Tooltip("Duration of the door animation in seconds")]
    public float animationDuration = 1.0f;
    
    [Tooltip("Ease type for the door animation")]
    public Ease easeType = Ease.InOutQuad;
    
    [Tooltip("Sound to play when door opens")]
    public AudioClip openSound;
    
    [Tooltip("Sound to play when door closes")]
    public AudioClip closeSound;
    
    private AudioSource audioSource;
    private Tween doorTween;
    private float closedRotationY;
    private float openRotationY;
    
    void Start()
    {
        // Store the initial Y rotation as the closed position
        closedRotationY = transform.eulerAngles.y;
        
        // Calculate the open position based on the rotation direction
        openRotationY = rotatePositive ? closedRotationY + 90f : closedRotationY - 90f;
        
        // Add an audio source component if sounds are assigned
        if (openSound != null || closeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // 3D sound
        }
    }
    
    // This creates a button in the inspector that can be clicked to test the door
    [ContextMenu("Test Door Toggle")]
    public void TestDoorToggle()
    {
        // Initialize values if they haven't been set yet (in case Start hasn't run)
        if (closedRotationY == 0 && openRotationY == 0)
        {
            closedRotationY = transform.eulerAngles.y;
            openRotationY = rotatePositive ? closedRotationY + 90f : closedRotationY - 90f;
        }
        
        // Call the toggle door function
        ToggleDoor();
        
        // Log the door state for debugging
        Debug.Log($"Door is now {(isOpen ? "open" : "closed")}");
    }
    
    /// <summary>
    /// Toggle the door between open and closed states
    /// </summary>
    public void ToggleDoor()
    {
        // If a door animation is already in progress, kill it
        if (doorTween != null && doorTween.IsActive())
        {
            doorTween.Kill();
        }
        
        // Recalculate the open position based on current rotation direction setting
        openRotationY = rotatePositive ? closedRotationY + 90f : closedRotationY - 90f;
        
        // Toggle the door state
        isOpen = !isOpen;
        
        // Get the target rotation based on the door state
        float targetRotationY = isOpen ? openRotationY : closedRotationY;
        
        // Create a new Vector3 for the target rotation
        Vector3 targetRotation = new Vector3(
            transform.eulerAngles.x,
            targetRotationY,
            transform.eulerAngles.z
        );
        
        // Animate the door rotation using DOTween
        doorTween = transform.DORotate(targetRotation, animationDuration)
            .SetEase(easeType)
            .OnComplete(OnDoorAnimationComplete);
        
        // Play the appropriate sound
        PlayDoorSound();
    }
    
    /// <summary>
    /// Called when the door animation completes
    /// </summary>
    private void OnDoorAnimationComplete()
    {
        // You can add additional logic here when the door finishes opening/closing
    }
    
    /// <summary>
    /// Play the appropriate door sound based on the door state
    /// </summary>
    private void PlayDoorSound()
    {
        if (audioSource != null)
        {
            AudioClip clipToPlay = isOpen ? openSound : closeSound;
            
            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
        }
    }
    
    /// <summary>
    /// Example of how to trigger the door from another script or interaction
    /// </summary>
    public void Interact()
    {
        ToggleDoor();
    }
}
