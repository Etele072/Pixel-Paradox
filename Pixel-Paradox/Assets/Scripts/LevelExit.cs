using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextLevelName;

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
                interactAction = playerInput.actions["Interact"];
        }
    }

    void Update()
    {
        if (playerInside && interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (playerScript != null && playerScript.hasCard)
            {
                Debug.Log("Sikeres kijutás kártyával!");
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.Log("Nincs nálad a kártya, nem mehetsz át!");
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