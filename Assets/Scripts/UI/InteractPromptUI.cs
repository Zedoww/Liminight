using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class InteractPromptUI : MonoBehaviour
{
    public float fadeSpeed = 6f;

    private CanvasGroup group;
    private bool shouldBeVisible = false;
    private TextMeshProUGUI promptText;
    


    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        promptText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (promptText == null)
        {
            Debug.LogError("InteractPromptUI: TextMeshProUGUI non trouvé dans les enfants!");
        }
    }

    void Update()
    {
        float target = shouldBeVisible ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, target, fadeSpeed * Time.deltaTime);



    }

    public void Show()
    {
        shouldBeVisible = true;
        Debug.Log($"InteractPromptUI.Show() - shouldBeVisible: {shouldBeVisible}, alpha: {group.alpha}");
    }

    public void Hide()
    {
        shouldBeVisible = false;
        Debug.Log($"InteractPromptUI.Hide() - shouldBeVisible: {shouldBeVisible}, alpha: {group.alpha}");
    }

    public void SetText(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
            Debug.Log($"InteractPromptUI.SetText() - Nouveau texte: {text}");
        }
        else
        {
            Debug.LogError("InteractPromptUI.SetText() - promptText est null!");
        }
    }
}
