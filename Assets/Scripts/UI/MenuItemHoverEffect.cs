using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuItemHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Color Settings")]
    [Tooltip("The color to transition to when hovering")]
    public Color hoverColor = Color.red;
    
    [Tooltip("The original color of the text")]
    private Color originalColor;
    
    [Header("Animation Settings")]
    [Tooltip("Time in seconds for the color transition")]
    public float transitionDuration = 0.2f;
    
    [Tooltip("The curve that controls the transition animation")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Reference to the text component
    private TextMeshProUGUI textComponent;
    private Text legacyTextComponent;
    
    // Coroutine references for smooth transitions
    private Coroutine colorTransitionCoroutine;
    
    private void Awake()
    {
        // Try to get TextMeshPro component first, if not found try legacy Text
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent == null)
        {
            legacyTextComponent = GetComponentInChildren<Text>();
        }
        
        // Store the original color
        if (textComponent != null)
        {
            originalColor = textComponent.color;
        }
        else if (legacyTextComponent != null)
        {
            originalColor = legacyTextComponent.color;
        }
        else
        {
            Debug.LogWarning("No text component found on " + gameObject.name);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Stop any ongoing transitions
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        
        // Start the transition to hover color
        colorTransitionCoroutine = StartCoroutine(TransitionColor(originalColor, hoverColor));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Stop any ongoing transitions
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }
        
        // Start the transition back to original color
        colorTransitionCoroutine = StartCoroutine(TransitionColor(hoverColor, originalColor));
    }
    
    private IEnumerator TransitionColor(Color fromColor, Color toColor)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionDuration;
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            Color newColor = Color.Lerp(fromColor, toColor, curveValue);
            
            // Apply the new color to the appropriate text component
            if (textComponent != null)
            {
                textComponent.color = newColor;
            }
            else if (legacyTextComponent != null)
            {
                legacyTextComponent.color = newColor;
            }
            
            yield return null;
        }
        
        // Ensure we end up with the exact target color
        if (textComponent != null)
        {
            textComponent.color = toColor;
        }
        else if (legacyTextComponent != null)
        {
            legacyTextComponent.color = toColor;
        }
    }
} 