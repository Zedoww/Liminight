// SettingsPanelUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;

    [Header("Sensibilité souris")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityValueText;
    public float sensitivityScaleFactor = 30f; // interne = slider * facteur

    [Header("Player Controller")]
    public PlayerController playerController;
    public PauseMenuManager pauseMenu;

    void Start()
    {
        // Volume
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
        UpdateVolumeText(AudioListener.volume);

        // Sensibilité
        float sliderValue = playerController.mouseSensitivity / sensitivityScaleFactor;
        sensitivitySlider.value = sliderValue;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        UpdateSensitivityText(sliderValue);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);
    }

    void UpdateVolumeText(float value)
    {
        volumeValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    void SetSensitivity(float value)
    {
        float actual = value * sensitivityScaleFactor;
        playerController.mouseSensitivity = actual;
        UpdateSensitivityText(value);
    }

    void UpdateSensitivityText(float value)
    {
        sensitivityValueText.text = value.ToString("F1");
    }

    public void BackToMenu()
    {
        gameObject.SetActive(false);
        if (pauseMenu != null)
            pauseMenu.ShowPauseMenuOnly();
    }
}