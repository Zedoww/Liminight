using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip soundToPlay;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] [Tooltip("True = jouer une seule fois")] private bool playOnce = false;

    [Header("Collision Settings")]
    [SerializeField] [Tooltip("Tag de l'objet qui peut déclencher le son")] private string triggerTag = "Player";
    [SerializeField] private bool checkTag = true;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Start()
    {
        // Ajouter un AudioSource s'il n'existe pas déjà
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet qui entre a le bon tag (si la vérification est activée)
        if (checkTag && !other.CompareTag(triggerTag))
            return;

        // Vérifier si on a déjà joué le son une fois (si playOnce est activé)
        if (playOnce && hasPlayed)
            return;

        // Jouer le son
        if (soundToPlay != null)
        {
            audioSource.clip = soundToPlay;
            audioSource.volume = volume;
            audioSource.Play();
            hasPlayed = true;
        }
        else
        {
            Debug.LogWarning("Aucun son n'a été assigné à " + gameObject.name);
        }
    }
} 