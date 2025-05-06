using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image canvasFillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 3f;

    private float targetAlpha = 0f;

    /// <summary>
    /// Appelé par PlayerController.onStaminaChanged (valeur entre 0 et 1).
    /// </summary>
    public void SetStamina(float ratio)
    {
        Debug.Log("Stamina " + ratio);
        canvasFillImage.fillAmount = ratio;
        targetAlpha = (ratio < 1f) ? 1f : 0f;
    }


    void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}
