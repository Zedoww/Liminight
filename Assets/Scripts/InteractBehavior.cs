// InteractBehavior.cs
using UnityEngine;

public class InteractBehavior : MonoBehaviour
{
    public Inventory inventory;
    public float pickupRange = 3f;
    public LayerMask pickableMask;
    public Camera cam;
    public CrosshairAnimator crosshairAnimator;
    public InteractPromptUI interactPrompt;
    public AudioClip equipSound;

    private AudioSource audioSource;
    
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
        
        // Ne pas traiter les interactions si un menu est ouvert
        if (isAnyMenuOpen)
        {
            // Cacher le crosshair et le prompt d'interaction quand un menu est ouvert
            if (crosshairAnimator != null)
                crosshairAnimator.Hide();
                
            if (interactPrompt != null)
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
            if (hitInteractable)
            {
                interactPrompt.Show();

                // Interaction avec le lecteur de badge
                if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
                {
                    if (inventory.Has(cardReader.RequiredItemName))
                    {
                        if (cardReader.IsActivated())
                            interactPrompt.SetText("Porte déverrouillée");
                        else
                            interactPrompt.SetText("Déverrouiller la porte (F)");
                    }
                    else
                        interactPrompt.SetText("Il vous faut un badge");
                }
                // Interaction avec les portes
                else if (hit.collider.TryGetComponent<DoorOpener>(out var doorOpener))
                {
                    if (doorOpener.IsLocked())
                        interactPrompt.SetText("Cette porte est verrouillée");
                    else
                        interactPrompt.SetText("Ouvrir/Fermer (Clic gauche)");
                }
                // Objets ramassables
                else if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
                {
                    // On essaie d'abord le nom défini dans le scriptable object,
                    // sinon on prend le nom du prefab dans la scène
                    string nomObjet = holder.itemData != null
                                      ? holder.itemData.itemName   // ou .displayName, selon ta classe
                                      : holder.gameObject.name;

                    interactPrompt.SetText($"Ramasser {nomObjet} (F)");
                }

            }
            else
            {
                interactPrompt.Hide();
            }
        }

        if (hitInteractable && Input.GetKeyDown(KeyCode.F))
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
