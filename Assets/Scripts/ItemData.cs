using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string    itemName;
    public Sprite    icon;
    public GameObject prefab;

    [Header("UI Settings")]
    [Range(0.2f, 1f)]
    [Tooltip("Échelle du sprite dans l'inventaire (1 = plein slot)")]
    public float iconScale = 1f;

    [Header("Hand Offset")]
    public Vector3   holdPositionOffset = Vector3.zero;
    public Vector3   holdRotationOffset = Vector3.zero;
}