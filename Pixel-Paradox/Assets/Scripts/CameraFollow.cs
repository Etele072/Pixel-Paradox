using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Célpont")]
    public Transform target;        // A Player transfrom-ja

    [Header("Beállítások")]
    public float smoothTime = 0.25f; // Minél nagyobb, annál "lustább" a kamera
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Eltolás (hogy ne pont a közepén legyen)

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // Kiszámoljuk a célpozíciót
        Vector3 targetPosition = target.position + offset;

        // Finom átmenet a jelenlegi és a célpozíció között
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}