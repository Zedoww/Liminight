using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject pauseMenu;
    public GameObject settingsPanel;
    public InventoryUI inventoryUI; // ✅ c'est bien le script, pas GameObject

    public CanvasGroup canvasGroup;
    public Button resumeButton;
    public Button settingsButton;
    public Button inventoryButton;

    [Header("Fade")]
    public float fadeDuration = 0.3f;

    private bool isPaused = false;
    private bool isTransitioning = false;
    
    public bool IsOpen() => pauseMenu.activeSelf;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        inventoryButton.onClick.AddListener(OpenInventory);

        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        // Ne pas traiter les inputs pendant une transition
        if (isTransitioning)
            return;
            
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Si le panneau de détails d'objets est ouvert, forcer l'appel à HandleEscapeKey
            if (inventoryUI != null && inventoryUI.IsDetailsOpen())
            {
                Debug.Log("PauseMenuManager: Détection du panneau de détails ouvert, appel à HandleEscapeKey");
                inventoryUI.HandleEscapeKey();
                return;
            }
            
            // Si le panneau de paramètres est ouvert, revenir au menu pause 
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }
            
            // Si l'inventaire est ouvert, le fermer et reprendre le jeu directement
            if (inventoryUI.IsOpen())
            {
                inventoryUI.Close();
                ResumeGame();
                return;
            }
            
            // Sinon, toggle le menu pause
            if (isPaused) 
                ResumeGame();
            else 
                PauseGame();
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            // Si les paramètres sont ouverts, ne rien faire
            if (settingsPanel.activeSelf) 
                return;

            if (inventoryUI.IsOpen())
            {
                // If inventory is open, close it and resume game
                inventoryUI.Close();
                ResumeGame();
            }
            else if (isPaused)
            {
                // If paused, open inventory
                OpenInventory();
            }
            else
            {
                // If in gameplay, pause and open inventory
                PauseGame();
                OpenInventory();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        StartCoroutine(FadeCanvas(0f, 1f));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        StartCoroutine(FadeOutAndResume());
        // Immediately restore player control
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void OpenInventory()
    {
        pauseMenu.SetActive(false);
        inventoryUI.Open();
    }

    public void CloseInventory()
    {
        if (inventoryUI.IsOpen())
        {
            // With the new direct return to gameplay, we should also resume
            inventoryUI.Close();
            ResumeGame();
        }
    }

    public void ShowPauseMenuOnly()
    {
        // S'assurer que le game flow est en pause
        isPaused = true;
        Time.timeScale = 0f;
    
        // S'assurer que tous les autres panneaux sont fermés
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // S'assurer que le menu UI est complètement opérationnel
        pauseMenu.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        // S'assurer que le curseur est visible et non verrouillé
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Rafraîchir tous les boutons
        EnableAllButtons();
    }
    
    private void EnableAllButtons()
    {
        // Explicitement activer chaque bouton et l'assurer qu'il est interactif
        if (resumeButton != null) 
        {
            resumeButton.gameObject.SetActive(true);
            resumeButton.interactable = true;
        }
        if (settingsButton != null) 
        {
            settingsButton.gameObject.SetActive(true);
            settingsButton.interactable = true;
        }
        if (inventoryButton != null) 
        {
            inventoryButton.gameObject.SetActive(true);
            inventoryButton.interactable = true;
        }
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        isTransitioning = true;
        float elapsed = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
        isTransitioning = false;
    }

    private IEnumerator FadeOutAndResume()
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeCanvas(1f, 0f));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
        // Time.timeScale is now set in ResumeGame immediately
        isTransitioning = false;
    }
}
