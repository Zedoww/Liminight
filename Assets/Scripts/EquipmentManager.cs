using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryUI ui;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ui?.Toggle();
    }

    public void EquipSlot(int index) { }
    public void Unequip() { }
}