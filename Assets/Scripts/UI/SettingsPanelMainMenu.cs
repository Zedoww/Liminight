using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelMainMenu : MonoBehaviour
{
    [Header("Main Menu Reference")]
    public MainMenuManager mainMenuManager;

    [Header("Volume Settings")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;

    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;

    [Header("Buttons")]
    public Button homeButton;

    /* ----------  Unity ---------- */
    void OnEnable()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);

        // -------- Volume --------
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            UpdateVolumeText(savedVolume);
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // -------- Sensibilité souris --------
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
            UpdateSensitivityText(savedSensitivity);
            sensitivitySlider.onValueChanged.RemoveAllListeners();
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        // -------- Bouton HOME --------
        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(ReturnToMainMenu);
            homeButton.gameObject.SetActive(true);
        }

        ApplyVolume(savedVolume);
    }

    /* ----------  Callbacks ---------- */
    void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        UpdateVolumeText(value);
    }

    void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
        UpdateSensitivityText(value);
    }

    void ReturnToMainMenu()
    {
        // 1. Désactive ce panneau
        gameObject.SetActive(false);

        // 2. Désactive proprement le RawImage de la vidéo (sinon écran noir)
        if (mainMenuManager != null && mainMenuManager.videoDisplay != null)
        {
            mainMenuManager.videoDisplay.texture = null;
            mainMenuManager.videoDisplay.gameObject.SetActive(false);
        }

        // 3. Ré-affiche le menu principal
        mainMenuManager?.CloseSettings();
    }

    /* ----------  Helpers ---------- */
    void ApplyVolume(float value)
    {
        AudioListener.volume = value;

        if (mainMenuManager != null && mainMenuManager.VideoPlayerExists())
            mainMenuManager.SetVideoPlayerVolume(value);
    }

    void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
            volumeValueText.text = (value * 100f).ToString("F0");
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("F2");
    }
}
