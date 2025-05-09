// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private bool isTransitioning = false;
    readonly List<ItemButton> buttons = new();

    void Awake()
    {
        inventoryPanel.SetActive(false);
        inventory.onItemAdded.AddListener(_ => Repaint());
    }

    public void Toggle()
    {
        if (isTransitioning)
            return;
            
        if (inventoryPanel.activeSelf)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        inventoryPanel.SetActive(true);
        Repaint();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        isTransitioning = false;
    }

    public void Close()
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        inventoryPanel.SetActive(false);
        
        // Ne pas modifier Time.timeScale ici, laisser le contrôle
        // au PauseMenuManager ou au système appelant
        
        // Ne modifier ces paramètres que si on revient au jeu
        // et pas quand on revient au menu pause
        if (pauseMenuUI == null || !pauseMenuUI.IsOpen())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        
        isTransitioning = false;
    }

    public void CloseInventoryToMenu()
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        
        // Important: activer directement le menu pause
        // et éviter d'utiliser une coroutine sur ce GameObject
        if (pauseMenuUI != null)
        {
            // Assurer que le pause menu est prêt avant de désactiver l'inventaire
            pauseMenuUI.ShowPauseMenuOnly();
        }
        
        // Maintenant on peut désactiver l'inventaire en toute sécurité
        inventoryPanel.SetActive(false);
        isTransitioning = false;
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
        // Éviter de traiter les inputs pendant les transitions
        if (isTransitioning)
            return;
            
        // L'inventaire peut être ouvert avec la touche I
        if (Keyboard.current?.iKey.wasPressedThisFrame == true)
        {
            Toggle();
        }

        // Quand l'inventaire est ouvert, ESC le ferme et revient au menu pause
        if (inventoryPanel.activeSelf && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
        {
            CloseInventoryToMenu();
        }
    }
}
