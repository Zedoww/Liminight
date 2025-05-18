using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Target Immersion Script")]
    [Tooltip("Reference to the EnvironmentImmersion script to control")]
    public EnvironmentImmersion environmentImmersion;
    
    [Header("UI Controls")]
    [Tooltip("Slider for master volume")]
    public Slider masterVolumeSlider;
    
    [Tooltip("Slider for creaking sounds volume")]
    public Slider creakingVolumeSlider;
    
    [Tooltip("Slider for ambient sounds volume")]
    public Slider ambientVolumeSlider;
    
    [Tooltip("Slider for wind sound volume")]
    public Slider windVolumeSlider;
    
    [Tooltip("Slider for pitch variation")]
    public Slider pitchVariationSlider;
    
    [Header("Labels (Optional)")]
    public TextMeshProUGUI masterVolumeLabel;
    public TextMeshProUGUI creakingVolumeLabel;
    public TextMeshProUGUI ambientVolumeLabel;
    public TextMeshProUGUI windVolumeLabel;
    public TextMeshProUGUI pitchVariationLabel;
    
    private void Start()
    {
        // Find the environment immersion script if not set
        if (environmentImmersion == null)
        {
            environmentImmersion = FindFirstObjectByType<EnvironmentImmersion>();
            if (environmentImmersion == null)
            {
                Debug.LogWarning("No EnvironmentImmersion script found in the scene!");
                this.enabled = false;
                return;
            }
        }
        
        // Initialize slider values from the immersion script
        InitializeSliders();
        
        // Add listeners to sliders
        SetupSliderListeners();
    }
    
    private void InitializeSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = environmentImmersion.masterVolume;
            UpdateVolumeLabel(masterVolumeLabel, environmentImmersion.masterVolume);
        }
        
        if (creakingVolumeSlider != null)
        {
            creakingVolumeSlider.value = environmentImmersion.creakingSoundVolume;
            UpdateVolumeLabel(creakingVolumeLabel, environmentImmersion.creakingSoundVolume);
        }
        
        if (ambientVolumeSlider != null)
        {
            ambientVolumeSlider.value = environmentImmersion.ambientSoundVolume;
            UpdateVolumeLabel(ambientVolumeLabel, environmentImmersion.ambientSoundVolume);
        }
        
        if (windVolumeSlider != null)
        {
            windVolumeSlider.value = environmentImmersion.windSoundVolume;
            UpdateVolumeLabel(windVolumeLabel, environmentImmersion.windSoundVolume);
        }
        
        if (pitchVariationSlider != null)
        {
            pitchVariationSlider.value = environmentImmersion.pitchVariation;
            UpdateVolumeLabel(pitchVariationLabel, environmentImmersion.pitchVariation);
        }
    }
    
    private void SetupSliderListeners()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        
        if (creakingVolumeSlider != null)
        {
            creakingVolumeSlider.onValueChanged.AddListener(OnCreakingVolumeChanged);
        }
        
        if (ambientVolumeSlider != null)
        {
            ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChanged);
        }
        
        if (windVolumeSlider != null)
        {
            windVolumeSlider.onValueChanged.AddListener(OnWindVolumeChanged);
        }
        
        if (pitchVariationSlider != null)
        {
            pitchVariationSlider.onValueChanged.AddListener(OnPitchVariationChanged);
        }
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        environmentImmersion.UpdateAudioVolumes(value);
        UpdateVolumeLabel(masterVolumeLabel, value);
    }
    
    public void OnCreakingVolumeChanged(float value)
    {
        environmentImmersion.creakingSoundVolume = value;
        UpdateVolumeLabel(creakingVolumeLabel, value);
    }
    
    public void OnAmbientVolumeChanged(float value)
    {
        environmentImmersion.ambientSoundVolume = value;
        UpdateVolumeLabel(ambientVolumeLabel, value);
    }
    
    public void OnWindVolumeChanged(float value)
    {
        environmentImmersion.UpdateAudioVolumes(
            environmentImmersion.masterVolume, 
            -1, // Keep current creaking volume
            -1, // Keep current ambient volume
            value
        );
        UpdateVolumeLabel(windVolumeLabel, value);
    }
    
    public void OnPitchVariationChanged(float value)
    {
        environmentImmersion.pitchVariation = value;
        UpdateVolumeLabel(pitchVariationLabel, value);
    }
    
    private void UpdateVolumeLabel(TextMeshProUGUI label, float value)
    {
        if (label != null)
        {
            label.text = (value * 100).ToString("0") + "%";
        }
    }
    
    // Save audio settings to PlayerPrefs when the panel is closed
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", environmentImmersion.masterVolume);
        PlayerPrefs.SetFloat("CreakingVolume", environmentImmersion.creakingSoundVolume);
        PlayerPrefs.SetFloat("AmbientVolume", environmentImmersion.ambientSoundVolume);
        PlayerPrefs.SetFloat("WindVolume", environmentImmersion.windSoundVolume);
        PlayerPrefs.SetFloat("PitchVariation", environmentImmersion.pitchVariation);
        PlayerPrefs.Save();
    }
    
    // Load audio settings from PlayerPrefs
    public void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float master = PlayerPrefs.GetFloat("MasterVolume");
            float creaking = PlayerPrefs.GetFloat("CreakingVolume");
            float ambient = PlayerPrefs.GetFloat("AmbientVolume");
            float wind = PlayerPrefs.GetFloat("WindVolume");
            float pitch = PlayerPrefs.GetFloat("PitchVariation");
            
            // Update the immersion script
            environmentImmersion.UpdateAudioVolumes(master, creaking, ambient, wind);
            environmentImmersion.pitchVariation = pitch;
            
            // Update the UI sliders
            InitializeSliders();
        }
    }
} 