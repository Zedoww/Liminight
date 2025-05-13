using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentImmersion : MonoBehaviour
{
    [Header("Ambient Effects")]
    [Tooltip("Volume profile for controlling post-processing effects")]
    public Volume postProcessingVolume;
    
    [Tooltip("Random creaking sounds")]
    public AudioClip[] creakingSounds;
    
    [Tooltip("Random ambient sounds")]
    public AudioClip[] ambientSounds;
    
    [Tooltip("Wind sounds")]
    public AudioClip windSound;
    
    [Header("Audio Settings")]
    [Tooltip("Master volume multiplier for all ambient sounds")]
    [Range(0f, 1f)]
    public float masterVolume = 1.0f;
    
    [Tooltip("Volume for creaking sounds")]
    [Range(0f, 1f)]
    public float creakingSoundVolume = 0.2f;
    
    [Tooltip("Volume for ambient sounds")]
    [Range(0f, 1f)]
    public float ambientSoundVolume = 0.15f;
    
    [Tooltip("Volume for wind sound")]
    [Range(0f, 1f)]
    public float windSoundVolume = 0.1f;
    
    [Tooltip("Pitch variation range (1.0 = normal pitch)")]
    [Range(0f, 0.5f)]
    public float pitchVariation = 0.1f;
    
    [Header("Light Settings")]
    [Tooltip("All lights in the scene to flicker")]
    public Light[] flickeringLights;
    
    [Tooltip("Intensity of light flickering (0-1)")]
    [Range(0f, 1f)]
    public float flickerIntensity = 0.2f;
    
    [Tooltip("How often lights can flicker (seconds)")]
    [Range(1f, 30f)]
    public float flickerFrequency = 10f;
    
    [Header("Dust Particles")]
    [Tooltip("Dust particle system")]
    public ParticleSystem dustParticles;
    
    [Header("TV Static")]
    [Tooltip("TV materials with emission to flicker")]
    public Material[] tvMaterials;
    
    // Private references
    private Dictionary<Light, float> originalLightIntensities = new Dictionary<Light, float>();
    private AudioSource[] audioSources = new AudioSource[3]; // For different sound categories
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;
    
    private void Start()
    {
        SetupAudioSources();
        SetupLights();
        SetupPostProcessing();
        
        // Start the immersion coroutines
        StartCoroutine(PlayRandomCreakingSounds());
        StartCoroutine(PlayRandomAmbientSounds());
        StartCoroutine(FlickerLights());
        StartCoroutine(TVStaticEffect());
        
        // Start continuous wind sound
        if (windSound != null && audioSources[2] != null)
        {
            audioSources[2].clip = windSound;
            audioSources[2].loop = true;
            audioSources[2].volume = windSoundVolume * masterVolume;
            audioSources[2].Play();
        }
        
        // Activate dust particles if available
        if (dustParticles != null)
        {
            dustParticles.Play();
        }
    }
    
    private void SetupAudioSources()
    {
        // Create audio sources for different sound categories
        for (int i = 0; i < audioSources.Length; i++)
        {
            GameObject audioSourceObj = new GameObject("AudioSource_" + i);
            audioSourceObj.transform.parent = transform;
            audioSources[i] = audioSourceObj.AddComponent<AudioSource>();
            audioSources[i].spatialBlend = 0.7f; // Mix of 2D and 3D sound
            audioSources[i].rolloffMode = AudioRolloffMode.Linear;
            audioSources[i].maxDistance = 30f;
        }
    }
    
    private void SetupLights()
    {
        if (flickeringLights != null)
        {
            foreach (Light light in flickeringLights)
            {
                if (light != null)
                {
                    originalLightIntensities[light] = light.intensity;
                }
            }
        }
    }
    
    private void SetupPostProcessing()
    {
        if (postProcessingVolume != null && postProcessingVolume.profile != null)
        {
            // Get post-processing effects we want to modify dynamically
            postProcessingVolume.profile.TryGet(out vignette);
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out filmGrain);
            
            // Set initial values
            if (vignette != null)
            {
                vignette.intensity.value = 0.3f;
            }
            
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = 0.05f;
            }
            
            if (filmGrain != null)
            {
                filmGrain.intensity.value = 0.2f;
            }
        }
    }
    
    private IEnumerator PlayRandomCreakingSounds()
    {
        if (creakingSounds == null || creakingSounds.Length == 0 || audioSources[0] == null)
            yield break;
            
        while (true)
        {
            // Wait for random time between 10-30 seconds
            yield return new WaitForSeconds(Random.Range(10f, 30f));
            
            // Play random creaking sound
            AudioClip clipToPlay = creakingSounds[Random.Range(0, creakingSounds.Length)];
            if (clipToPlay != null)
            {
                audioSources[0].clip = clipToPlay;
                audioSources[0].volume = Random.Range(creakingSoundVolume * 0.5f, creakingSoundVolume) * masterVolume;
                audioSources[0].pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
                audioSources[0].Play();
                
                // Temporarily increase vignette when creaking occurs
                if (vignette != null)
                {
                    float originalIntensity = vignette.intensity.value;
                    vignette.intensity.value = originalIntensity + 0.1f;
                    yield return new WaitForSeconds(0.5f);
                    vignette.intensity.value = originalIntensity;
                }
            }
        }
    }
    
    private IEnumerator PlayRandomAmbientSounds()
    {
        if (ambientSounds == null || ambientSounds.Length == 0 || audioSources[1] == null)
            yield break;
            
        while (true)
        {
            // Wait for random time between 15-60 seconds
            yield return new WaitForSeconds(Random.Range(15f, 60f));
            
            // Play random ambient sound
            AudioClip clipToPlay = ambientSounds[Random.Range(0, ambientSounds.Length)];
            if (clipToPlay != null)
            {
                audioSources[1].clip = clipToPlay;
                audioSources[1].volume = Random.Range(ambientSoundVolume * 0.3f, ambientSoundVolume) * masterVolume;
                audioSources[1].pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
                audioSources[1].Play();
            }
        }
    }
    
    private IEnumerator FlickerLights()
    {
        if (flickeringLights == null || flickeringLights.Length == 0)
            yield break;
            
        while (true)
        {
            // Wait for random time based on flicker frequency
            yield return new WaitForSeconds(Random.Range(flickerFrequency * 0.5f, flickerFrequency * 1.5f));
            
            // Choose random light to flicker
            if (flickeringLights.Length > 0)
            {
                Light lightToFlicker = flickeringLights[Random.Range(0, flickeringLights.Length)];
                if (lightToFlicker != null && originalLightIntensities.ContainsKey(lightToFlicker))
                {
                    StartCoroutine(FlickerSingleLight(lightToFlicker));
                }
            }
        }
    }
    
    private IEnumerator FlickerSingleLight(Light light)
    {
        if (light == null || !originalLightIntensities.ContainsKey(light))
            yield break;
            
        float originalIntensity = originalLightIntensities[light];
        
        // Flicker for a short duration
        float flickerDuration = Random.Range(0.05f, 0.3f);
        float endTime = Time.time + flickerDuration;
        
        while (Time.time < endTime)
        {
            // Randomly adjust light intensity
            light.intensity = originalIntensity * Random.Range(1f - flickerIntensity, 1f);
            
            // Wait a very short time
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
        }
        
        // Maybe turn the light off completely for a moment
        if (Random.value < 0.3f)
        {
            light.intensity = 0;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }
        
        // Return to original intensity
        light.intensity = originalIntensity;
    }
    
    private IEnumerator TVStaticEffect()
    {
        if (tvMaterials == null || tvMaterials.Length == 0)
            yield break;
            
        while (true)
        {
            // Randomly flicker TV screens
            foreach (Material tvMat in tvMaterials)
            {
                if (tvMat != null && tvMat.HasProperty("_EmissionColor"))
                {
                    Color originalEmission = tvMat.GetColor("_EmissionColor");
                    
                    // Random flicker
                    if (Random.value < 0.2f)
                    {
                        float flickerIntensity = Random.Range(0.5f, 1.5f);
                        tvMat.SetColor("_EmissionColor", originalEmission * flickerIntensity);
                    }
                    else
                    {
                        tvMat.SetColor("_EmissionColor", originalEmission);
                    }
                }
            }
            
            // Wait a small amount of time
            yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
        }
    }
    
    // Called when player is close to a scary area or during important story moments
    public void IncreaseTension(float amount, float duration)
    {
        StartCoroutine(TensionEffect(amount, duration));
    }
    
    private IEnumerator TensionEffect(float amount, float duration)
    {
        // Increase post-processing effects temporarily
        float originalVignetteIntensity = vignette?.intensity.value ?? 0.3f;
        float originalChromaticIntensity = chromaticAberration?.intensity.value ?? 0.05f;
        float originalGrainIntensity = filmGrain?.intensity.value ?? 0.2f;
        
        // Increase effects
        if (vignette != null)
            vignette.intensity.value = Mathf.Clamp01(originalVignetteIntensity + (0.3f * amount));
            
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Clamp01(originalChromaticIntensity + (0.3f * amount));
            
        if (filmGrain != null)
            filmGrain.intensity.value = Mathf.Clamp01(originalGrainIntensity + (0.2f * amount));
            
        // Hold for duration
        yield return new WaitForSeconds(duration);
        
        // Gradually return to normal
        float returnTime = 2.0f;
        float elapsedTime = 0;
        
        while (elapsedTime < returnTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / returnTime;
            
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(originalVignetteIntensity + (0.3f * amount), originalVignetteIntensity, t);
                
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(originalChromaticIntensity + (0.3f * amount), originalChromaticIntensity, t);
                
            if (filmGrain != null)
                filmGrain.intensity.value = Mathf.Lerp(originalGrainIntensity + (0.2f * amount), originalGrainIntensity, t);
                
            yield return null;
        }
        
        // Ensure we return to exact original values
        if (vignette != null)
            vignette.intensity.value = originalVignetteIntensity;
            
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = originalChromaticIntensity;
            
        if (filmGrain != null)
            filmGrain.intensity.value = originalGrainIntensity;
    }
    
    // Method to update audio volumes at runtime
    public void UpdateAudioVolumes(float master, float creaking = -1, float ambient = -1, float wind = -1)
    {
        masterVolume = Mathf.Clamp01(master);
        
        if (creaking >= 0)
            creakingSoundVolume = Mathf.Clamp01(creaking);
            
        if (ambient >= 0)
            ambientSoundVolume = Mathf.Clamp01(ambient);
            
        if (wind >= 0)
            windSoundVolume = Mathf.Clamp01(wind);
        
        // Update wind sound immediately if it's playing
        if (audioSources[2] != null && audioSources[2].isPlaying)
        {
            audioSources[2].volume = windSoundVolume * masterVolume;
        }
    }
} 