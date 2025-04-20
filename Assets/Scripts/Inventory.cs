using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Un slot contient un item et sa quantité
public class Inventory : MonoBehaviour
{
    public UnityEvent<ItemData> onItemAdded = new UnityEvent<ItemData>();

    readonly List<InventorySlot> slots = new List<InventorySlot>();

    // Nombre de slots utilisés
    public int Count
    {
        get { return slots.Count; }
    }

    // Récupère le slot à l’index (ou null si hors limites)
    public InventorySlot GetSlot(int i)
    {
        if (i >= 0 && i < slots.Count)
            return slots[i];
        return null;
    }

    // Récupère seulement l’ItemData pour compatibilité
    public ItemData GetItemAt(int i)
    {
        InventorySlot s = GetSlot(i);
        return s != null ? s.data : null;
    }

    // Vérifie si l’inventaire contient au moins un exemplaire du nom donné
    public bool Has(string itemName)
    {
        foreach (InventorySlot s in slots)
        {
            if (s.data.itemName == itemName && s.count > 0)
                return true;
        }
        return false;
    }

    // Ajoute un item : incrémente le count ou crée un nouveau slot
    public void Add(ItemData item)
    {
        foreach (InventorySlot s in slots)
        {
            if (s.data == item)
            {
                s.count = s.count + 1;
                onItemAdded.Invoke(item);
                return;
            }
        }
        slots.Add(new InventorySlot(item, 1));
        onItemAdded.Invoke(item);
    }

    // Enlève un exemplaire de l’item (pour consommation par exemple)
    public void RemoveOne(ItemData item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].data == item)
            {
                slots[i].count = slots[i].count - 1;
                if (slots[i].count <= 0)
                {
                    slots.RemoveAt(i);
                }
                return;
            }
        }
    }
}