using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image selectedFrame;

    int index;
    InventoryUI ui;

    public void Init(InventoryUI _ui, int _index, ItemData data)
    {
        ui = _ui;
        index = _index;

        bool hasItem = data != null;
        icon.enabled = hasItem;
        icon.rectTransform.localScale = Vector3.one * data.iconScale;
        if (hasItem) icon.sprite = data.icon;

        GetComponent<Button>().interactable = hasItem;
        selectedFrame.enabled = false;
    }

    public void OnClick() => ui.OnItemClicked(index);

    public void SetSelected(bool on) => selectedFrame.enabled = on;
}
