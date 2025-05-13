using UnityEngine;

/// <summary>
/// Displays an informational message on the UI when the player enters a trigger zone.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InfoTriggerZone : MonoBehaviour
{
    [Header("Message Settings")]
    [SerializeField] private string messageToShow = "Default message";
    [SerializeField] private float messageDuration = 3f;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool showOnce = false; // If true, the message will only be shown once.

    private InteractBehavior interactBehavior;
    private Collider triggerCollider;
    private bool hasBeenShown = false;

    private void Start()
    {
        // Ensure the collider is set as a trigger
        triggerCollider = GetComponent<Collider>();
        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.LogWarning($"InfoTriggerZone on {gameObject.name}: Collider automatically set to trigger mode.", this);
        }

        // Find the InteractBehavior instance in the scene
        // This assumes there's only one InteractBehavior, typically on the player.
        interactBehavior = FindFirstObjectByType<InteractBehavior>();
        if (interactBehavior == null)
        {
            Debug.LogError("InfoTriggerZone: InteractBehavior not found in the scene. Make sure an object with InteractBehavior exists.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactBehavior == null) return; // Guard clause if InteractBehavior wasn't found

        if (other.CompareTag(playerTag))
        {
            if (showOnce && hasBeenShown)
            {
                return; // Don't show again if 'showOnce' is true and it has already been shown
            }

            interactBehavior.ShowTemporaryMessage(messageToShow, messageDuration);
            hasBeenShown = true;
        }
    }
} 