using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Audio;

public class UIcontroller : MonoBehaviour
{

    public static UIcontroller instance;


    private void Awake()
    {
        instance = this;
    }

    public TMP_Text overheatedMessage;
    public Slider TempSlider;
    

    public Image crosshair;

    public GameObject deathScreen;
    public TMP_Text deathText;
    public TMP_Text timerText;

    public TMP_Text healthNumber;

    public TMP_Text killsText, deathsText;

    public GameObject leaderboard;

    public LeaderboardPlayer leaderboardPlayerDisplay;

    public GameObject endScreen;

    public GameObject settingsScreen;

    public GameObject optionScreen;

    public AudioMixer audioMixerSound;
    public AudioMixer audioMixerMusic;
    public TMP_Dropdown resolutionDropown;
    Resolution[] resolutions;
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropown.AddOptions(options);
        resolutionDropown.value = currentResolutionIndex;
        resolutionDropown.RefreshShownValue();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideMenu();
        }

    }

    public void ShowHideMenu()
    {

        if (!settingsScreen.activeInHierarchy)
        {
            settingsScreen.SetActive(true);
        } else if (optionScreen.activeInHierarchy && settingsScreen.activeInHierarchy)
        {
            optionScreen.SetActive(false);
        } else if (!optionScreen.activeInHierarchy && settingsScreen.activeInHierarchy)
        {
            settingsScreen.SetActive(false);
        }

        if ((settingsScreen.activeInHierarchy || optionScreen.activeInHierarchy) && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState == CursorLockMode.None)
        {
            if (!settingsScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void Options()
    {
        //settingsScreen.SetActive(false);
        optionScreen.SetActive(true);
    }

    public void Back()
    {
        optionScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolumeSound(float volume)
    {
        audioMixerSound.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeMusic(float volume)
    {
        audioMixerMusic.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
