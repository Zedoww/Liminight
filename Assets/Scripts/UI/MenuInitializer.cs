using UnityEngine;

public class MenuInitializer : MonoBehaviour
{
    public Canvas menuCanvas;
    
    private void Awake()
    {
        // Make sure the MenuCanvas is enabled
        if (menuCanvas != null)
        {
            menuCanvas.enabled = true;
            
            // Set render mode to Screen Space - Overlay for maximum visibility
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Ensure all child objects are active
            foreach (Transform child in menuCanvas.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        
        // Make menu visible
        GameObject panelObject = GameObject.Find("Panel");
        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }
        
        // Ensure the canvas is visible in the scene
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
} 