using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [Header("Panel Beállítások")]
    public GameObject mainMenuContainer;  
    public GameObject optionsContainer;    

    [Header("Controller Support - Elsõ kijelölt gombok")]
    public GameObject firstSelectedMain;   
    public GameObject firstSelectedOptions; 

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    private void Start()
    {
        if (MusicManager.Instance != null) MusicManager.Instance.PlayMusic("MainMenu");
        LoadVolume();

        SetFocus(firstSelectedMain);

        if (optionsContainer != null) optionsContainer.SetActive(false);
    }


    public void OpenOptions()
    {
        if (optionsContainer != null)
        {
            mainMenuContainer.SetActive(false);
            optionsContainer.SetActive(true);

            SetFocus(firstSelectedOptions);
        }
    }

    public void CloseOptions()
    {
        if (optionsContainer != null)
        {
            SaveVolume();
            optionsContainer.SetActive(false);
            mainMenuContainer.SetActive(true);

            SetFocus(firstSelectedMain);
        }
    }


    public void OnStartClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    #region Audio Settings (Változatlan)
    public void UpdateMasterVolume(float volume) { audioMixer.SetFloat("MasterVolume", volume); }
    public void UpdateMusicVolume(float volume) { audioMixer.SetFloat("MusicVolume", volume); }
    public void UpdateSoundVolume(float volume) { audioMixer.SetFloat("SFXVolume", volume); }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        audioMixer.GetFloat("MasterVolume", out float masterVolume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
    }
    #endregion

    private void SetFocus(GameObject target)
    {
        if (target != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}