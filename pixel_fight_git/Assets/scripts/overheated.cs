using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class overheated : MonoBehaviour
{

    public static overheated instance;


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
    //public Slider healthSlider;

    public TMP_Text killsText, deathsText;

    public GameObject leaderboard;

    public LeaderboardPlayer leaderboardPlayerDisplay;

    public GameObject endScreen;

    public GameObject settingsScreen;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideMenu();
        }

        if(settingsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowHideMenu()
    {
        if (!settingsScreen.activeInHierarchy)
        {
            settingsScreen.SetActive(true);
        } else
        {
            settingsScreen.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
