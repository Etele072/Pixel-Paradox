using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Beállítások")]
    public Camera cam;            
    public float parallaxEffect;    

    private float length, startPos;
    private float startY;

    void Start()
    {
        startPos = transform.position.x;
        startY = transform.position.y;

        if (GetComponent<SpriteRenderer>() != null)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else
        {
            length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        }
    }

    void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        Vector3 targetPos = new Vector3(startPos + dist, startY, transform.position.z);
        transform.position = targetPos;
        if (temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }
}