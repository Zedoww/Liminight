using System.Collections;
using UnityEngine;

public class RandomDoor : MonoBehaviour
{
    public Transform doorHinge; // Pivot de la porte
    public float openAngle = 90f;
    public float speed = 2f;
    public float minDelay = 5f;
    public float maxDelay = 15f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorHinge == null)
            doorHinge = transform;

        closedRotation = doorHinge.rotation;
        openRotation = Quaternion.Euler(doorHinge.eulerAngles + new Vector3(0f, openAngle, 0f));
        StartCoroutine(DoorRoutine());
    }

    IEnumerator DoorRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (isOpen)
                yield return StartCoroutine(RotateDoor(openRotation, closedRotation));
            else
                yield return StartCoroutine(RotateDoor(closedRotation, openRotation));

            isOpen = !isOpen;
        }
    }

    IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            doorHinge.rotation = Quaternion.Slerp(from, to, time);
            yield return null;
        }
    }
}
