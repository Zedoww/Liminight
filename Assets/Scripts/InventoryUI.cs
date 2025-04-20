using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    const int MAX_SLOTS = 12;
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
        // Assure un nombre fixe de slots affichés
        int maxSlots = MAX_SLOTS;  // par exemple défini en haut

        // Crée autant de boutons que nécessaire
        while (buttons.Count < maxSlots)
        {
            GameObject go = Instantiate(itemButtonPrefab, gridParent);
            buttons.Add(go.GetComponent<ItemButton>());
        }

        // Initialise chaque bouton avec le slot (ou null)
        for (int i = 0; i < maxSlots; i++)
        {
            InventorySlot slot = i < inventory.Count ? inventory.GetSlot(i) : null;
            buttons[i].Init(this, i, slot);
        }
    }
}
