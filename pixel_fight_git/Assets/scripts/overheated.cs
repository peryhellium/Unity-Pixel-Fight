using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
