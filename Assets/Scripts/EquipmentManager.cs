using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] Transform handSocket;      // empty sous Main Camera
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryUI ui;            // pour ouvrir l'inventaire

    int equippedIndex = -1;
    GameObject equippedGO;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ui.Toggle();

        // F : sort / range la torche
        if (Input.GetKeyDown(KeyCode.F) && inventory.Has("Flashlight"))
            ToggleFlashlight();

        // clic gauche : action de l'objet tenu
        if (Input.GetMouseButtonDown(0) && equippedGO)
            equippedGO.GetComponent<IUsable>()?.Use();
    }

    void ToggleFlashlight()
    {
        // si on en tient d�j� une, on la range
        if (equippedGO && equippedGO.GetComponent<FlashlightController>())
        { Unequip(); return; }

        // sinon on cherche la torche dans l'inventaire
        for (int i = 0; i < inventory.Count; i++)
        {
            var data = inventory.GetItemAt(i);
            if (data && data.itemName == "Flashlight")
            { EquipSlot(i); break; }
        }
    }

    // ---------- appel� par l'UI ----------
    public void EquipSlot(int index)
    {
        Unequip();

        var data = inventory.GetItemAt(index);
        if (data == null) return;

        // instanciation attachée au socket
        equippedGO = Instantiate(data.prefab, handSocket, false);

        // applique offset position
        Vector3 pos = data.holdPositionOffset;
        equippedGO.transform.localPosition = pos;

        // applique offset rotation (Euler en degrés)
        Vector3 rot = data.holdRotationOffset;
        equippedGO.transform.localRotation = Quaternion.Euler(rot);

        equippedGO.GetComponent<IEquipable>()?.OnEquip();
        equippedIndex = index;
    }

    public void Unequip()
    {
        if (!equippedGO) return;
        equippedGO.GetComponent<IEquipable>()?.OnUnequip();
        Destroy(equippedGO);
        equippedGO = null;
        equippedIndex = -1;
    }
}
