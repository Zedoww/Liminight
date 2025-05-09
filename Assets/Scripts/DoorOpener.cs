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
    public bool isLockedByDefault = true;
    [Tooltip("ID unique de la porte pour l'associer à un lecteur de badge")]
    public string doorID = "door01";
    [Tooltip("Distance maximale à laquelle le joueur peut interagir avec la porte")]
    public float maxInteractionDistance = 3f;

    private bool isOpen;
    private bool isAnimating;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isLocked;

    private AudioSource audioSource;

    // Référence au collider pour les interactions
    private Collider doorCollider;
    private Transform playerTransform;

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
            Debug.Log("DoorOpener: BoxCollider ajouté automatiquement.");
        }
        
        // Trouver la caméra du joueur pour les vérifications de distance
        playerTransform = Camera.main.transform;
    }

    private void Update()
    {
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
                    TryToggleDoor();
                }
                else
                {
                    Debug.Log("Trop loin pour interagir avec la porte.");
                }
            }
        }
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
            // Jouer un son de porte verrouillée ou feedback visuel
            Debug.Log("Cette porte est verrouillée.");
            // Tu peux ajouter ici un son de porte verrouillée
            return;
        }
        
        StartCoroutine(AnimateDoor());
    }

    // Méthode appelée par le lecteur de badge pour déverrouiller
    public void Unlock()
    {
        isLocked = false;
        // Ajouter ici du son/feedback de déverrouillage
        Debug.Log("Porte déverrouillée: " + doorID);
    }
    
    // Pour verrouiller à nouveau
    public void Lock()
    {
        isLocked = true;
        Debug.Log("Porte verrouillée: " + doorID);
    }
    
    // Pour vérifier l'état actuel
    public bool IsLocked()
    {
        return isLocked;
    }
}