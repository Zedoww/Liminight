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
    }

    void Update()
    {
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

                if (hit.collider.TryGetComponent<CardReader>(out var cardReader))
                {
                    if (inventory.Has(cardReader.RequiredItemName))
                        interactPrompt.SetText("Ouvrir la porte (F)");
                    else
                        interactPrompt.SetText("Il vous faut un badge");
                }
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
