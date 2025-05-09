using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsPanelBackgroundFixer : MonoBehaviour
{
    [Header("Background Settings")]
    public Color backgroundColor = new Color(0, 0, 0, 0.8f); // Dark semi-transparent background
    
    private Image backgroundImage;
    
    private void Awake()
    {
        // Check if there's a background image component
        backgroundImage = GetComponent<Image>();
        
        // If no background image, add one
        if (backgroundImage == null)
        {
            // Add an image component to this panel
            backgroundImage = gameObject.AddComponent<Image>();
            backgroundImage.color = backgroundColor;
            
            // Make sure it's drawn behind other elements by setting its position in hierarchy
            backgroundImage.transform.SetAsFirstSibling();
        }
        
        // Make sure the Container inside the SettingsPanel has a background too
        Transform containerTransform = transform.Find("Container");
        if (containerTransform != null)
        {
            Image containerImage = containerTransform.GetComponent<Image>();
            if (containerImage == null)
            {
                containerImage = containerTransform.gameObject.AddComponent<Image>();
                containerImage.color = backgroundColor;
            }
        }
        
        // Ensure this settings panel is fully visible
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
} 