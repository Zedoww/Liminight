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
    public float openAngle = 90f;          // Y-axis angle when fully opened
    public float speed = 2f;               // Lerp speed

    [Header("Squeak sound")]
    [SerializeField] private AudioClip squeakAudio;
    [SerializeField, Range(0f, .2f)] private float pitchVariation = 0.05f;
    [SerializeField, Range(0f, 1f)] private float volumeMin = 0.8f;
    [SerializeField, Range(0f, 1f)] private float volumeMax = 1f;

    private bool isOpen;
    private bool isAnimating;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private AudioSource audioSource;

    private void Awake()
    {
        // Cache rotations
        closedRotation = transform.rotation;
        openRotation   = Quaternion.Euler(0f, openAngle, 0f) * closedRotation;

        // Ensure we have an AudioSource and configure it like in HeadBob
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource(audioSource);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
            StartCoroutine(AnimateDoor());
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

    /// <summary>External method for UI / triggers.</summary>
    public void Toggle() => StartCoroutine(AnimateDoor());
}