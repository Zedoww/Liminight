using UnityEngine;

// Fait osciller la caméra selon le mouvement du CharacterController
public class HeadBob : MonoBehaviour
{
    public CharacterController controller;
    public float bobAmplitude = 0.05f;       // mouvement vertical
    public float bobSpeedFactor = 10f;       // vitesse du bob en fonction de la vitesse
    public float bobSideAmplitude = 0.025f;  // mouvement latéral

    private float bobTimer = 0f;
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            Vector3 v = controller.velocity;
            v.y = 0f;

            if (v.magnitude > 0.1f)
            {
                float currentFrequency = v.magnitude * bobSpeedFactor;
                bobTimer = bobTimer + Time.deltaTime * currentFrequency;

                float offsetY = Mathf.Sin(bobTimer) * bobAmplitude;
                float offsetX = Mathf.Cos(bobTimer * 2f) * bobSideAmplitude;

                transform.localPosition = initialPos 
                                        + new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                bobTimer = 0f;
                transform.localPosition = initialPos;
            }
        }
        else
        {
            transform.localPosition = initialPos;
        }
    }
}