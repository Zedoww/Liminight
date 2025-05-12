using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public CharacterController controller;

    [Header("Head Movement")]
    public float bobAmplitude = 0.08f;
    public float bobSpeedFactor = 4f;
    public float bobSideAmplitude = 0.04f;
    public float idleBobAmplitude = 0.005f;
    public float idleBobSpeed = 1f;
    public float sprintBobMultiplier = 1.5f;

    [Header("Footstep Sound")]
    [SerializeField] AudioClip footstepSound;
    [SerializeField] float footstepInterval = 0.45f;
    [SerializeField] float pitchVariation = 0.05f;
    [SerializeField] float volumeMin = 0.8f;
    [SerializeField] float volumeMax = 1.0f;

    private float bobTimer = 0f;
    private float lastStepTime = 0f;
    private Vector3 initialPos;
    private AudioSource audioSource;

    void Start()
    {
        initialPos = transform.localPosition;

        // Ajout d'un AudioSource si nécessaire
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f; // 3D
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
        audioSource.dopplerLevel = 0f;
    }

    void Update()
    {
        if (controller == null) return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;
        float speed = velocity.magnitude;

        float targetAmplitude = 0f;
        float targetSideAmplitude = 0f;
        float targetSpeed = 0f;

        if (controller.isGrounded)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float sprintFactor = isSprinting ? sprintBobMultiplier : 1f;

            if (speed > 0.1f)
            {
                targetAmplitude = bobAmplitude * sprintFactor;
                targetSideAmplitude = bobSideAmplitude * sprintFactor;
                targetSpeed = speed * bobSpeedFactor * sprintFactor;
            }
            else
            {
                targetAmplitude = idleBobAmplitude;
                targetSideAmplitude = 0f;
                targetSpeed = idleBobSpeed;
            }

            bobTimer += Time.deltaTime * targetSpeed;

            float offsetY = Mathf.Sin(bobTimer) * targetAmplitude;
            float offsetX = Mathf.Cos(bobTimer * 2f) * targetSideAmplitude;

            // Déclenche le son de pas sur la descente
            if (Time.time - lastStepTime > footstepInterval &&
                Mathf.Sin(bobTimer) < -0.95f &&
                speed > 0.1f)
            {
                PlayFootstepSound();
                lastStepTime = Time.time;
            }

            Vector3 targetPosition = initialPos + new Vector3(offsetX, offsetY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 10f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos, Time.deltaTime * 5f);
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSound == null) return;

        audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.PlayOneShot(footstepSound);
    }
}
