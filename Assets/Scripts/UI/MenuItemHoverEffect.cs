using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MenuItemHoverEffect : MonoBehaviour,
                                   IPointerEnterHandler,
                                   IPointerExitHandler,
                                   IPointerClickHandler,
                                   IDeselectHandler       // ← nouveau
{
    /* ---------- réglages ---------- */
    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.red;

    [Header("Animation Settings")]
    public float transitionDuration = 0.2f;
    public AnimationCurve transitionCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    /* ---------- internes ---------- */
    TextMeshProUGUI tmp;
    Coroutine tween;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp) tmp.color = normalColor;
    }

    /* ----- remettre la couleur quand on masque/affiche l’objet ----- */
    void OnEnable() { if (tmp) tmp.color = normalColor; }
    void OnDisable() { if (tmp) tmp.color = normalColor; }

    /* ---------------- événement souris / focus ---------------- */
    public void OnPointerEnter(PointerEventData _) => StartTween(hoverColor);
    public void OnPointerExit(PointerEventData _) => StartTween(normalColor);
    public void OnPointerClick(PointerEventData _) => StartTween(normalColor);
    public void OnDeselect(BaseEventData _) => StartTween(normalColor);

    /* ---------------- helpers animation ---------------- */
    void StartTween(Color target)
    {
        if (!tmp) return;
        if (tween != null) StopCoroutine(tween);
        tween = StartCoroutine(TweenColor(target));
    }

    IEnumerator TweenColor(Color target)
    {
        Color start = tmp.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / transitionDuration; // menu en pause OK
            tmp.color = Color.Lerp(start, target,
                                   transitionCurve.Evaluate(t));
            yield return null;
        }
        tmp.color = target;
    }
}
