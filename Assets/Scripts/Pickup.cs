using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public ItemData itemData;          // drag “Torche”

    void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponentInParent<Inventory>();
        if (inv)
        {
            inv.Add(itemData);
            Destroy(gameObject);       // l’objet disparaît du sol
        }
    }
}
