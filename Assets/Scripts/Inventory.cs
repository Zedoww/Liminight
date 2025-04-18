using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent<ItemData> onItemAdded
        = new UnityEngine.Events.UnityEvent<ItemData>();

    readonly List<ItemData> items = new();

    public void Add(ItemData item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            onItemAdded.Invoke(item);
        }
    }

    // --- AJOUTS indispensables ----------------------------------

    // Nombre total d’objets
    public int Count => items.Count;

    // Accès sécurisé par index
    public ItemData GetItemAt(int i) =>
        i >= 0 && i < items.Count ? items[i] : null;

    // Vérifier si on possède un objet donné (par son nom)
    public bool Has(string itemName) =>
        items.Exists(it => it.itemName == itemName);
}
