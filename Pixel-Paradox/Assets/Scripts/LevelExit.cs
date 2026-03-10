using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelExit : MonoBehaviour
{
    public string nextLevelName;

    private bool playerInside = false;
    private PlayerInput playerInput;
    private InputAction interactAction;

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
    }

    void Update()
    {
        if (playerInside && interactAction.WasPressedThisFrame())
        {
            SceneManager.LoadScene(nextLevelName);
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