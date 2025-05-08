// ItemButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image selectedFrame;
    [SerializeField] TextMeshProUGUI countLabel;

    int myIndex;
    InventoryUI ui;

    public void Init(InventoryUI inventoryUI, int index, InventorySlot slot)
    {
        ui = inventoryUI;
        myIndex = index;

        bool hasItem = slot != null && slot.data != null;

        icon.enabled = hasItem;
        if (hasItem) icon.sprite = slot.data.icon;
        GetComponent<Button>().interactable = hasItem;

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
        ui.OnItemClicked(myIndex);
    }
}
