using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject panel;           // InventoryPanel
    public Transform gridParent;       // Content
    public GameObject itemButtonPrefab;

    [Header("Gameplay")]
    public Inventory inventory;        // sur le Player
    public EquipmentManager equip;     // idem

    readonly List<ItemButton> buttons = new();

    void Awake()
    {
        panel.SetActive(false);
        inventory.onItemAdded.AddListener(_ => Repaint());
    }

    // Touche I (appel depuis EquipmentManager)
    public void Toggle()
    {
        bool state = !panel.activeSelf;
        panel.SetActive(state);
        if (state) Repaint();

        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // ---------- callbacks ----------

    public void OnItemClicked(int idx)
    {
        equip.EquipSlot(idx);
        Toggle();           // ferme l'inventaire
    }

    // ---------- helpers ------------

    void Repaint()
    {
        // Garante un bouton par item
        for (int i = buttons.Count; i < inventory.Count; i++)
        {
            var go = Instantiate(itemButtonPrefab, gridParent);
            buttons.Add(go.GetComponent<ItemButton>());
        }

        // Met � jour chaque bouton
        for (int i = 0; i < buttons.Count; i++)
        {
            var data = inventory.GetItemAt(i);
            buttons[i].Init(this, i, data);
        }
    }
}
