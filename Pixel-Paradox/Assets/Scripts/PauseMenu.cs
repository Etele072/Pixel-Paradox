using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using UnityEngine.InputSystem;  

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject container;
    [SerializeField] private GameObject optionsContainer;

    [Header("Controller Settings")]
    public GameObject firstButtonMainMenu;
    public GameObject firstButtonOptions;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    void Update()
    {
        bool escPressed = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (escPressed || startPressed)
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

        if (isActive)
        {
            SetFocus(firstButtonMainMenu);
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        if (optionsContainer != null) optionsContainer.SetActive(false);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenOptions()
    {
        if (optionsContainer != null)
        {
            LoadVolume();
            container.SetActive(false);
            optionsContainer.SetActive(true);

            SetFocus(firstButtonOptions);
        }
    }

    public void CloseOptions()
    {
        if (optionsContainer != null)
        {
            SaveVolume();
            optionsContainer.SetActive(false);
            container.SetActive(true);

            SetFocus(firstButtonMainMenu);
        }
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("NewMainMenu");
    }

    public void ExitButton()
    {
        MainMenuButton();
    }

    #region Audio Settings
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