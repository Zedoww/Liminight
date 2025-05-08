// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    const int MAX_SLOTS = 15;

    [Header("UI Refs")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject itemButtonPrefab;
    public PauseMenuManager pauseMenuUI;

    [Header("Gameplay")]
    public Inventory inventory;

    readonly List<ItemButton> buttons = new();

    void Awake()
    {
        inventoryPanel.SetActive(false);
        inventory.onItemAdded.AddListener(_ => Repaint());
    }

    public void Toggle()
    {
        if (inventoryPanel.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        inventoryPanel.SetActive(true);
        Repaint();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void Close()
    {
        inventoryPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void CloseInventoryToMenu()
    {
        inventoryPanel.SetActive(false);
        pauseMenuUI.ShowPauseMenuOnly();
    }

    public bool IsOpen() => inventoryPanel.activeSelf;

    public void OnItemClicked(int idx)
    {
        Close();
    }

    void Repaint()
    {
        while (buttons.Count < MAX_SLOTS)
        {
            GameObject go = Instantiate(itemButtonPrefab, gridParent);
            buttons.Add(go.GetComponent<ItemButton>());
        }

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            InventorySlot slot = i < inventory.Count ? inventory.GetSlot(i) : null;
            buttons[i].Init(this, i, slot);
        }
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            Toggle();

        if (inventoryPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }
}
