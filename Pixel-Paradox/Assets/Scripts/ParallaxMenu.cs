using UnityEngine;

public class ParallaxMenu : MonoBehaviour
{
    public RectTransform[] layers;
    public float[] movementStrength;

    Vector2 screenCenter;

    void Start()
    {
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 offset = (mousePos - screenCenter) / screenCenter;

        for (int i = 0; i < layers.Length; i++)
        {
            float strength = movementStrength[i];
            layers[i].anchoredPosition = offset * strength;
        }
    }
}