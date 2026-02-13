using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Beállítások")]
    public GameObject container;        
    [SerializeField] private GameObject optionsContainer; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (optionsContainer != null && optionsContainer.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        bool isActive = !container.activeSelf;
        container.SetActive(isActive);


        Time.timeScale = isActive ? 0f : 1f;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        if (optionsContainer != null) optionsContainer.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        if (optionsContainer != null)
        {
            container.SetActive(false);        
            optionsContainer.SetActive(true); 
            Debug.Log("Options megnyitva!");
        }
    }

    public void CloseOptions()
    {
        if (optionsContainer != null)
        {
            optionsContainer.SetActive(false); 
            container.SetActive(true);        
        }
    }


    public void MainMenuButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitButton()
    {
        MainMenuButton();
    }
}