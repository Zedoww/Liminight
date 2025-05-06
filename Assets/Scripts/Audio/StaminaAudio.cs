using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class StaminaAudio : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip normalBreath;
    [SerializeField] private AudioClip exhaustedBreath;

    [Header("Timing")]
    [Tooltip("Temps min avant un prochain souffle normal")]
    [SerializeField] private float minBreathDelay = 5f;
    [Tooltip("Temps max avant un prochain souffle normal")]
    [SerializeField] private float maxBreathDelay = 10f;

    private AudioSource audioSource;
    private PlayerController player;
    private float lastBreathTime;
    private float nextBreathDelay;
    private bool wasSprintingLastFrame;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
        audioSource.loop = false;

        lastBreathTime = Time.time;
        nextBreathDelay = Random.Range(minBreathDelay, maxBreathDelay);
    }

    void Update()
    {
        float ratio = player.currentStamina / player.maxStamina;
        bool sprinting = player.isSprinting;
        bool exhausted = player.isExhausted;

        // 1) Essoufflement immédiat
        if (exhausted && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(exhaustedBreath);
            lastBreathTime = Time.time;
            nextBreathDelay = Random.Range(minBreathDelay, maxBreathDelay);
            wasSprintingLastFrame = sprinting;
            return;
        }

        // 2) Souffle normal après un sprint, au repos
        if (!sprinting && !exhausted && wasSprintingLastFrame)
        {
            if (Time.time > lastBreathTime + nextBreathDelay)
            {
                audioSource.PlayOneShot(normalBreath);
                lastBreathTime = Time.time;
                nextBreathDelay = Random.Range(minBreathDelay, maxBreathDelay);
            }
        }

        wasSprintingLastFrame = sprinting;
    }
}
