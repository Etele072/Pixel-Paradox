using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextLevelName;
    [Header("Requirements")]
    public int requiredCards = 1;

    private bool playerInside = false;
    private PlayerInput playerInput;
    private InputAction interactAction;
    private PlayerMovement playerScript;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerInput = playerObj.GetComponent<PlayerInput>();
            playerScript = playerObj.GetComponent<PlayerMovement>();

            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
            }
        }
    }

    void Update()
    {
        if (playerInside && interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (playerScript != null)
            {
                if (playerScript.cardsCollected >= requiredCards)
                {
                    Debug.Log($"Sikeres szökés! Megvan mind a {playerScript.cardsCollected} kártya.");
                    SceneManager.LoadScene(nextLevelName);
                }
                else
                {
                    int missing = requiredCards - playerScript.cardsCollected;
                    Debug.Log($"Még kellene {missing} kártya a kijárathoz!");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = false;
    }
}