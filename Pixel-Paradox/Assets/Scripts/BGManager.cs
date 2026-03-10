using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    public Transform cam;

    [System.Serializable]
    public class Layer
    {
        public Transform parent;
        public float parallaxSpeed;
    }

    public Layer[] layers;

    float camStartX;

    void Start()
    {
        camStartX = cam.position.x;
    }

    void LateUpdate()
    {
        float camDelta = cam.position.x - camStartX;

        foreach (Layer layer in layers)
        {
            float move = camDelta * layer.parallaxSpeed;
            layer.parent.position = new Vector3(move, layer.parent.position.y, layer.parent.position.z);

            Transform bg1 = layer.parent.GetChild(0);
            Transform bg2 = layer.parent.GetChild(1);

            float width = bg1.GetComponent<SpriteRenderer>().bounds.size.x;

            // JOBBRA loop
            if (cam.position.x - bg1.position.x > width)
            {
                bg1.position = new Vector3(bg2.position.x + width, bg1.position.y, bg1.position.z);
            }

            if (cam.position.x - bg2.position.x > width)
            {
                bg2.position = new Vector3(bg1.position.x + width, bg2.position.y, bg2.position.z);
            }

            // BALRA loop
            if (cam.position.x - bg1.position.x < -width)
            {
                bg1.position = new Vector3(bg2.position.x - width, bg1.position.y, bg1.position.z);
            }

            if (cam.position.x - bg2.position.x < -width)
            {
                bg2.position = new Vector3(bg1.position.x - width, bg2.position.y, bg2.position.z);
            }
        }
    }
}