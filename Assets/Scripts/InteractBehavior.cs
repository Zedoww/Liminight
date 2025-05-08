using UnityEngine;

public class InteractBehavior : MonoBehaviour
{
    public Inventory inventory;
    public float pickupRange = 3f;
    public LayerMask pickableMask;
    public Camera cam;
    public CrosshairAnimator crosshairAnimator; // ⚠️ nouveau champ pour le script d'animation
    public InteractPromptUI interactPrompt;

    void Start()
    {
        // Cache le curseur système au démarrage
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Force le curseur UI à être invisible au départ
        if (crosshairAnimator != null)
            crosshairAnimator.Hide();
    }

    void Update()
    {
        // Raycast depuis le centre de l'écran
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool hitInteractable = Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableMask);

        // Affiche ou cache le curseur en douceur
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
                
                // Vérifie d'abord si c'est un lecteur de carte
                if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
                {
                    if (inventory.Has(cardReader.RequiredItemName))
                        interactPrompt.SetText("Ouvrir la porte (F)");
                    else
                        interactPrompt.SetText("Il vous faut un badge");
                }
                // Sinon vérifie si c'est un objet ramassable
                else if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
                {
                    interactPrompt.SetText("Ramasser (F)");
                }
            }
            else
            {
                interactPrompt.Hide();
            }
        }

        // Interaction par F
        if (hitInteractable && Input.GetKeyDown(KeyCode.F))
        {
            // Vérifie d'abord si c'est un lecteur de carte
            if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
            {
                // Le CardReader gère lui-même l'interaction
                return;
            }
            // Sinon vérifie si c'est un objet ramassable
            else if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
            {
                if (inventory.Add(holder.itemData))
                {
                    Destroy(holder.gameObject);

                    // Ajoute ici : si c’est la torche, on joue le son
                    if (holder.itemData.itemName == "Flashlight")
                    {
                        // Recherche le GameObject "Flashlight" dans la scène
                        var fl = FindObjectOfType<FlashlightController>();
                        if (fl != null)
                            fl.OnEquip(); // joue le equipSound
                    }
                }
            }
        }
    }
}
