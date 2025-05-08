using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject pauseMenu;
    public GameObject settingsPanel;
    public InventoryUI inventoryUI;

    public CanvasGroup canvasGroup;
    public Button resumeButton;
    public Button settingsButton;
    public Button inventoryButton;

    [Header("Fade")]
    public float fadeDuration = 0.3f;

    private bool isPaused = false;

    void Start()
    {
        // Connexion des boutons
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        inventoryButton.onClick.AddListener(OpenInventory);

        // Démarrage caché
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(FadeCanvas(0f, 1f));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        StartCoroutine(FadeOutAndDisable());
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);         // Masque le menu pause
        settingsPanel.SetActive(true);      // Affiche le panneau des réglages
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }


    public void OpenInventory()
    {
        pauseMenu.SetActive(false);
        inventoryUI.Open(); // fade + curseur + pause
    }


    private IEnumerator FadeCanvas(float from, float to)
    {
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
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return FadeCanvas(1f, 0f);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        pauseMenu.SetActive(false);
    }
}
