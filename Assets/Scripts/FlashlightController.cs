using UnityEngine;

public class FlashlightController : MonoBehaviour, IEquipable, IUsable
{
    [SerializeField] Light spot;
    [SerializeField] Transform visualModel;
    [SerializeField] Transform spotTransform;

    [Header("Audio")]
    [SerializeField] AudioClip equipSound;
    [SerializeField] AudioClip toggleSound;
    AudioSource audioSource;

    [Header("Oscillation")]
    public float baseSwayAmount = 0.002f;
    public float maxSwayAmount = 0.015f;
    public float swaySpeed = 2f;

    private CharacterController playerController;
    private Vector3 initialLocalPos;
    private float swayTimer = 0f;
    private bool isOn = false;
    private Vector3 currentOffset = Vector3.zero;

    [Header("Flicker Settings")]
    [SerializeField] float flickerChancePerSecond = 0.1f; // probabilité de clignoter par seconde
    [SerializeField] float minFlickerDuration = 0.05f;
    [SerializeField] float maxFlickerDuration = 0.2f;

    private float flickerTimer = 0f;
    private bool isFlickering = false;



    void Awake()
    {
        if (spot != null) spot.enabled = false;
        if (visualModel == null) visualModel = transform.Find("Body");
        if (visualModel != null) initialLocalPos = visualModel.localPosition;
        playerController = FindFirstObjectByType<CharacterController>();
        if (spot != null) spotTransform = spot.transform;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnEquip()
    {
        spot.enabled = isOn;
        if (equipSound) audioSource.PlayOneShot(equipSound);
    }

    public void OnUnequip()
    {
        spot.enabled = false;
    }

    public void Use()
    {
        isOn = !isOn;
        spot.enabled = isOn;
        if (toggleSound) audioSource.PlayOneShot(toggleSound);
    }

    void Update()
    {
        if (visualModel == null || playerController == null) return;

        Vector3 velocity = playerController.velocity;
        velocity.y = 0f;
        float speed = velocity.magnitude;

        float swayAmount = Mathf.Lerp(baseSwayAmount, maxSwayAmount, speed / 4f);
        float swaySpeedCurrent = Mathf.Lerp(0.5f, swaySpeed, speed / 4f);
        swayTimer += Time.deltaTime * swaySpeedCurrent;

        Vector3 targetOffset = new Vector3(
            Mathf.Sin(swayTimer * 1.2f) * swayAmount,
            Mathf.Cos(swayTimer * 2f) * swayAmount * 0.5f,
            0f
        );

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * 2f);
        visualModel.localPosition = initialLocalPos + currentOffset;

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
    }


}
