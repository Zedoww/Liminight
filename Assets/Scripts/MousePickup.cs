using UnityEngine;

public class MousePickup : MonoBehaviour
{
    public Inventory inventory;            // drag le composant Inventory du Player
    public float pickupRange = 3f;         // distance max
    public LayerMask pickableMask;         // couche Pickable
    public Transform crosshair;            // optionnel : affichage sur UI

    void Update()
    {
        // clic gauche et pas d'intéraction si on vise un UI
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange, pickableMask))
            {
                // L'objet visé doit posséder un ItemDataHolder
                ItemDataHolder holder = hit.collider.GetComponent<ItemDataHolder>();
                if (holder != null)
                {
                    inventory.Add(holder.itemData);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}