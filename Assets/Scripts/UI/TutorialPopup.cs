using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialPopup : MonoBehaviour
{
    public GameObject panel; // Le panel sombre
    public TextMeshProUGUI tutorialText; // Le texte TMP
    public float openSpeed = 0.5f; // Vitesse d'ouverture du panel

    private bool isActive = false;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        panel.SetActive(false);
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        if (isActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            HideTutorial();
        }
    }

    public void ShowTutorial(string message)
    {
        tutorialText.text = message;
        panel.SetActive(true);
        isActive = true;
        StartCoroutine(OpenPanel());
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideTutorial()
    {
        StartCoroutine(ClosePanel());
        isActive = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator OpenPanel()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / openSpeed;
            yield return null;
        }
    }

    private IEnumerator ClosePanel()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / openSpeed;
            yield return null;
        }
        panel.SetActive(false);
    }
}