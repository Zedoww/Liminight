using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InteractPromptUI : MonoBehaviour
{
    public float fadeSpeed = 6f;

    private CanvasGroup group;
    private bool shouldBeVisible = false;
    


    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    void Update()
    {
        float target = shouldBeVisible ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, target, fadeSpeed * Time.deltaTime);



    }

    public void Show() => shouldBeVisible = true;
    public void Hide() => shouldBeVisible = false;
}
