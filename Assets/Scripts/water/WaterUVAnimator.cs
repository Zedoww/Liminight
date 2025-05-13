using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterUVAnimator : MonoBehaviour
{
    [Header("Vitesse de défilement (UV/sec)")]
    public Vector2 baseSpeed = new Vector2(0.02f, 0.01f);  // Albedo / Base Map
    public Vector2 normalSpeed = new Vector2(0.05f, -0.03f);  // Normal Map
    public Vector2 dirtSpeed = new Vector2(-0.01f, 0.00f);  // Texture de saleté (Detail)

    Renderer r;
    Material mat;

    void Awake()
    {
        r = GetComponent<Renderer>();
        mat = r.material;               // instancie le mat pour cet objet
    }

    void Update()
    {
        float t = Time.time;

        // Albedo / couleur
        mat.SetTextureOffset("_BaseMap", t * baseSpeed);

        // Normal map (relief des vaguelettes)
        mat.SetTextureOffset("_BumpMap", t * normalSpeed);

        // Saleté ou mousse (Detail Albedo)  ─►   si tu n’utilises pas de texture de saleté, retire cette ligne
        mat.SetTextureOffset("_DetailAlbedoMap", t * dirtSpeed);
    }
}
