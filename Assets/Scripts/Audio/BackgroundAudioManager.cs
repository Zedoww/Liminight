using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages background audio/music for the game
/// Can toggle audio on/off and change tracks based on game situations
/// </summary>
public class BackgroundAudioManager : MonoBehaviour
{
    public static BackgroundAudioManager Instance { get; private set; }

    [Header("Audio Tracks")]
    [SerializeField] private AudioClip defaultBackgroundTrack;
    [SerializeField] private List<AudioClip> alternativeBackgroundTracks = new List<AudioClip>();
    
    [Header("Audio Settings")]
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] [Range(0f, 10f)] private float fadeInDuration = 2f;
    [SerializeField] [Range(0f, 10f)] private float fadeOutDuration = 2f;
    [SerializeField] private bool playOnAwake = true;
    
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private bool isEnabled = true;
    private int currentTrackIndex = -1; // -1 means default track
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0f; // Start with volume at 0 for fade-in
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (playOnAwake)
        {
            PlayDefaultTrack();
        }
    }
    
    /// <summary>
    /// Play the default background track with fade-in
    /// </summary>
    public void PlayDefaultTrack()
    {
        if (!isEnabled || defaultBackgroundTrack == null) return;
        
        PlayTrack(defaultBackgroundTrack);
        currentTrackIndex = -1;
    }
    
    /// <summary>
    /// Play a specific track from the alternative tracks list by index
    /// </summary>
    public void PlayTrackByIndex(int index)
    {
        if (!isEnabled || index < 0 || index >= alternativeBackgroundTracks.Count) return;
        
        PlayTrack(alternativeBackgroundTracks[index]);
        currentTrackIndex = index;
    }
    
    /// <summary>
    /// Play a specific AudioClip with fade-in
    /// </summary>
    private void PlayTrack(AudioClip track)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // If currently playing a track, fade it out first
        if (audioSource.isPlaying)
        {
            StartCoroutine(FadeOutThenPlay(track));
        }
        else
        {
            audioSource.clip = track;
            audioSource.Play();
            fadeCoroutine = StartCoroutine(FadeVolume(0f, volume, fadeInDuration));
        }
    }
    
    /// <summary>
    /// Fade out the current track, then play the new one
    /// </summary>
    private IEnumerator FadeOutThenPlay(AudioClip nextTrack)
    {
        // Fade out current track
        fadeCoroutine = StartCoroutine(FadeVolume(audioSource.volume, 0f, fadeOutDuration));
        yield return new WaitForSeconds(fadeOutDuration);
        
        // Play new track
        audioSource.Stop();
        audioSource.clip = nextTrack;
        audioSource.Play();
        
        // Fade in new track
        fadeCoroutine = StartCoroutine(FadeVolume(0f, volume, fadeInDuration));
    }
    
    /// <summary>
    /// Fade audio volume from start to target over duration
    /// </summary>
    private IEnumerator FadeVolume(float startVolume, float targetVolume, float duration)
    {
        float timeElapsed = 0;
        audioSource.volume = startVolume;
        
        while (timeElapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        audioSource.volume = targetVolume;
    }
    
    /// <summary>
    /// Enable or disable background audio
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        
        if (!isEnabled && audioSource.isPlaying)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeVolume(audioSource.volume, 0f, fadeOutDuration));
        }
        else if (isEnabled && !audioSource.isPlaying && defaultBackgroundTrack != null)
        {
            PlayDefaultTrack();
        }
    }
    
    /// <summary>
    /// Toggle background audio on/off
    /// </summary>
    public void ToggleAudio()
    {
        SetEnabled(!isEnabled);
    }
    
    /// <summary>
    /// Set the volume of the background audio
    /// </summary>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        
        if (isEnabled && audioSource.isPlaying)
        {
            fadeCoroutine = StartCoroutine(FadeVolume(audioSource.volume, volume, fadeInDuration));
        }
    }
    
    /// <summary>
    /// Set the fade in/out durations for smoother transitions
    /// </summary>
    /// <param name="fadeInTime">Duration in seconds for fading in (0.1f to 10f)</param>
    /// <param name="fadeOutTime">Duration in seconds for fading out (0.1f to 10f)</param>
    public void SetFadeDurations(float fadeInTime, float fadeOutTime)
    {
        fadeInDuration = Mathf.Clamp(fadeInTime, 0.1f, 10f);
        fadeOutDuration = Mathf.Clamp(fadeOutTime, 0.1f, 10f);
    }
    
    /// <summary>
    /// Get the current fade durations
    /// </summary>
    /// <returns>A Vector2 with x=fadeInDuration and y=fadeOutDuration</returns>
    public Vector2 GetFadeDurations()
    {
        return new Vector2(fadeInDuration, fadeOutDuration);
    }
} 