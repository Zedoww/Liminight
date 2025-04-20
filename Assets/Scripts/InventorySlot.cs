using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData data;
    public int count;

    // Constructeur
    public InventorySlot(ItemData d, int c)
    {
        data  = d;
        count = c;
    }
}