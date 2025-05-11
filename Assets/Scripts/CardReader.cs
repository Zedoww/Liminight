using UnityEngine;

public class CardReader : MonoBehaviour
{
    [SerializeField] private DoorOpener doorToUnlock;
    [SerializeField] private string requiredItemName = "IDCard";
    [Tooltip("ID de la porte à déverrouiller - doit correspondre au doorID de la porte")]
    [SerializeField] private string targetDoorID = "door01";
    [Tooltip("Si true, déverrouille la porte. Si false, l'ouvre directement.")]
    [SerializeField] private bool unlockOnly = true;
    public string RequiredItemName => requiredItemName;

    private bool isActivated = false;
    private Inventory playerInventory;
    
    // Pour les effets sonores et visuels
    [Header("Feedback")]
    [SerializeField] private AudioClip accessGrantedSound;
    [SerializeField] private AudioClip accessDeniedSound;
    [SerializeField] private GameObject successEffect;
    private AudioSource audioSource;
    
    // Référence au menu pause pour vérifier si un menu est ouvert
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;
    
    // Pour gérer l'activation/désactivation des entrées
    private bool inputEnabled = true;

    void Awake()
    {
        // S'assure que le CardReader est sur le layer "Interactable"
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    void Start()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        
        // Si doorToUnlock n'est pas assigné, essayer de trouver une porte avec l'ID correspondant
        if (doorToUnlock == null)
        {
            DoorOpener[] allDoors = FindObjectsByType<DoorOpener>(FindObjectsSortMode.None);
            foreach (var door in allDoors)
            {
                if (door.doorID == targetDoorID)
                {
                    doorToUnlock = door;
                    break;
                }
            }
        }
        
        // Configurer l'audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // Désactiver les effets visuels au démarrage
        if (successEffect != null)
            successEffect.SetActive(false);
            
        // Récupérer les références aux gestionnaires de menu
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
    }
    
    // Méthode appelée par l'InputManager pour activer/désactiver les entrées
    public void EnableInput(bool enable)
    {
        inputEnabled = enable;
    }

    void Update()
    {
        if (isActivated) return;
        
        // Si les entrées sont désactivées, ne rien faire
        if (!inputEnabled)
            return;
        
        // Vérifier si un menu est ouvert
        bool isAnyMenuOpen = false;
        
        if (pauseMenuManager != null)
        {
            isAnyMenuOpen = pauseMenuManager.IsOpen();
        }
        
        if (inventoryUI != null && !isAnyMenuOpen)
        {
            isAnyMenuOpen = inventoryUI.IsOpen();
        }
        
        // Ne pas traiter les interactions si un menu est ouvert
        if (isAnyMenuOpen)
        {
            return;
        }

        // Raycast depuis le centre de l'écran
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            // Vérifie si on regarde ce lecteur de carte
            if (hit.collider.gameObject == gameObject)
            {
                // Vérifie si le joueur a le badge requis
                bool hasRequiredItem = playerInventory.Has(requiredItemName);

                if (hasRequiredItem)
                {
                    // Vérifie l'input pour activer le lecteur
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        ActivateReader();
                    }
                }
            }
        }
    }

    void ActivateReader()
    {
        if (doorToUnlock == null)
        {
            Debug.LogError("Aucune porte associée au lecteur " + gameObject.name);
            return;
        }

        isActivated = true;

        // Effet sonore et visuel de succès
        if (accessGrantedSound != null && audioSource != null)
            audioSource.PlayOneShot(accessGrantedSound);
            
        if (successEffect != null)
        {
            successEffect.SetActive(true);
            Invoke("HideEffect", 3f); // Désactiver l'effet après 3 secondes
        }

        // Déverrouiller ou ouvrir la porte
        if (unlockOnly)
        {
            doorToUnlock.Unlock();
            Debug.Log("Porte déverrouillée ! Utilise le clic gauche pour l'ouvrir.");
        }
        else
        {
            // Comportement précédent - ouvre directement la porte
            doorToUnlock.Unlock();
            doorToUnlock.TryToggleDoor();
        }
    }
    
    void HideEffect()
    {
        if (successEffect != null)
            successEffect.SetActive(false);
    }
    
    // Méthode pour vérifier si le lecteur a déjà été activé
    public bool IsActivated()
    {
        return isActivated;
    }
} 