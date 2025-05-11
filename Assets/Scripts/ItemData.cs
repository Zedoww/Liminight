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

    [Header("Description")]
    [Tooltip("Description courte affichée lors de l'interaction avec l'objet dans l'inventaire")]
    [TextArea(2, 4)]
    public string shortDescription = "";
    
    [Header("Contenu (pour documents)")]
    [Tooltip("Contenu textuel détaillé pour les documents, lettres, livres, etc.")]
    [TextArea(5, 20)]
    public string documentContent = "";
    
    [Header("Hand Offset")]
    public Vector3   holdPositionOffset = Vector3.zero;
    public Vector3   holdRotationOffset = Vector3.zero;
}