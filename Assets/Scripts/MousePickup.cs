using UnityEngine;

public class MousePickup : MonoBehaviour
{
    public Inventory inventory;
    public float pickupRange = 3f;
    public LayerMask pickableMask;
    public Camera cam;

    OutlineController lastOutlined;

    void Update()
    {
        // Raycast depuis le centre
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableMask))
        {
            // Essaie de récupérer OutlineController
            OutlineController oc = hit.collider.GetComponentInParent<OutlineController>();

            // désactive l'ancien
            if (lastOutlined != null && lastOutlined != oc)
                lastOutlined.SetOutline(false);

            // active le nouveau
            if (oc != null)
            {
                oc.SetOutline(true);
                lastOutlined = oc;
            }

            // clic pour ramasser
            if (Input.GetMouseButtonDown(0) && oc != null)
            {
                var holder = hit.collider.GetComponent<ItemDataHolder>();
                if (holder != null)
                {
                    inventory.Add(holder.itemData);
                    Destroy(oc.gameObject);
                    lastOutlined = null;
                }
            }
        }
        else
        {
            // rien visé : enlever surlignage
            if (lastOutlined != null)
            {
                lastOutlined.SetOutline(false);
                lastOutlined = null;
            }
        }
    }
}