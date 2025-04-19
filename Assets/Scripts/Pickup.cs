using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public ItemData itemData;          // drag Torche

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
            
        var inv = other.GetComponentInParent<Inventory>();
        if (inv)
        {
            inv.Add(itemData);
            Destroy(gameObject);       // l'objet disparait du sol
        }
    }
}
