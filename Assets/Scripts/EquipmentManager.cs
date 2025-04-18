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
        // si on en tient déjà une, on la range
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

    // ---------- appelé par l'UI ----------
    public void EquipSlot(int index)
    {
        Unequip();

        var data = inventory.GetItemAt(index);
        if (data == null) return;

        // le false évite de déplacer HandSocket
        equippedGO = Instantiate(data.prefab, handSocket, false);
        equippedGO.transform.localPosition = Vector3.zero;
        equippedGO.transform.localRotation = Quaternion.identity;

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
