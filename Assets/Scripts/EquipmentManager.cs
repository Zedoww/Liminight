using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryUI ui;

    // L’objet précédemment tenu (plus utilisé pour l’instant)
    GameObject equippedGO;
    int equippedIndex = -1;

    void Update()
    {
        // On laisse l'ouverture d'inventaire
        if (Input.GetKeyDown(KeyCode.I))
            ui?.Toggle();

        // On désactive toute gestion d'objet tenu
    }

    // Conservé pour compatibilité future
    public void EquipSlot(int index) { }
    public void Unequip()
    {
        if (!equippedGO) return;
        Destroy(equippedGO);
        equippedGO = null;
        equippedIndex = -1;
    }

    void ToggleFlashlight()
    {
        // Ne fait rien, la gestion est maintenant dans FlashlightController
    }
}
