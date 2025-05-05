using UnityEngine;

public class HeadBobFootstep : MonoBehaviour
{
    [Header("Références")]
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("HeadBob")]
    public float bobAmplitude = 0.06f;
    public float bobSpeedFactor = 4f;
    public float sideAmplitude = 0.04f;
    public float idleAmplitude = 0.004f;
    public float idleSpeed = 1.2f;

    [Header("Pas réalistes")]
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.5f;
    public float speedThreshold = 0.1f;
    public float pitchVariation = 0.05f;
    public float volumeMin = 0.8f;
    public float volumeMax = 1.0f;

    private float bobTimer = 0f;
    private float lastStepTime = 0f;
    private Vector3 initialCamPos;
    private AudioSource footstepSource;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

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
        if (controller == null || !controller.isGrounded) return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;
        float speed = velocity.magnitude;

        bool isMoving = speed > speedThreshold;

        float bobFreq = isMoving ? speed * bobSpeedFactor : idleSpeed;
        float ampY = isMoving ? bobAmplitude : idleAmplitude;
        float ampX = isMoving ? sideAmplitude : 0f;

        bobTimer += Time.deltaTime * bobFreq;

        float offsetY = Mathf.Sin(bobTimer) * ampY;
        float offsetX = Mathf.Cos(bobTimer * 2f) * ampX;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCamPos + new Vector3(offsetX, offsetY, 0f), Time.deltaTime * 12f);

        if (isMoving && Mathf.Sin(bobTimer) < -0.95f && Time.time - lastStepTime >= footstepInterval)
        {
            PlayFootstep();
            lastStepTime = Time.time;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0)
        {
            Debug.LogWarning("Aucun AudioClip assigné !");
            return;
        }

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        footstepSource.volume = Random.Range(volumeMin, volumeMax);
        footstepSource.PlayOneShot(clip);

        Debug.Log("Pas joué : " + clip.name);
    }
}
