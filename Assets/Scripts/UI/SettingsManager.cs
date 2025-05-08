using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Références UI")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Gameplay")]
    public PlayerController playerController;

    void Start()
    {
        // Sensibilité souris
        sensitivitySlider.minValue = 0.1f;
        sensitivitySlider.maxValue = 500.0f;
        sensitivitySlider.value = playerController.mouseSensitivity;
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);

        // Volume
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateSensitivity(float val)
    {
        playerController.mouseSensitivity = val;
    }

    void UpdateVolume(float val)
    {
        AudioListener.volume = val;
    }

    public void BackToMenu()
    {
        gameObject.SetActive(false);
        // Tu peux appeler pauseMenuManager.ShowPauseMenuOnly() ici si tu veux revenir proprement
    }
}
