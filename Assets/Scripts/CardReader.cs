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

    [Header("Feedback")]
    [SerializeField] private AudioClip accessGrantedSound;
    [SerializeField] private AudioClip accessDeniedSound;
    [SerializeField] private GameObject successEffect;
    [SerializeField] public GameObject RedLight;
    [SerializeField] public GameObject GreenLight;

    private AudioSource audioSource;
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;
    private InteractBehavior interactBehavior;
    private bool inputEnabled = true;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    void Start()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        interactBehavior = FindFirstObjectByType<InteractBehavior>();

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (successEffect != null)
            successEffect.SetActive(false);

        if (RedLight != null) RedLight.SetActive(true);
        if (GreenLight != null) GreenLight.SetActive(false);

        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
    }

    public void EnableInput(bool enable)
    {
        inputEnabled = enable;
    }

    void Update()
    {
        if (isActivated || !inputEnabled)
            return;

        bool isAnyMenuOpen = false;

        if (pauseMenuManager != null)
            isAnyMenuOpen = pauseMenuManager.IsOpen();
        if (inventoryUI != null && !isAnyMenuOpen)
            isAnyMenuOpen = inventoryUI.IsOpen();
        if (isAnyMenuOpen)
            return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.gameObject == gameObject && Input.GetKeyDown(KeyCode.F))
            {
                bool hasRequiredItem = playerInventory.Has(requiredItemName);
                if (hasRequiredItem)
                {
                    ActivateReader();
                }
                else
                {
                    if (accessDeniedSound != null && audioSource != null)
                        audioSource.PlayOneShot(accessDeniedSound);

                    if (interactBehavior != null)
                        interactBehavior.ShowTemporaryMessage("Access Denied - Card Required", 2f);
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

        if (accessGrantedSound != null && audioSource != null)
            audioSource.PlayOneShot(accessGrantedSound);

        if (successEffect != null)
        {
            successEffect.SetActive(true);
            Invoke("HideEffect", 3f);
        }

        if (RedLight != null) RedLight.SetActive(false);
        if (GreenLight != null) GreenLight.SetActive(true);

        if (unlockOnly)
        {
            doorToUnlock.Unlock();
            Debug.Log("Porte déverrouillée ! Utilise le clic gauche pour l'ouvrir.");
        }
        else
        {
            doorToUnlock.Unlock();
            doorToUnlock.TryToggleDoor();
        }
    }

    void HideEffect()
    {
        if (successEffect != null)
            successEffect.SetActive(false);
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}