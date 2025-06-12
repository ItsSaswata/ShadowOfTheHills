using UnityEngine;
using DG.Tweening;

public class Double_door_open : MonoBehaviour
{
    [Header("Double Door Settings")]
    [Tooltip("Is the door currently open?")]
    public bool isOpen = false;
    
    [Tooltip("Duration of the door animation in seconds")]
    public float animationDuration = 1.0f;
    
    [Tooltip("Ease type for the door animation")]
    public Ease easeType = Ease.InOutQuad;
    
    [Tooltip("Reference to the left door that will open -90 degrees")]
    public Door_open leftDoor;
    
    [Tooltip("Reference to the right door that will open +90 degrees")]
    public Door_open rightDoor;
    
    [Tooltip("Sound to play when doors open")]
    public AudioClip openSound;
    
    [Tooltip("Sound to play when doors close")]
    public AudioClip closeSound;
    
    private AudioSource audioSource;
    private bool doorsConfigured = false;
    
    void Start()
    {
        ConfigureDoors();
        
        // Add an audio source component if sounds are assigned
        if (openSound != null || closeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // 3D sound
        }
    }
    
    // This method can be called anytime to configure the doors
    public void ConfigureDoors()
    {
        // Configure the doors if they exist
        if (leftDoor != null)
        {
            leftDoor.rotatePositive = false; // Set to rotate -90 degrees
            leftDoor.animationDuration = animationDuration;
            leftDoor.easeType = easeType;
            doorsConfigured = true;
        }
        
        if (rightDoor != null)
        {
            rightDoor.rotatePositive = true; // Set to rotate +90 degrees
            rightDoor.animationDuration = animationDuration;
            rightDoor.easeType = easeType;
            doorsConfigured = true;
        }
    }
    
    // This creates a button in the inspector that can be clicked to test the doors
    [ContextMenu("Test Double Door Toggle")]
    public void TestDoubleDoorToggle()
    {
        // Make sure doors are configured before testing
        if (!doorsConfigured)
        {
            ConfigureDoors();
        }
        
        ToggleDoubleDoors();
        Debug.Log($"Double doors are now {(isOpen ? "open" : "closed")}");
    }
    
    /// <summary>
    /// Toggle both doors between open and closed states
    /// </summary>
    public void ToggleDoubleDoors()
    {
        // Make sure doors are configured
        if (!doorsConfigured)
        {
            ConfigureDoors();
        }
        
        // Toggle the door state
        isOpen = !isOpen;
        
        // Toggle both doors
        if (leftDoor != null)
        {
            // Ensure the left door's state matches our state
            if (leftDoor.isOpen != isOpen)
            {
                leftDoor.ToggleDoor();
            }
        }
        else
        {
            Debug.LogWarning("Left door reference is missing!");
        }
        
        if (rightDoor != null)
        {
            // Ensure the right door's state matches our state
            if (rightDoor.isOpen != isOpen)
            {
                rightDoor.ToggleDoor();
            }
        }
        else
        {
            Debug.LogWarning("Right door reference is missing!");
        }
        
        // Play the appropriate sound
        PlayDoorSound();
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
    /// Example of how to trigger the doors from another script or interaction
    /// </summary>
    public void Interact()
    {
        ToggleDoubleDoors();
    }
    
    /// <summary>
    /// Set door references at runtime and configure them
    /// </summary>
    public void SetDoors(Door_open left, Door_open right)
    {
        leftDoor = left;
        rightDoor = right;
        ConfigureDoors();
        Debug.Log("Door references set and configured successfully.");
    }
}