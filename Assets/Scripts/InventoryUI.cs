using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    const int MAX_SLOTS = 12;

    [Header("Désactiver contrôle joueur")]
    [SerializeField] MonoBehaviour[] scriptsToDisable;

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
        bool open = ! panel.activeSelf;
        panel.SetActive(open);

        if (open)
        {
            Repaint();
        }

        // gestion curseur
        if (open)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // désactivation / réactivation des scripts de contrôle
        for (int i = 0; i < scriptsToDisable.Length; i = i + 1)
        {
            scriptsToDisable[i].enabled = ! open;
        }
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
