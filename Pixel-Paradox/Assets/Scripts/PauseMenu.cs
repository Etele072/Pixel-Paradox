using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Beállítások")]
    public GameObject container;        
    [SerializeField] private GameObject optionsContainer;

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

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
            LoadVolume();
            container.SetActive(false);        
            optionsContainer.SetActive(true); 
            Debug.Log("Options megnyitva!");
        }
    }

    public void CloseOptions()
    {
        if (optionsContainer != null)
        {
            SaveVolume();
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

    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        audioMixer.GetFloat("MasterVolume", out float masterVolume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }
}