using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuseBoxManager : MonoBehaviour
{
    [Header("Fuse Requirements")]
    [Tooltip("The name applied to fuse items in inventory")]
    public string fuseItemName = "Fuse";
    [Tooltip("Number of fuses needed to activate the fusebox")]
    public int requiredFuseCount = 3;

    [Header("Lights Configuration")]
    [Tooltip("GameObjects that will be turned ON when fusebox is activated")]
    public List<GameObject> lightsToTurnOn = new List<GameObject>();
    [Tooltip("GameObjects that will be turned OFF when fusebox is activated")]
    public List<GameObject> lightsToTurnOff = new List<GameObject>();
    
    [Header("Door Control")]
    [Tooltip("Door that will close when fusebox is activated")]
    public DoorOpener doorToClose;
    [Tooltip("Delay before closing the door (seconds)")]
    public float doorCloseDelay = 1.5f;

    [Header("Audio")]
    [Tooltip("Sound played when fusebox is activated")]
    public AudioClip activationSound;
    [Tooltip("Sound played when player tries to activate without enough fuses")]
    public AudioClip failSound;
    
    [Header("Effects")]
    [Tooltip("Optional particle effect to play when activated")]
    public ParticleSystem activationEffect;
    [Tooltip("Duration in seconds to play the particle effect")]
    public float particleEffectDuration = 2f;

    private AudioSource audioSource;
    private bool isActivated = false;
    private Inventory playerInventory;
    private InteractBehavior interactBehavior;

    private void Start()
    {
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Try to find player's inventory and interact behavior
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            interactBehavior = player.GetComponent<InteractBehavior>();
            
            if (playerInventory == null)
                Debug.LogError("FuseBoxManager: Player found but it has no Inventory component!");
        }
        else
        {
            Debug.LogError("FuseBoxManager: No GameObject with 'Player' tag found!");
        }
        
        // Make sure this object is on the "Interactable" layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1) {
            gameObject.layer = interactableLayer;
        } else {
            Debug.LogError("FuseBoxManager: 'Interactable' layer not found. Make sure to create this layer in Unity's Layer settings!");
        }

        // Vérifier si la porte à fermer est assignée
        if (doorToClose == null)
        {
            Debug.LogWarning("FuseBoxManager: Aucune porte n'a été assignée pour être fermée lors de l'activation!");
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    public int CountFuses()
    {
        if (playerInventory == null)
        {
            Debug.LogError("FuseBoxManager: Cannot count fuses - playerInventory is null!");
            return 0;
        }
            
        // Debug the entire inventory content first
        Debug.Log("FuseBoxManager: Dumping inventory contents for debugging:");
        
        // Accéder directement aux slots de l'inventaire
        int totalFuseCount = 0;
        
        for (int i = 0; i < playerInventory.Count; i++)
        {
            ItemData item = playerInventory.GetItemAt(i);
            if (item == null) continue;
            
            // Obtenir le slot complet pour accéder à count
            InventorySlot slot = playerInventory.GetSlot(i);
            int slotCount = (slot != null) ? slot.count : 1;
            
            Debug.Log($"Inventory slot {i}: {item.itemName} (count: {slotCount})");
            
            // Check pour les fusibles
            if (IsFuseItem(item))
            {
                totalFuseCount += slotCount;
                Debug.Log($"Found fuse(s) in slot {i}: {item.itemName} × {slotCount}");
            }
        }
        
        Debug.Log($"Total fuses found in inventory: {totalFuseCount}");
        return totalFuseCount;
    }
    
    private bool IsFuseItem(ItemData item)
    {
        if (item == null) return false;
        
        // Correspondance exacte avec fuseItemName (par défaut: "Fuse")
        if (item.itemName == fuseItemName)
            return true;
            
        // Correspondance avec suffixe comme "Fuse (1)"
        if (item.itemName.StartsWith(fuseItemName + " (") && item.itemName.EndsWith(")"))
            return true;
            
        // Vérifier si le nom contient "Fuse" comme fallback
        if (item.itemName.Contains("Fuse"))
            return true;
            
        return false;
    }

    public bool CanActivate()
    {
        if (isActivated)
            return false;

        int fuseCount = CountFuses();
        return fuseCount >= requiredFuseCount;
    }

    public string GetInteractionText()
    {
        if (isActivated)
            return "Fusebox is already active";
            
        int fuseCount = CountFuses();
        
        Debug.Log($"GetInteractionText - Fuses in inventory: {fuseCount}/{requiredFuseCount}");
            
        if (fuseCount >= requiredFuseCount)
            return "Activate fusebox (F)";
        else
            return $"Need {requiredFuseCount} fuses to activate (have {fuseCount})";
    }

    public void HandleInteraction()
    {
        Debug.Log("FuseBox interaction handled!");
        
        if (isActivated)
        {
            // Already activated
            if (interactBehavior != null)
                interactBehavior.ShowTemporaryMessage("Fusebox already activated", 2f);
            return;
        }

        int fuseCount = CountFuses();
        
        if (fuseCount >= requiredFuseCount)
        {
            ActivateFuseBox();
        }
        else
        {
            // Not enough fuses
            if (audioSource != null && failSound != null)
                audioSource.PlayOneShot(failSound);

            if (interactBehavior != null)
                interactBehavior.ShowTemporaryMessage($"Need {requiredFuseCount} fuses to activate (have {fuseCount})", 2f);
        }
    }

    private void ActivateFuseBox()
    {
        // Mark as activated
        isActivated = true;

        // Retirer les fusibles de l'inventaire
        if (playerInventory != null)
        {
            int fusesToRemove = requiredFuseCount;
            
            // Parcourir l'inventaire pour trouver des fusibles
            for (int i = 0; i < playerInventory.Count && fusesToRemove > 0; i++)
            {
                InventorySlot slot = playerInventory.GetSlot(i);
                if (slot == null || slot.data == null || !IsFuseItem(slot.data))
                    continue;
                
                // Nombre de fusibles qu'on peut prendre de ce slot
                int fusesTaken = Mathf.Min(slot.count, fusesToRemove);
                
                // Retirer les fusibles
                for (int j = 0; j < fusesTaken; j++)
                {
                    playerInventory.RemoveOne(slot.data);
                    fusesToRemove--;
                }
                
                // Recommencer la boucle si le tableau a changé
                if (fusesTaken > 0)
                    i = -1;
            }
        }

        // Play activation sound
        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);
            
        // Play particle effect for a limited time if available
        if (activationEffect != null)
        {
            StartCoroutine(PlayParticleEffectForDuration(activationEffect, particleEffectDuration));
        }

        // Turn on specified GameObjects
        foreach (GameObject obj in lightsToTurnOn)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Turn off specified GameObjects
        foreach (GameObject obj in lightsToTurnOff)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
        // Fermer la porte après un délai si configurée
        if (doorToClose != null)
        {
            StartCoroutine(CloseTheDoorAfterDelay(doorToClose, doorCloseDelay));
        }

        // Show message to player
        if (interactBehavior != null)
            interactBehavior.ShowTemporaryMessage("Fusebox activated. Power restored.", 3f);
    }
    
    private IEnumerator CloseTheDoorAfterDelay(DoorOpener door, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Vérifier si la porte est verrouillée
        if (door.IsLocked())
        {
            // Déverrouiller la porte d'abord si elle est verrouillée
            door.Unlock();
        }
        
        // On ne peut pas vérifier si la porte est en train de s'animer car isAnimating est privé
        // Attendons un court instant pour s'assurer que tout effet précédent est terminé
        yield return new WaitForSeconds(0.2f);
        
        // Essayer de fermer la porte
        door.TryToggleDoor();
        
        Debug.Log("Door closed automatically after fusebox activation");
    }
    
    private IEnumerator PlayParticleEffectForDuration(ParticleSystem effect, float duration)
    {
        // Assurez-vous que l'effet est arrêté au cas où
        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        // Jouer l'effet
        effect.Play(true);
        
        // Attendre la durée spécifiée
        yield return new WaitForSeconds(duration);
        
        // Arrêter l'effet
        effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
        Debug.Log($"Particle effect stopped after {duration} seconds");
    }
    
    // This helps with debugging
    private void OnDrawGizmos()
    {
        // Draw a wire cube to visualize the interaction area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, 0.2f));
    }
} 