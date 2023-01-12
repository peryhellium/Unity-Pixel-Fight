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

    private Vector2Int resolutionResolver(string resolution){
        string[] temp = resolution.Split(char.Parse("x"));
        return new Vector2Int(int.Parse(temp[0]), int.Parse(temp[1]));
    }

    public void Apply(){
        mainMenuMusic.volume = globalSound.value / 100;
        shootSound.volume = globalSound.value / 100;
        reloadSound.volume = globalSound.value / 100;

        Vector2Int resolution = resolutionResolver(selectedResolution.captionText.text);
        Screen.SetResolution(resolution.x, resolution.y, fullscreenToggle.isOn);
    }
}
