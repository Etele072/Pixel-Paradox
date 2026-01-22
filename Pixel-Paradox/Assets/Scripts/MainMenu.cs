using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Main Menu");
    }
    public void Play()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
