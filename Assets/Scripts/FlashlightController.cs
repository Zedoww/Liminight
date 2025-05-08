using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] Light spot;
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerController playerController;

    [Header("Audio")]
    [SerializeField] AudioClip equipSound;
    [SerializeField] AudioClip toggleSound;

    [Header("Flicker Settings")]
    [SerializeField] float flickerChancePerSecond = 0.1f;
    [SerializeField] float minFlickerDuration = 0.05f;
    [SerializeField] float maxFlickerDuration = 0.2f;

    [Header("Mouvement")]
    [Tooltip("Amplitude du mouvement vertical (en idle)")]
    [SerializeField] float swayAmplitude = 0.05f;
    [Tooltip("Vitesse du mouvement vertical")]
    [SerializeField] float swaySpeed = 6f;
    [Tooltip("Amplitude du mouvement latéral")]
    [SerializeField] float sideSwayAmplitude = 0.03f;
    [Tooltip("Multiplicateur d'amplitude quand le joueur est essoufflé")]
    [SerializeField] float exhaustedSwayMultiplier = 2f;
    [Tooltip("Décalage de temps pour un mouvement plus naturel")]
    [SerializeField] float swayLag = 0.1f;
    [Tooltip("Intensité du retard entre les mouvements de la tête et de la lampe")]
    [SerializeField] float lagAmount = 1f;
    [Tooltip("Vitesse de retour à la position centrale")]
    [SerializeField] float returnSpeed = 1f;

    private AudioSource audioSource;
    private bool isOn = false;
    private bool isFlickering = false;
    private float flickerTimer = 0f;
    
    // Variables pour le mouvement
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 previousPlayerPosition;
    private Vector3 swayVelocity;
    private Vector3 targetPosition;
    private float bobTimer = 0f;

    void Start()
    {
        if (spot != null)
            spot.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // Son 2D
        
        // Sauvegarde de la position et rotation initiales
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        
        // Trouver le PlayerController si non assigné
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
            
        if (playerController != null)
            previousPlayerPosition = playerController.transform.position;
    }

    void Update()
    {
        // Activation par touche T
        if (Input.GetKeyDown(KeyCode.T))
            TryToggle();

        // Gestion du clignotement (flicker)
        if (isOn && spot != null)
        {
            if (isFlickering)
            {
                flickerTimer -= Time.deltaTime;
                if (flickerTimer <= 0f)
                {
                    isFlickering = false;
                    spot.enabled = true;
                }
            }
            else
            {
                if (Random.value < flickerChancePerSecond * Time.deltaTime)
                {
                    isFlickering = true;
                    flickerTimer = Random.Range(minFlickerDuration, maxFlickerDuration);
                    spot.enabled = false;
                }
            }
        }

        // Fluctuation subtile réaliste de l'intensité
        if (isOn && spot != null && !isFlickering)
        {
            float fluctuation = Mathf.PerlinNoise(Time.time * 3f, 0f) * 0.1f + 0.95f;
            spot.intensity = 3f * fluctuation;
        }

        // Mouvement de la lampe torche
        UpdateTorchMovement();
    }

    void UpdateTorchMovement()
    {
        if (playerController == null) return;

        // Paramètres de base pour le mouvement
        float ampY = swayAmplitude;
        float ampX = sideSwayAmplitude;
        float speed = swaySpeed;

        // Appliquer l'amplification si le joueur est essoufflé
        if (playerController.isExhausted)
        {
            ampY *= exhaustedSwayMultiplier;
            ampX *= exhaustedSwayMultiplier;
            speed *= 1.5f; // Augmenter aussi la vitesse du mouvement
        }

        // Mouvement de base (idle)
        bobTimer += Time.deltaTime * speed;
        float idleY = Mathf.Sin(bobTimer) * ampY;
        float idleX = Mathf.Cos(bobTimer * 2f) * ampX;

        // Mouvement basé sur le déplacement du joueur
        Vector3 playerVelocity = Vector3.zero;
        if (Time.deltaTime > 0)
        {
            Vector3 playerMovement = playerController.transform.position - previousPlayerPosition;
            playerVelocity = playerMovement / Time.deltaTime;
            previousPlayerPosition = playerController.transform.position;
        }

        // Appliquer un décalage pour un effet plus naturel
        float lagFactor = Mathf.Exp(-lagAmount * Time.deltaTime);
        swayVelocity = Vector3.Lerp(swayVelocity, playerVelocity, 1 - lagFactor);

        // Calculer la position cible avec le mouvement idle et le mouvement du joueur
        Vector3 sway = new Vector3(
            idleX - swayVelocity.x * swayLag,
            idleY + swayVelocity.y * swayLag,
            -swayVelocity.z * swayLag
        );

        // Limiter l'amplitude du mouvement
        sway = Vector3.ClampMagnitude(sway, ampY * 2);

        // Appliquer un retour progressif vers la position initiale
        targetPosition = Vector3.Lerp(targetPosition, initialPosition + sway, Time.deltaTime * returnSpeed);

        // Appliquer la position finale
        transform.localPosition = targetPosition;
    }

    void TryToggle()
    {
        if (inventory == null)
        {
            return;
        }

        if (!inventory.Has("Flashlight"))
        {
            return;
        }

        isOn = !isOn;

        if (spot != null)
        {
            spot.enabled = isOn;
        }
        else
        {
        }

        if (toggleSound)
            audioSource.PlayOneShot(toggleSound);
    }

    public void OnEquip()
    {
        //if (equipSound)
            //audioSource.PlayOneShot(equipSound);
    }

    public void OnUnequip()
    {
        isOn = false;
        if (spot != null)
            spot.enabled = false;
    }
}
