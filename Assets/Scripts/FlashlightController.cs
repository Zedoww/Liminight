using UnityEngine;

public class FlashlightController : MonoBehaviour, IEquipable, IUsable
{
    [SerializeField] Light spot;   // référence la Spot Light du prefab
    bool isOn;

    void Awake() => spot.enabled = false;

    public void OnEquip() { spot.enabled = isOn; }
    public void OnUnequip() { spot.enabled = false; }

    public void Use()
    {
        isOn = !isOn;
        spot.enabled = isOn;
    }
}
