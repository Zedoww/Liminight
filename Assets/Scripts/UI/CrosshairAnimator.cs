using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CrosshairAnimator : MonoBehaviour
{
    public float fadeSpeed = 6f;
    public float scaleSpeed = 6f;
    public float targetScale = 1.2f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool shouldBeVisible = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one;
    }

    void Update()
    {
        // Animation de transparence
        float targetAlpha = shouldBeVisible ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

        // Animation de zoom (scale)
        float target = shouldBeVisible ? targetScale : 1f;
        float current = rectTransform.localScale.x;
        float newScale = Mathf.MoveTowards(current, target, scaleSpeed * Time.deltaTime);
        rectTransform.localScale = new Vector3(newScale, newScale, 1f);
    }

    public void Show() => shouldBeVisible = true;
    public void Hide() => shouldBeVisible = false;
}
