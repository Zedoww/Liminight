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

    // Initialise le bouton avec le slot
    public void Init(InventoryUI inventoryUI, int index, InventorySlot slot)
    {
        ui      = inventoryUI;
        myIndex = index;

        bool hasItem = slot != null && slot.data != null;

        // Sprite et interactivité
        icon.enabled = hasItem;
        if (hasItem)
            icon.sprite = slot.data.icon;
        GetComponent<Button>().interactable = hasItem;

        // Cadre de sélection désactivé
        selectedFrame.enabled = false;

        // Compteur de stack en haut‑droite
        if (hasItem && slot.count > 1)
        {
            countLabel.enabled = true;
            countLabel.text    = slot.count.ToString();
        }
        else
        {
            countLabel.enabled = false;
        }

        // Échelle individuelle
        if (hasItem)
        {
            icon.rectTransform.localScale = Vector3.one * slot.data.iconScale;
        }
    }

    // Appelé par le Button.onClick()
    public void OnClick()
    {
        ui.OnItemClicked(myIndex);
    }
}