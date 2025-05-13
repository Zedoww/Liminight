using UnityEngine;

/// <summary>
/// Place this component on a trigger collider to change background music when player enters the zone
/// </summary>
[RequireComponent(typeof(Collider))]
public class MusicTriggerZone : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] [Tooltip("-1 = default track, 0+ = index in alternative tracks list")]
    private int trackIndex = -1;
    
    [SerializeField] private bool disableAudioInZone = false;
    
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool revertOnExit = true;
    [SerializeField] private int revertToTrackIndex = -1;
    
    private bool playerInZone = false;
    private Collider triggerCollider;
    
    private void Start()
    {
        // Ensure the collider is set as a trigger
        triggerCollider = GetComponent<Collider>();
        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.LogWarning($"MusicTriggerZone on {gameObject.name}: Collider set to trigger mode.", this);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !playerInZone)
        {
            playerInZone = true;
            
            if (disableAudioInZone)
            {
                // Disable background audio
                if (BackgroundAudioManager.Instance != null)
                {
                    BackgroundAudioManager.Instance.SetEnabled(false);
                }
            }
            else
            {
                // Change to specified track
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeBackgroundMusic(trackIndex);
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && playerInZone && revertOnExit)
        {
            playerInZone = false;
            
            if (disableAudioInZone)
            {
                // Re-enable background audio
                if (BackgroundAudioManager.Instance != null)
                {
                    BackgroundAudioManager.Instance.SetEnabled(true);
                }
            }
            else if (revertOnExit)
            {
                // Revert to specified track
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeBackgroundMusic(revertToTrackIndex);
                }
            }
        }
    }
} 