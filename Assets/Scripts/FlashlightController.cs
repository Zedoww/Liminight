using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] Light spot;
    [SerializeField] Inventory inventory;

    [Header("Audio")]
    [SerializeField] AudioClip equipSound;
    [SerializeField] AudioClip toggleSound;

    [Header("Flicker Settings")]
    [SerializeField] float flickerChancePerSecond = 0.1f;
    [SerializeField] float minFlickerDuration = 0.05f;
    [SerializeField] float maxFlickerDuration = 0.2f;

    private AudioSource audioSource;
    private bool isOn = false;
    private bool isFlickering = false;
    private float flickerTimer = 0f;

    void Start()
    {
        if (spot != null)
            spot.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // Son 2D
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

        // Fluctuation subtile réaliste de l’intensité
        if (isOn && spot != null && !isFlickering)
        {
            float fluctuation = Mathf.PerlinNoise(Time.time * 3f, 0f) * 0.1f + 0.95f;
            spot.intensity = 3f * fluctuation;
        }

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
