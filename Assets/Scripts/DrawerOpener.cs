using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DrawerOpener : MonoBehaviour
{
    /* ─────────────── Animation ─────────────── */
    [Header("Animation")]
    [Tooltip("The offset from the closed position to the open position, in local space.")]
    public Vector3 openOffset = new Vector3(0, 0, 0.5f); // Default to move 0.5 units along local Z-axis
    [Tooltip("Speed of the opening/closing animation.")]
    public float speed = 2f;    // Lerp speed

    /* ─────────────── Audio ─────────────── */
    [Header("Audio Settings")]
    [SerializeField] private AudioClip openAudio;     // Sound for opening
    [SerializeField] private AudioClip closeAudio;    // Sound for closing
    [SerializeField] private AudioClip lockedSound;   // Sound for locked drawer
    [SerializeField, Range(0f, .2f)] private float pitchVariation = 0.05f;
    [SerializeField, Range(0f, 1f)]  private float volumeMin = 0.8f;
    [SerializeField, Range(0f, 1f)]  private float volumeMax = 1f;

    /* ─────────────── State / ID ─────────────── */
    [Header("Drawer State")]
    [Tooltip("If the drawer is locked by default.")]
    public bool isLockedByDefault = false;
    [Tooltip("Unique ID for the drawer (e.g., for a DrawerLock system).")]
    public string drawerID;
    [Tooltip("Maximum distance for interaction.")]
    public float maxInteractionDistance = 2f;
    [Tooltip("Duration to display the 'locked' message.")]
    [SerializeField] private float lockedMessageDuration = 2f;

    /* ─────────────── Runtime state ─────────────── */
    private bool isOpen;
    private bool isAnimating;
    private bool isLocked;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    /* ─────────────── Références ─────────────── */
    // private DrawerLock drawerLock; // Placeholder for a potential DrawerLock component
    private AudioSource audioSource;
    private Collider drawerCollider;
    private Transform playerTransform;
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;
    private Inventory playerInventory;
    private InteractBehavior interactBehavior; // To show messages like "Drawer is locked"

    /* ─────────────── Input flag ─────────────── */
    private bool inputEnabled = true;

    /*──────────────────────────────────────────*/

    /* ---------- Awake ---------- */
    private void Awake()
    {
        // Store initial local position as closed, calculate open position
        closedPosition = transform.localPosition;
        openPosition   = closedPosition + openOffset;

        // AudioSource is added by RequireComponent
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource(audioSource);

        isLocked = isLockedByDefault;

        // Collider for click interaction
        drawerCollider = GetComponent<Collider>();
        if (drawerCollider == null)
        {
            Debug.LogWarning($"DrawerOpener on {gameObject.name} is missing a Collider. Adding a BoxCollider by default. Please configure it correctly.");
            drawerCollider = gameObject.AddComponent<BoxCollider>();
        }

        if (Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError($"Main Camera not found. DrawerOpener on {gameObject.name} needs a tagged 'MainCamera' for distance checks.");
            inputEnabled = false; // Disable interaction if no camera
        }

        // drawerLock = GetComponent<DrawerLock>(); // Example: If you create a DrawerLock system
    }

    /* ---------- Start ---------- */
    private void Start()
    {
        // Find menu managers
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI      = FindFirstObjectByType<InventoryUI>();

        // Find InteractBehavior on the player (or a central game manager)
        interactBehavior = FindFirstObjectByType<InteractBehavior>();
        if (interactBehavior == null)
        {
            Debug.LogWarning($"InteractBehavior not found in scene. DrawerOpener on {gameObject.name} will not be able to display messages or check inventory for locks.");
        }
        else
        {
            playerInventory = interactBehavior.inventory; // Assuming InteractBehavior has a reference to the player's inventory
        }
    }

    /* ---------- Update ---------- */
    private void Update()
    {
        if (!inputEnabled || playerTransform == null) return;

        // Block interaction if a menu is open
        bool anyMenuOpen = (pauseMenuManager != null && pauseMenuManager.IsOpen()) ||
                           (inventoryUI      != null && inventoryUI.IsOpen());
        if (anyMenuOpen) return;

        // Handle mouse click for interaction
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider == drawerCollider)
            {
                // Check distance from player to drawer
                if (Vector3.Distance(hit.point, playerTransform.position) <= maxInteractionDistance)
                {
                    HandleDrawerInteraction();
                }
            }
        }
    }

    /* ---------- Interaction Logic ---------- */
    private void HandleDrawerInteraction()
    {
        // TODO: Implement DrawerLock interaction if needed, similar to DoorOpener
        // if (drawerLock != null)
        // {
        //     drawerLock.HandleInteraction(playerInventory);
        // }
        // else
        // {
            TryToggleDrawer();
        // }
    }

    /* ---------- Public Methods for Lock State ---------- */
    public void EnableInput(bool enable) => inputEnabled = enable;
    public void Unlock()  => isLocked = false;
    public void Lock()    => isLocked = true;
    public bool IsLocked() => isLocked;
    public bool IsOpen() => isOpen;


    /* ---------- Attempt to Open/Close Drawer ---------- */
    public void TryToggleDrawer()
    {
        if (isLocked)
        {
            PlaySound(lockedSound);

            if (interactBehavior != null)
            {
                interactBehavior.ShowTemporaryMessage("This drawer is locked", lockedMessageDuration);
            }
            else
            {
                Debug.LogWarning($"InteractBehavior reference missing on DrawerOpener {gameObject.name}, cannot show 'locked' message.");
            }
            return; // Do not animate if locked
        }

        // If not locked, proceed to animate
        StartCoroutine(AnimateDrawer());
    }

    /* ---------- Animation Coroutine ---------- */
    public IEnumerator AnimateDrawer()
    {
        isAnimating = true;
        bool opening = !isOpen; // Determine if we are opening or closing

        PlaySound(opening ? openAudio : closeAudio);

        Vector3 startPosition  = transform.localPosition;
        Vector3 targetPosition = isOpen ? closedPosition : openPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Ensure final position is exact
        transform.localPosition = targetPosition;

        isOpen      = !isOpen;
        isAnimating = false;
    }

    /* ---------- Sound Playback ---------- */
    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.pitch  = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.PlayOneShot(clip);
    }

    /* ---------- AudioSource Configuration ---------- */
    private static void ConfigureAudioSource(AudioSource src)
    {
        src.spatialBlend = 1f;        // 3D sound
        src.rolloffMode  = AudioRolloffMode.Linear;
        src.minDistance  = 0.5f;      // Min distance for hearing sound at full volume
        src.maxDistance  = 5f;       // Max distance at which sound can be heard
        src.dopplerLevel = 0f;        // No doppler effect
        src.playOnAwake  = false;     // Don't play sound on start
    }
} 