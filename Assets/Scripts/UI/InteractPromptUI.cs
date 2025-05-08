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
        
        
    }

    void Update()
    {
        float target = shouldBeVisible ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, target, fadeSpeed * Time.deltaTime);



    }

    public void Show()
    {
        shouldBeVisible = true;
    }

    public void Hide()
    {
        shouldBeVisible = false;
    }

    public void SetText(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }
      
    }
}
