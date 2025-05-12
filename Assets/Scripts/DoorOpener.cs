using UnityEngine;
using System.Collections;

/// <summary>
/// Simple door opener/closer that applies the same audio principles as the HeadBob
/// script: a 3D-spatialised AudioSource is configured once and every time the door
/// starts to move we play the squeak with a slight random pitch & volume variation.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DoorOpener : MonoBehaviour
{
    [Header("Animation")]
    public float openAngle = -90f;          // Y-axis angle when fully opened
    public float speed = 2f;               // Lerp speed

    [Header("Squeak sound")]
    [SerializeField] private AudioClip squeakAudio;
    [SerializeField, Range(0f, .2f)] private float pitchVariation = 0.05f;
    [SerializeField, Range(0f, 1f)] private float volumeMin = 0.8f;
    [SerializeField, Range(0f, 1f)] private float volumeMax = 1f;

    [Header("État Porte")]
    [Tooltip("Si la porte est verrouillée au démarrage")]
    public bool isLockedByDefault = false;
    [Tooltip("ID unique de la porte pour l'associer à un lecteur de badge")]
    public string doorID;
    [Tooltip("Distance maximale à laquelle le joueur peut interagir avec la porte")]
    public float maxInteractionDistance = 3f;

    private bool isOpen;
    private bool isAnimating;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isLocked;
    
    // Référence au DoorLock si présent
    private DoorLock doorLock;

    private AudioSource audioSource;

    // Référence au collider pour les interactions
    private Collider doorCollider;
    private Transform playerTransform;
    
    // Référence au menu pause pour vérifier si un menu est ouvert
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;
    
    // Référence à l'inventaire du joueur pour vérifier la clé
    private Inventory playerInventory;
    
    // Pour gérer l'activation/désactivation des entrées
    private bool inputEnabled = true;

    private void Awake()
    {
        // Cache rotations
        closedRotation = transform.rotation;
        openRotation   = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;

        // Ensure we have an AudioSource and configure it like in HeadBob
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource(audioSource);

        isLocked = isLockedByDefault;
        
        // Récupérer le collider pour détecter les clics
        doorCollider = GetComponent<Collider>();
        if (doorCollider == null)
        {
            doorCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Trouver la caméra du joueur pour les vérifications de distance
        playerTransform = Camera.main.transform;
        
        // Récupérer le DoorLock s'il existe
        doorLock = GetComponent<DoorLock>();
    }
    
    private void Start()
    {
        // Récupérer les références aux gestionnaires de menu
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        
        // Trouver l'inventaire du joueur
        var player = FindFirstObjectByType<InteractBehavior>();
        if (player != null)
        {
            playerInventory = player.inventory;
        }
    }

    private void Update()
    {
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
        
        // Ne pas traiter les clics si un menu est ouvert
        if (isAnyMenuOpen)
        {
            return;
        }
        
        // Détection du clic sur la porte avec vérification de distance
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && hit.collider == doorCollider)
            {
                // Vérification de la distance
                float distanceToPlayer = Vector3.Distance(hit.point, playerTransform.position);
                if (distanceToPlayer <= maxInteractionDistance)
                {
                    HandleDoorInteraction();
                }
            }
        }
    }
    
    // Gère l'interaction avec la porte au clic
    private void HandleDoorInteraction()
    {
        // Si la porte a un DoorLock, vérifier si le joueur a la clé
        if (doorLock != null && isLocked && playerInventory != null)
        {
            string requiredKeyName = doorLock.GetRequiredKeyName();
            if (playerInventory.Has(requiredKeyName))
            {
                // Le joueur a la clé, on déverrouille et on ouvre la porte
                isLocked = false;
                StartCoroutine(AnimateDoor());
                
                // Optionnel: jouer un son de déverrouillage
                AudioClip unlockSound = doorLock.GetUnlockSound();
                if (unlockSound != null)
                {
                    audioSource.PlayOneShot(unlockSound);
                }
                
                // Afficher message (via DoorLock)
                doorLock.ShowUnlockMessage();
                return;
            }
            else
            {
                // Le joueur n'a pas la clé, afficher le message
                doorLock.ShowLockedMessage();
                return;
            }
        }
        
        // Si on arrive ici, soit la porte n'a pas de DoorLock,
        // soit elle n'est pas verrouillée, donc on essaie de l'ouvrir normalement
        TryToggleDoor();
    }
    
    // Méthode appelée par l'InputManager pour activer/désactiver les entrées
    public void EnableInput(bool enable)
    {
        inputEnabled = enable;
    }

    private IEnumerator AnimateDoor()
    {
        isAnimating = true;

        PlaySqueak();   // Play once at the start of every open/close action

        Quaternion startRotation  = transform.rotation;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        isOpen = !isOpen;
        isAnimating = false;
    }

    /// <summary>Plays the door squeak with random pitch & volume.</summary>
    private void PlaySqueak()
    {
        if (squeakAudio == null) return;

        audioSource.pitch  = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.PlayOneShot(squeakAudio);
    }

    /// <summary>Configures the AudioSource once, mirroring HeadBob settings.</summary>
    private static void ConfigureAudioSource(AudioSource source)
    {
        source.spatialBlend = 1f;                 // Fully 3D
        source.rolloffMode  = AudioRolloffMode.Linear;
        source.minDistance  = 1f;
        source.maxDistance  = 10f;
        source.dopplerLevel = 0f;
        source.playOnAwake  = false;
    }

    // Essaie d'ouvrir/fermer la porte
    public void TryToggleDoor()
    {
        if (isLocked)
        {
            return;
        }
        
        StartCoroutine(AnimateDoor());
    }

    // Méthode appelée par le lecteur de badge pour déverrouiller
    public void Unlock()
    {
        isLocked = false;
    }
    
    // Pour verrouiller à nouveau
    public void Lock()
    {
        isLocked = true;
    }
    
    // Pour vérifier l'état actuel
    public bool IsLocked()
    {
        return isLocked;
    }
}