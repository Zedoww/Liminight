using UnityEngine;

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
    private InteractPromptUI interactPrompt;
    public bool isUnlocked = false;
    
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
        
        // Trouver le prompt d'interaction
        interactPrompt = FindFirstObjectByType<InteractPromptUI>();
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
    
    // Méthode pour afficher le message de déverrouillage
    public void ShowUnlockMessage()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetText("Door unlocked");
            interactPrompt.Show();
            Invoke("HidePrompt", messageDisplayTime);
        }
        
        
    }
    
    // Méthode pour afficher le message de porte verrouillée
    public void ShowLockedMessage()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetText("This door is locked");
            interactPrompt.Show();
            Invoke("HidePrompt", messageDisplayTime);
        }

    }
    
    // Cette méthode sera appelée par InteractBehavior
    public bool TryUnlock(Inventory playerInventory)
    {
        if (playerInventory == null)
        {
            return false;
        }
            
        // Vérifie si le joueur a la clé requise
        bool hasKey = playerInventory.Has(requiredKeyName);
        
        if (hasKey && !isUnlocked)
        {
            // Déverrouille la porte
            doorOpener.Unlock();
            isUnlocked = true;
            
            // Joue le son de déverrouillage
            if (unlockSound != null)
                audioSource.PlayOneShot(unlockSound);
            
            // Affiche un message de succès
            ShowUnlockMessage();
                
            return true;
        }
        else
        {
            // Joue le son de porte verrouillée
            if (lockedSound != null)
                audioSource.PlayOneShot(lockedSound);
                
            // Affiche un message 
            ShowLockedMessage();
                
            return false;
        }
    }
    
    // Cache le prompt après un délai
    private void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.Hide();
    }
    
    // Méthode pour obtenir la référence directement
    public void SetInteractPrompt(InteractPromptUI prompt)
    {
        interactPrompt = prompt;
    }
} 