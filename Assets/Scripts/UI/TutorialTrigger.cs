using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea]
    public string tutorialMessage = "Votre message de tutoriel ici.";
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            FindFirstObjectByType<TutorialPopup>().ShowTutorial(tutorialMessage);
            if (triggerOnce) hasTriggered = true;
        }
    }
}