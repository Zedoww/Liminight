using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MenuItemHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Color Settings")]
    public Color normalColor = Color.white;   // <- couleur au repos
    public Color hoverColor = Color.red;     // <- couleur au survol

    [Header("Animation Settings")]
    public float transitionDuration = 0.2f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TextMeshProUGUI tmp;
    private Coroutine tween;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();

        // On force la couleur au démarrage pour être sûr qu’elle est bien « normale »
        if (tmp != null)
            tmp.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData) => StartTween(hoverColor);
    public void OnPointerExit(PointerEventData eventData) => StartTween(normalColor);

    /* ---------- helpers ---------- */

    void StartTween(Color target)
    {
        if (tween != null) StopCoroutine(tween);
        tween = StartCoroutine(TweenColor(target));
    }

    IEnumerator TweenColor(Color target)
    {
        Color start = tmp.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / transitionDuration;   // unscaled !
            tmp.color = Color.Lerp(start, target, transitionCurve.Evaluate(t));
            yield return null;
        }
        tmp.color = target;
    }
}
