using UnityEngine;

[RequireComponent(typeof(FuseBoxManager))]
public class FuseBoxDebug : MonoBehaviour
{
    private FuseBoxManager fuseBoxManager;
    
    void Start()
    {
        fuseBoxManager = GetComponent<FuseBoxManager>();
        
        // Check if on the correct layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer == -1)
        {
            Debug.LogError("ERROR: 'Interactable' layer does not exist in project settings!");
        }
        else if (gameObject.layer != interactableLayer)
        {
            Debug.LogError("ERROR: FuseBox is not on the 'Interactable' layer! Current layer: " + LayerMask.LayerToName(gameObject.layer));
        }
        else
        {
            Debug.Log("FuseBox is on the correct layer: " + LayerMask.LayerToName(gameObject.layer));
        }
        
        // Check if has a collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("ERROR: FuseBox has no collider for interaction!");
        }
        else
        {
            if (collider.isTrigger)
            {
                Debug.Log("FuseBox collider is a trigger - this is usually fine for interaction");
            }
            else
            {
                Debug.Log("FuseBox has a non-trigger collider - should work for raycast interactions");
            }
        }
        
        Debug.Log("FuseBoxDebug initialized. Press F to interact when looking at the fusebox.");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("(Debug) Force testing FuseBox interaction");
            fuseBoxManager.HandleInteraction();
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw a larger visual indicator in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
} 