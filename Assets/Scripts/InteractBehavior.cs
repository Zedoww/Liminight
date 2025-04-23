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

        // Interaction par F
        if (hitInteractable && Input.GetKeyDown(KeyCode.F))
        {
            if (hit.collider.TryGetComponent<ItemDataHolder>(out var holder))
            {
                if (inventory.Add(holder.itemData))
                {
                    Destroy(holder.gameObject);
                }
            }
        }


        if (interactPrompt != null)
        {
            if (hitInteractable)
                interactPrompt.Show();
            else
                interactPrompt.Hide();
        }
    }
}
