// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    const int MAX_SLOTS = 15;

    [Header("UI Refs")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject itemButtonPrefab;
    public PauseMenuManager pauseMenuUI;

    [Header("Item Details Panel")]
    public GameObject itemDetailsPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemContentText;
    public Image itemDetailIcon;
    public Button closeDetailsButton;
    public Button returnToInventoryButton;

    [Header("Gameplay")]
    public Inventory inventory;

    private bool isTransitioning = false;
    readonly List<ItemButton> buttons = new();
    private int selectedItemIndex = -1;

    void Awake()
    {
        inventoryPanel.SetActive(false);
        
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
            
        if (closeDetailsButton != null)
            closeDetailsButton.onClick.AddListener(CloseItemDetails);
            
        if (returnToInventoryButton != null)
            returnToInventoryButton.onClick.AddListener(ReturnToInventory);
            
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
        
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
            
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
        
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
        
        // Restore gameplay state regardless of pause menu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        
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
        
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
            
        isTransitioning = false;
    }

    public bool IsOpen() => inventoryPanel.activeSelf;

    // Méthode pour vérifier si le panneau de détails est actif
    public bool IsDetailsOpen() => itemDetailsPanel != null && itemDetailsPanel.activeSelf;

    public void OnItemClicked(int idx)
    {
        ItemData item = inventory.GetItemAt(idx);
        if (item != null)
        {
            ShowItemDetails(item);
            selectedItemIndex = idx;
        }
    }
    
    private void ShowItemDetails(ItemData item)
    {
        if (itemDetailsPanel == null || item == null)
            return;
            
        // Remplir les détails de l'objet
        if (itemNameText != null)
            itemNameText.text = item.itemName;
            
        if (itemDescriptionText != null)
            itemDescriptionText.text = item.shortDescription;
            
        if (itemContentText != null)
        {
            // Si c'est un document, afficher son contenu, sinon cacher la zone de texte
            if (!string.IsNullOrEmpty(item.documentContent))
            {
                itemContentText.gameObject.SetActive(true);
                itemContentText.text = item.documentContent;
            }
            else
            {
                itemContentText.gameObject.SetActive(false);
            }
        }
            
        if (itemDetailIcon != null)
        {
            itemDetailIcon.sprite = item.icon;
            itemDetailIcon.enabled = true;
        }
            
        // Afficher le panneau de détails et cacher l'inventaire
        inventoryPanel.SetActive(false);
        itemDetailsPanel.SetActive(true);
    }
    
    private void CloseItemDetails()
    {
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
            
        Close();
    }
    
    private void ReturnToInventory()
    {
        if (itemDetailsPanel != null)
            itemDetailsPanel.SetActive(false);
            
        inventoryPanel.SetActive(true);
        
        // S'assurer que le jeu reste en pause et que le curseur reste visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
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
        {
            Debug.Log("InventoryUI: Transition en cours, inputs ignorés");
            return;
        }
            
        // L'inventaire peut être ouvert avec la touche I
        if (Keyboard.current?.iKey.wasPressedThisFrame == true)
        {
            Toggle();
        }

        // Vérifier explicitement si la touche Échap est pressée
        bool escapePressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        
        // Toujours imprimer l'état pour déboguer
        if (escapePressed)
        {
            Debug.Log("InventoryUI: Touche Échap détectée");
            Debug.Log("InventoryUI: État panel détails = " + (itemDetailsPanel != null ? itemDetailsPanel.activeSelf.ToString() : "null"));
            Debug.Log("InventoryUI: État inventaire = " + inventoryPanel.activeSelf);
        }

        if (escapePressed)
        {
            // Si on est dans les détails d'un objet, retourner à l'inventaire
            if (itemDetailsPanel != null && itemDetailsPanel.activeSelf)
            {
                Debug.Log("InventoryUI: Retour à l'inventaire depuis les détails");
                ReturnToInventory();
            }
            // Si on est dans l'inventaire, fermer l'inventaire et revenir au jeu
            else if (inventoryPanel.activeSelf)
            {
                Debug.Log("InventoryUI: Fermeture de l'inventaire");
                Close();
            }
        }
    }

    // Méthode publique pour gérer la touche Échap explicitement
    public void HandleEscapeKey()
    {
        if (isTransitioning)
            return;
            
        Debug.Log("InventoryUI: HandleEscapeKey appelé explicitement");
        
        // Si on est dans les détails d'un objet, retourner à l'inventaire
        if (itemDetailsPanel != null && itemDetailsPanel.activeSelf)
        {
            Debug.Log("InventoryUI: Retour à l'inventaire depuis les détails via HandleEscapeKey");
            ReturnToInventory();
        }
        // Si on est dans l'inventaire principal, fermer l'inventaire
        else if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            Debug.Log("InventoryUI: Fermeture de l'inventaire via HandleEscapeKey");
            Close();
        }
    }
}
