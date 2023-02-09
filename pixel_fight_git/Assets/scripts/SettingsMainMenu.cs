using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SettingsMainMenu : MonoBehaviour
{
    public Slider globalSound;
    public AudioSource mainMenuMusic;
    public AudioSource shootSound;
    public AudioSource reloadSound;
    public TMP_Dropdown selectedResolution;
    public Toggle fullscreenToggle;

    public void Start()
    {
        float volume = 15;
        globalSound.value = volume;
        mainMenuMusic.volume = volume / 100;
        shootSound.volume = volume / 100;
        reloadSound.volume = volume / 100;

        Screen.SetResolution(1920, 1080, false);
        selectedResolution.value = 4;

        fullscreenToggle.isOn = false;
    }

    private Vector2Int ResolutionResolver(string resolution){
        string[] temp = resolution.Split(char.Parse("x"));
        return new Vector2Int(int.Parse(temp[0]), int.Parse(temp[1]));
    }

    public void Apply(){
        mainMenuMusic.volume = globalSound.value / 100;
        shootSound.volume = globalSound.value / 100;
        reloadSound.volume = globalSound.value / 100;

        Vector2Int resolution = ResolutionResolver(selectedResolution.captionText.text);
        Screen.SetResolution(resolution.x, resolution.y, fullscreenToggle.isOn);
    }
}
