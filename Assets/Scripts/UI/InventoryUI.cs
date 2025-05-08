// InventoryUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    const int MAX_SLOTS = 12;

    [Header("UI Refs")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject itemButtonPrefab;
    public CanvasGroup canvasGroup;

    [Header("Gameplay")]
    public Inventory inventory;

    [Header("Fade")]
    public float fadeDuration = 0.3f;

    readonly List<ItemButton> buttons = new();
    bool isFading = false;

    void Awake()
    {
        inventoryPanel.SetActive(false);
        inventory.onItemAdded.AddListener(_ => Repaint());

        // Démarre invisible
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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

        StartCoroutine(FadeCanvas(0f, 1f, true));
    }

    public void Close()
    {
        if (isFading) return;
        StartCoroutine(FadeOutAndDisable());
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

    IEnumerator FadeCanvas(float from, float to, bool enable)
    {
        isFading = true;
        float elapsed = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
        isFading = false;

        if (!enable)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            inventoryPanel.SetActive(false);
        }
    }

    IEnumerator FadeOutAndDisable()
    {
        yield return FadeCanvas(1f, 0f, false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
}