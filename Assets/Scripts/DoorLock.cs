using UnityEngine;
// using System.Collections; // Plus nécessaire pour la coroutine locale

[RequireComponent(typeof(DoorOpener))]
public class DoorLock : MonoBehaviour
{
    [SerializeField] private string requiredKeyName = "key";
    [Tooltip("Son joué quand la porte est déverrouillée")]
    [SerializeField] private AudioClip unlockSound;
    [Tooltip("Son joué quand on essaie d'ouvrir la porte sans clé")]
    [SerializeField] private AudioClip lockedSound;
    [Tooltip("Durée d'affichage du message quand on n'a pas la clé")]
    [SerializeField] private float messageDisplayTime = 2f;
    
    private DoorOpener doorOpener;
    private AudioSource audioSource;
    // private InteractPromptUI interactPrompt; // Référence directe moins nécessaire
    public bool isUnlocked = false;
    
    // private Coroutine _hidePromptCoroutine = null; // Supprimé
    private InteractBehavior interactBehavior; // Ajout référence InteractBehavior
    
    private void Start()
    {
        // Récupérer le DoorOpener associé
        doorOpener = GetComponent<DoorOpener>();
        if (doorOpener == null)
        {
            return;
        }
        
        // S'assurer que la porte est verrouillée par défaut
        doorOpener.isLockedByDefault = true;
        
        // Configurer l'audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
        audioSource.playOnAwake = false;
        
        // Trouver le script InteractBehavior (devrait être sur le joueur)
        interactBehavior = FindFirstObjectByType<InteractBehavior>();
        if (interactBehavior == null)
        {
            Debug.LogError("InteractBehavior not found in the scene for DoorLock on " + gameObject.name);
        }
        
        // Trouver le prompt d'interaction - Gardé pour référence si nécessaire mais non utilisé directement
        // interactPrompt = FindFirstObjectByType<InteractPromptUI>();
    }
    
    // Méthode pour récupérer le nom de la clé requise
    public string GetRequiredKeyName()
    {
        return requiredKeyName;
    }
    
    // Méthode pour récupérer le son de déverrouillage
    public AudioClip GetUnlockSound()
    {
        return unlockSound;
    }
    
    // Méthode pour récupérer le son de porte verrouillée
    public AudioClip GetLockedSound()
    {
        return lockedSound;
    }
    
    // Méthodes Show... supprimées car remplacées par appel à InteractBehavior
    /*
    public void ShowUnlockMessage()
    {
        DisplayPrompt("Door unlocked", messageDisplayTime);
    }
    
    public void ShowLockedMessage()
    {
        DisplayPrompt("This door is locked", messageDisplayTime);
    }
    
    // Nouvelle méthode pour gérer l'affichage et le masquage
    private void DisplayPrompt(string message, float delay)
    {
       // ... Ancienne logique de coroutine ...
    }

    // Coroutine pour afficher le message puis le masquer
    private IEnumerator ShowPromptCoroutine(string message, float delay)
    {
       // ... Ancienne logique de coroutine ...
    }
    */

    // Méthode principale appelée par DoorOpener lors d'une interaction
    public void HandleInteraction(Inventory playerInventory)
    {
        if (playerInventory == null) 
        {
            Debug.LogWarning("Inventaire du joueur non trouvé pour DoorLock.");
            return;
        }

        // Vérifier si InteractBehavior a été trouvé
        if (interactBehavior == null)
        {
             Debug.LogError("InteractBehavior reference missing in DoorLock on " + gameObject.name);
             return;
        }

        // Si déjà déverrouillé, on anime juste la porte
        if (isUnlocked)
        {
            if(doorOpener != null)
            {
                 doorOpener.StartCoroutine(doorOpener.AnimateDoor());
            }
            return; 
        }

        // Vérifie si le joueur a la clé requise
        bool hasKey = playerInventory.Has(requiredKeyName);
        
        if (hasKey)
        {   
            isUnlocked = true; 
            doorOpener.Unlock(); 
            
            if (unlockSound != null && audioSource != null)
                audioSource.PlayOneShot(unlockSound);
            
            // Affiche un message de succès via InteractBehavior
            interactBehavior.ShowTemporaryMessage("Door unlocked", messageDisplayTime);
                
            if(doorOpener != null)
            {
                 doorOpener.StartCoroutine(doorOpener.AnimateDoor());
            }
        }
        else
        {   
            if (lockedSound != null && audioSource != null)
                audioSource.PlayOneShot(lockedSound);
                
            // Affiche un message via InteractBehavior
            interactBehavior.ShowTemporaryMessage("This door is locked", messageDisplayTime);
        }
    }

    // Méthode pour obtenir la référence directement (Peut-être plus nécessaire)
    /*
    public void SetInteractPrompt(InteractPromptUI prompt)
    {
        interactPrompt = prompt;
    }
    */
} 