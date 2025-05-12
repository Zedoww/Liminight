// InteractBehavior.cs
using UnityEngine;
using System.Collections;

public class InteractBehavior : MonoBehaviour
{
    public Inventory inventory;
    public float pickupRange = 3f;
    public LayerMask pickableMask;
    public Camera cam;
    public CrosshairAnimator crosshairAnimator;
    public InteractPromptUI interactPrompt;
    public AudioClip equipSound;
    [Tooltip("Duration of the 'You need a key' message display")]
    public float keyMessageDuration = 2.5f;

    private AudioSource audioSource;
    private float messageTimer = 0f;
    private bool forceShowMessage = false;
    
    // Référence au menu pause pour vérifier si un menu est ouvert
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (crosshairAnimator != null)
            crosshairAnimator.Hide();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        
        // Récupérer les références aux gestionnaires de menu
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        
        // Injecter la référence de interactPrompt dans tous les DoorLock
        if (interactPrompt != null)
        {
            DoorLock[] allDoorLocks = FindObjectsByType<DoorLock>(FindObjectsSortMode.None);
            foreach (var doorLock in allDoorLocks)
            {
                doorLock.SetInteractPrompt(interactPrompt);
            }
            Debug.Log("InteractPrompt injected into " + allDoorLocks.Length + " DoorLocks");
        }
        else
        {
            Debug.LogError("InteractPrompt is null in InteractBehavior");
        }
    }

    void Update()
    {
        // Vérifier si un menu est ouvert
        bool isAnyMenuOpen = false;
        
        if (pauseMenuManager != null)
        {
            isAnyMenuOpen = pauseMenuManager.IsOpen();
        }
        
        if (inventoryUI != null)
        {
            isAnyMenuOpen = isAnyMenuOpen || inventoryUI.IsOpen();
        }
        
        // Mise à jour du timer de message
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                forceShowMessage = false;
            }
        }
        
        // Ne pas traiter les interactions si un menu est ouvert
        if (isAnyMenuOpen)
        {
            // Cacher le crosshair et le prompt d'interaction quand un menu est ouvert
            if (crosshairAnimator != null)
                crosshairAnimator.Hide();
                
            if (interactPrompt != null && !forceShowMessage)
                interactPrompt.Hide();
                
            return;
        }
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool hitInteractable = Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableMask);

        if (crosshairAnimator != null)
        {
            if (hitInteractable)
                crosshairAnimator.Show();
            else
                crosshairAnimator.Hide();
        }

        if (interactPrompt != null)
        {
            if (hitInteractable || forceShowMessage)
            {
                interactPrompt.Show();

                if (!forceShowMessage)
                {
                    // Interaction avec le lecteur de badge
                    if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
                    {
                        if (inventory.Has(cardReader.RequiredItemName))
                        {
                            if (cardReader.IsActivated())
                                interactPrompt.SetText("Door unlocked");
                            else
                                interactPrompt.SetText("Unlock the door (F)");
                        }
                        else
                            interactPrompt.SetText("You need a badge");
                    }
                    // Interaction avec les portes
                    else if (hit.collider.TryGetComponent<DoorOpener>(out var doorOpener))
                    {
                        // Vérifier si la porte a un système de verrouillage par clé
                        if (hit.collider.TryGetComponent<DoorLock>(out var doorLock))
                        {
                            if (doorOpener.IsLocked())
                            {
                                string requiredKeyName = doorLock.GetRequiredKeyName();
                                if (inventory.Has(requiredKeyName))
                                    interactPrompt.SetText("Use the key");
                                else
                                    interactPrompt.Hide(); // Cache le texte si pas de clé
                            }
                            else if (!doorLock.isUnlocked)
                            {
                                interactPrompt.SetText("Door unlocked");
                                doorLock.isUnlocked = true;
                            }
                            else
                            {
                                interactPrompt.Hide();
                            }
                        }
                        else if (doorOpener.IsLocked())
                        {
                            interactPrompt.Hide(); // Cache le texte pour les portes verrouillées
                        }
                    }
                    // Objets ramassables
                    else if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
                    {
                        // On essaie d'abord le nom défini dans le scriptable object,
                        // sinon on prend le nom du prefab dans la scène
                        string nomObjet = holder.itemData != null
                                        ? holder.itemData.itemName   // ou .displayName, selon ta classe
                                        : holder.gameObject.name;

                        interactPrompt.SetText($"Pick up {nomObjet} (F)");
                    }
                }
            }
            else if (!forceShowMessage)
            {
                interactPrompt.Hide();
            }
        }

        if (hitInteractable)
        {
            // Interaction avec clic gauche pour les portes déverrouillées
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent<DoorOpener>(out var doorOpener))
                {
                    // Vérifie si la porte a un DoorLock et est verrouillée
                    if (hit.collider.TryGetComponent<DoorLock>(out var doorLock) && doorOpener.IsLocked())
                    {
                        // Affiche un message que la porte est verrouillée
                        interactPrompt.SetText("This door is locked");
                        messageTimer = keyMessageDuration;
                        forceShowMessage = true;
                    }
                    else if (!doorOpener.IsLocked())
                    {
                        // Seulement ouvrir si la porte n'est pas verrouillée
                        doorOpener.TryToggleDoor();
                    }
                }
            }
            
            // Interaction avec touche F pour les portes verrouillées et les objets à ramasser
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
                {
                    return;
                }
                else if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
                {
                    if (inventory.Add(holder.itemData))
                    {
                        Destroy(holder.gameObject);

                        // Joue un son d'équipement pour tout item ramassé
                        if (equipSound != null)
                            audioSource.PlayOneShot(equipSound);

                        // Si torche, appelle son activation
                        if (holder.itemData.itemName == "Flashlight")
                        {
                            var fl = Object.FindFirstObjectByType<FlashlightController>();
                            if (fl != null)
                                fl.OnEquip();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator OuvrirPorteApresDélai(DoorOpener porte, float délai)
    {
        yield return new WaitForSeconds(délai);
        porte.TryToggleDoor();
    }
}
