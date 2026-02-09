using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("FirstLevel");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
