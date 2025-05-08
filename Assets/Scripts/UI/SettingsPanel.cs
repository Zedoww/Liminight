using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Sensibilité souris")]
    public TMP_InputField sensitivityInput;

    [Header("Player Controller")]
    public PlayerController playerController;

    void Start()
    {
        // Volume
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Sensibilité
        sensitivityInput.onValueChanged.AddListener(UpdateSensitivityLive);
        sensitivityInput.onEndEdit.AddListener(ApplySensitivity);

        // Affiche la valeur actuelle dans le champ
        if (playerController != null)
            sensitivityInput.text = playerController.mouseSensitivity.ToString("F0");
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    void UpdateSensitivityLive(string value)
    {
        // Facultatif : change pendant la frappe
        if (float.TryParse(value, out float v))
            playerController.mouseSensitivity = Mathf.Clamp(v, 0f, 300f);
    }

    void ApplySensitivity(string value)
    {
        if (!float.TryParse(value, out float newVal)) return;

        newVal = Mathf.Clamp(newVal, 0f, 300f);
        playerController.mouseSensitivity = newVal;
        sensitivityInput.text = newVal.ToString("F0"); // nettoie
    }
}
