using UnityEngine;

public class CardReader : MonoBehaviour
{
    [SerializeField] private DoorOpener doorToOpen;
    [SerializeField] private string requiredItemName = "IDCard";
    public string RequiredItemName => requiredItemName;

    private bool isOpen = false;
    private Inventory playerInventory;

    void Awake()
    {
        // S'assure que le CardReader est sur le layer "Interactable"
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    void Start()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        if (isOpen) return;

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
                    // Vérifie l'input pour ouvrir la porte
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        OpenDoor();
                    }
                }
            }
        }
    }

    void OpenDoor()
    {
        if (!isOpen && doorToOpen != null)
        {
            isOpen = true;
            doorToOpen.Toggle();
            Debug.Log("Porte ouverte");
        }
    }
} 