using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Gère l’affichage de l’étage + effets visuels d’une chute d’ascenseur.
/// </summary>
public class ElevatorFall : MonoBehaviour
{
    [Header("Références scène")]
    public TextMeshPro floorDisplay;      // Objet TextMeshPro “Floor”
    public GameObject pointLight;         // Lumière principale
    public GameObject leftLight;          // Lumière gauche
    public GameObject rightLight;         // Lumière droite
    public GameObject electricalSparks;   // Particules (désactivées au départ)

    [Header("Paramètres de timing")]
    public float initialDelay       = 2f;   // Temps avant toute animation
    public float slowStepTime       = 1f;   // 30→25 (descente normale)
    public float numberFlickerDur   = 1.2f; // Clignotement du “25”
    public float flickerMinInt      = 0.05f;// Intervalle mini du flicker numéro
    public float flickerMaxInt      = 0.15f;// Intervalle maxi du flicker numéro
    public float maxFallStepTime    = 0.35f;// Début chute (lent)
    public float minFallStepTime    = 0.05f;// Fin chute (très rapide)

    //──────────────────────────────────────────────────────────────

    void Start() => StartCoroutine(ElevatorSequence());

    IEnumerator ElevatorSequence()
    {
        /* — Phase 0 : petit temps mort — */
        yield return new WaitForSeconds(initialDelay);

        /* — Phase 1 : descente lente 30 → 25 — */
        for (int floor = 30; floor >= 25; floor--)
        {
            UpdateFloorDisplay(floor);
            yield return new WaitForSeconds(slowStepTime);
        }

        /* — Phase 2 : clignotement du numéro “25” — */
        yield return StartCoroutine(NumberFlicker());

        /* — Phase 3 : chute libre accélérée 25 → -10 — */
        electricalSparks.SetActive(true);           // étincelles ON
        bool flickerRun = true;                     // flag pour la coroutine lumière
        StartCoroutine(LightFlicker(() => flickerRun));

        yield return StartCoroutine(AcceleratedFall(25, -10));

        flickerRun = false;                         // stop flicker lumière
        electricalSparks.SetActive(false);          // étincelles OFF

        /* — Phase 4 : arrêt à -10, lumières coupées — */
        UpdateFloorDisplay(-10);
        SetLights(false);
    }

    //─────────────────  FONCTIONS  ───────────────────────────────

    IEnumerator NumberFlicker()
    {
        float t = 0f;
        while (t < numberFlickerDur)
        {
            floorDisplay.enabled = !floorDisplay.enabled;
            float wait = Random.Range(flickerMinInt, flickerMaxInt);
            t += wait;
            yield return new WaitForSeconds(wait);
        }
        floorDisplay.enabled = true; // ré-affiche le “25”
    }

    IEnumerator AcceleratedFall(int startFloor, int endFloor)
    {
        int steps = Mathf.Abs(endFloor - startFloor);
        for (int i = 0; i <= steps; i++)
        {
            int current = startFloor - i;
            UpdateFloorDisplay(current);

            // accélération quadratique : long → court
            float t = (float)i / steps;             // 0 → 1
            float dt = Mathf.Lerp(maxFallStepTime, minFallStepTime, t * t);
            yield return new WaitForSeconds(dt);
        }
    }

    IEnumerator LightFlicker(System.Func<bool> keepGoing)
    {
        while (keepGoing())
        {
            ToggleLights();
            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        }
        SetLights(false); // éteint en fin de chute
    }

    // Helpers
    void ToggleLights() => SetLights(!pointLight.activeSelf);
    void SetLights(bool state)
    {
        pointLight.SetActive(state);
        leftLight.SetActive(state);
        rightLight.SetActive(state);
    }
    void UpdateFloorDisplay(int value) => floorDisplay.text = value.ToString();
}