using UnityEngine;
using System.Collections;

public class PNJScareTrigger : MonoBehaviour
{
    [Header("PNJ Settings")]
    [Tooltip("The PNJ GameObject that will disappear.")]
    public GameObject pnjToDisappear;

    [Header("Light Settings")]
    [Tooltip("The lights in the corridor to be turned off temporarily.")]
    public Light[] corridorLights;
    [Tooltip("Duration for which the lights will be turned off.")]
    public float lightsOffDuration = 0.25f;

    [Header("Sound Settings")]
    [Tooltip("The sound effect to play when the PNJ disappears.")]
    public AudioClip scareSound;
    [Range(0f, 1f)]
    [Tooltip("Volume du jumpscare (0 = muet, 1 = max)")]
    public float scareVolume = 1f;
    private AudioSource audioSource;

    private bool triggered = false;

    void Awake()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // Prevent re-triggering

        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            triggered = true;
            PerformScare();
        }
    }

    void PerformScare()
    {
        // Make the PNJ disappear
        if (pnjToDisappear != null)
        {
            pnjToDisappear.SetActive(false);
        }

        // Play the scare sound
        if (scareSound != null && audioSource != null)
        {
            audioSource.volume = scareVolume;
            audioSource.PlayOneShot(scareSound);
        }

        // Turn off lights and start coroutine to turn them back on
        if (corridorLights != null && corridorLights.Length > 0)
        {
            StartCoroutine(FlickerLights());
        }

        // Optional: Disable this script/trigger after it has been activated once
        // gameObject.SetActive(false); // Or just this.enabled = false;
    }

    IEnumerator FlickerLights()
    {
        // Turn off all lights
        foreach (Light light in corridorLights)
        {
            if (light != null)
            {
                light.enabled = false;
            }
        }

        // Wait for the specified duration
        yield return new WaitForSeconds(lightsOffDuration);

        // Turn all lights back on
        foreach (Light light in corridorLights)
        {
            if (light != null)
            {
                light.enabled = true;
            }
        }
    }
} 