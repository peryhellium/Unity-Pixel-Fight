using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class overheated : MonoBehaviour
{

    public static overheated instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text overheatedMessage;

    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
