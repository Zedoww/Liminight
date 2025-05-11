// ItemButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image selectedFrame;
    [SerializeField] TextMeshProUGUI countLabel;

    int myIndex;
    InventoryUI ui;
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Init(InventoryUI inventoryUI, int index, InventorySlot slot)
    {
        ui = inventoryUI;
        myIndex = index;

        bool hasItem = slot != null && slot.data != null;

        icon.enabled = hasItem;
        if (hasItem) icon.sprite = slot.data.icon;
        
        if (button == null)
            button = GetComponent<Button>();
            
        button.interactable = hasItem;

        selectedFrame.enabled = false;

        if (hasItem && slot.count > 1)
        {
            countLabel.enabled = true;
            countLabel.text = slot.count.ToString();
        }
        else
        {
            countLabel.enabled = false;
        }

        if (hasItem)
            icon.rectTransform.localScale = Vector3.one * slot.data.iconScale;
    }

    public void OnClick()
    {
        Debug.Log("Item clicked: " + myIndex);
        if (ui != null)
        {
            ui.OnItemClicked(myIndex);
        }
        else
        {
            Debug.LogError("InventoryUI reference is null");
        }
    }
}
