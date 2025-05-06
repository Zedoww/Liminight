using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class HeadBobFootstep : MonoBehaviour
{
    /* --- Références --- */
    public CharacterController controller;
    public Transform cameraTransform;
    PlayerController player;

    /* --- HeadBob --- */
    public float bobAmplitude = 0.06f;
    public float bobSpeedFactor = 4f;
    public float sideAmplitude = 0.04f;
    public float idleAmplitude = 0.004f;
    public float idleSpeed = 1.2f;

    /* --- Clips Walk / Run --- */
    [Header("Clips pas")]
    public AudioClip[] footstepClipsWalk;
    public AudioClip[] footstepClipsRun;
    public float runPitchBoost = 0.10f;   // +10 % pitch quand on court
    public float runVolumeBoost = 0.10f;   // +10 % volume

    /* --- Audio & Timing --- */
    public float footstepInterval = 0.5f;
    public float speedThreshold = 0.1f;
    public float pitchVariation = 0.05f;
    public float volumeMin = 0.8f;
    public float volumeMax = 1f;

    /* --- internes --- */
    float bobTimer;
    Vector3 initialCamPos;
    AudioSource footstepSource;
    bool canStep = true;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        player = GetComponent<PlayerController>();

        initialCamPos = cameraTransform.localPosition;

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.spatialBlend = 1f;
        footstepSource.rolloffMode = AudioRolloffMode.Linear;
        footstepSource.minDistance = 1f;
        footstepSource.maxDistance = 10f;
        footstepSource.dopplerLevel = 0f;
    }

    void Update()
    {
        if (!controller.isGrounded) return;

        Vector3 vel = controller.velocity; vel.y = 0f;
        float speed = vel.magnitude;
        bool moving = speed > speedThreshold;

        float freq = moving ? speed * bobSpeedFactor : idleSpeed;
        float ampY = moving ? bobAmplitude : idleAmplitude;
        float ampX = moving ? sideAmplitude : 0f;

        bobTimer += Time.deltaTime * freq;

        float yOff = Mathf.Sin(bobTimer) * ampY;
        float xOff = Mathf.Cos(bobTimer * 2) * ampX;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            initialCamPos + new Vector3(xOff, yOff, 0f),
            Time.deltaTime * 12f
        );

        if (moving && Mathf.Sin(bobTimer) < -0.95f && canStep)
        {
            PlayFootstep();
            canStep = false;
            Invoke(nameof(ResetStep), footstepInterval);
        }
    }

    void PlayFootstep()
    {
        bool running = player != null && player.isSprinting;
        AudioClip[] bank = running ? footstepClipsRun : footstepClipsWalk;
        if (bank == null || bank.Length == 0) return;

        AudioClip clip = bank[Random.Range(0, bank.Length)];

        float basePitch = running ? 1f + runPitchBoost : 1f;
        float baseVolume = running ? volumeMax + runVolumeBoost : volumeMax;

        footstepSource.pitch = Random.Range(basePitch - pitchVariation,
                                             basePitch + pitchVariation);
        footstepSource.volume = Random.Range(volumeMin, baseVolume);
        footstepSource.PlayOneShot(clip);
    }

    void ResetStep() => canStep = true;
}
