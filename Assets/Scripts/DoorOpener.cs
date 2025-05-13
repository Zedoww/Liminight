using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DoorOpener : MonoBehaviour
{
    /* ─────────────── Animation ─────────────── */
    [Header("Animation")]
    public float openAngle = -90f;   // Y-axis angle fully opened
    public float speed      = 2f;    // Lerp speed

    /* ─────────────── Audio ─────────────── */
    [Header("Squeak sound")]
    [SerializeField] private AudioClip openAudio;     // Son d’ouverture
    [SerializeField] private AudioClip closeAudio;    // Son de fermeture
    [SerializeField] private AudioClip lockedSound;   // Son porte verrouillée
    [SerializeField, Range(0f, .2f)] private float pitchVariation = 0.05f;
    [SerializeField, Range(0f, 1f)]  private float volumeMin       = 0.8f;
    [SerializeField, Range(0f, 1f)]  private float volumeMax       = 1f;

    /* ─────────────── État / ID ─────────────── */
    [Header("État Porte")]
    [Tooltip("Si la porte est verrouillée au démarrage")]
    public bool isLockedByDefault = false;
    [Tooltip("ID unique de la porte (pour CardReader)")]
    public string doorID;
    [Tooltip("Distance max d’interaction")]
    public float maxInteractionDistance = 3f;
    [Tooltip("Durée d’affichage du message « porte verrouillée »")]
    [SerializeField] private float lockedMessageDuration = 2f;

    /* ─────────────── Runtime state ─────────────── */
    private bool isOpen;
    private bool isAnimating;
    private bool isLocked;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    /* ─────────────── Références ─────────────── */
    private DoorLock doorLock;
    private AudioSource audioSource;
    private Collider doorCollider;
    private Transform playerTransform;
    private PauseMenuManager pauseMenuManager;
    private InventoryUI inventoryUI;
    private Inventory playerInventory;
    private InteractBehavior interactBehavior;

    /* ─────────────── Input flag ─────────────── */
    private bool inputEnabled = true;

    /*──────────────────────────────────────────*/

    /* ---------- Awake ---------- */
    private void Awake()
    {
        // Cache rotations
        closedRotation = transform.rotation;
        openRotation   = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;

        // AudioSource déjà ajouté par RequireComponent
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource(audioSource);

        isLocked = isLockedByDefault;

        // Collider pour le click
        doorCollider = GetComponent<Collider>() ?? gameObject.AddComponent<BoxCollider>();

        // Caméra joueur (pour distance)
        playerTransform = Camera.main.transform;

        // DoorLock éventuel
        doorLock = GetComponent<DoorLock>();
    }

    /* ---------- Start ---------- */
    private void Start()
    {
        // Menus
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();
        inventoryUI      = FindFirstObjectByType<InventoryUI>();

        // InteractBehavior (joueur)
        interactBehavior = FindFirstObjectByType<InteractBehavior>();
        if (interactBehavior == null)
        {
            Debug.LogError($"InteractBehavior not found in scene for DoorOpener on {gameObject.name}");
        }
        else
        {
            playerInventory = interactBehavior.inventory;
        }
    }

    /* ---------- Update ---------- */
    private void Update()
    {
        if (!inputEnabled) return;

        // Bloquer si un menu est ouvert
        bool anyMenuOpen = (pauseMenuManager != null && pauseMenuManager.IsOpen()) ||
                           (inventoryUI      != null && inventoryUI.IsOpen());
        if (anyMenuOpen) return;

        // Clic gauche ?
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider == doorCollider)
            {
                // Vérifier distance joueur->porte
                if (Vector3.Distance(hit.point, playerTransform.position) <= maxInteractionDistance)
                {
                    HandleDoorInteraction();
                }
            }
        }
    }

    /* ---------- Interaction ---------- */
    private void HandleDoorInteraction()
    {
        // Si DoorLock : déléguer
        if (doorLock != null)
        {
            doorLock.HandleInteraction(playerInventory);
        }
        else
        {
            // Portes simples
            TryToggleDoor();
        }
    }

    /* ---------- Public utils ---------- */
    public void EnableInput(bool enable) => inputEnabled = enable;

    public void Unlock()  => isLocked = false;
    public void Lock()    => isLocked = true;
    public bool IsLocked() => isLocked;

    /* ---------- Porte verrouillée ou déplacement ---------- */
    public void TryToggleDoor()
    {
        if (isLocked)
        {
            // ▶ Son « verrouillé »
            if (lockedSound != null && audioSource != null)
            {
                audioSource.pitch  = Random.Range(1f - pitchVariation, 1f + pitchVariation);
                audioSource.volume = Random.Range(volumeMin, volumeMax);
                audioSource.PlayOneShot(lockedSound);
            }

            // ▶ Message à l’écran
            if (interactBehavior != null)
            {
                interactBehavior.ShowTemporaryMessage("This door is locked", lockedMessageDuration);
            }
            else
            {
                Debug.LogError($"InteractBehavior reference missing on DoorOpener {gameObject.name}");
            }

            return; // Pas d’animation
        }

        // Porte déverrouillée → animation
        StartCoroutine(AnimateDoor());
    }

    /* ---------- Animation coroutine ---------- */
    public IEnumerator AnimateDoor()
    {
        isAnimating = true;

        bool opening = !isOpen;
        PlayDoorSound(opening);

        Quaternion startRotation  = transform.rotation;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        isOpen      = !isOpen;
        isAnimating = false;
    }

    /* ---------- Sounds ---------- */
    private void PlayDoorSound(bool opening)
    {
        AudioClip clip = opening ? openAudio : closeAudio;

        if (clip == null)            clip = opening ? closeAudio : openAudio;
        if (clip == null)            return;

        audioSource.pitch  = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.PlayOneShot(clip);
    }

    private static void ConfigureAudioSource(AudioSource src)
    {
        src.spatialBlend = 1f;
        src.rolloffMode  = AudioRolloffMode.Linear;
        src.minDistance  = 1f;
        src.maxDistance  = 10f;
        src.dopplerLevel = 0f;
        src.playOnAwake  = false;
    }
}