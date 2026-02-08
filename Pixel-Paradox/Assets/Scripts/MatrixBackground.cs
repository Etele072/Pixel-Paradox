using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public partial class MatrixBackground : MonoBehaviour
{
    public GameObject columnPrefab; 
    public int columnCount = 40;    
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    void Start()
    {
        for (int i = 0; i < columnCount; i++)
        {
            StartCoroutine(SpawnColumn(i));
        }
    }

    IEnumerator SpawnColumn(int index)
    {
        yield return new WaitForSeconds(Random.Range(0f, 5f));

        while (true)
        {
            float xPos = (Screen.width / columnCount) * index;
            GameObject col = Instantiate(columnPrefab, transform);
            col.GetComponent<RectTransform>().position = new Vector3(xPos, Screen.height + 100, 0);

            float speed = Random.Range(minSpeed, maxSpeed);
            StartCoroutine(MoveColumn(col, speed));

            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    IEnumerator MoveColumn(GameObject col, float speed)
    {
        RectTransform rect = col.GetComponent<RectTransform>();
        TextMeshProUGUI text = col.GetComponent<TextMeshProUGUI>();

        while (rect.position.y > -200)
        {
            rect.position += Vector3.down * speed;

            if (Random.value > 0.9f)
                text.text = GenerateRandomChars(10);

            yield return null;
        }
        Destroy(col);
    }

    string GenerateRandomChars(int length)
    {
        string chars = "10";
        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)] + "\n";
        }
        return result;
    }
}