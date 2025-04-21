using UnityEngine;

public class OutlineController : MonoBehaviour
{
    Transform outline;

    void Awake()
    {
        // on suppose que l'enfant s'appelle "Outline"
        outline = transform.Find("Outline");
        if (outline == null)
            Debug.LogError("Outline child missing on " + name);
    }

    // active ou désactive l'outline
    public void SetOutline(bool on)
    {
        if (outline != null && outline.gameObject.activeSelf != on)
            outline.gameObject.SetActive(on);
    }
}