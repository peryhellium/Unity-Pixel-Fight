using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource soundHover;
    public AudioSource soundClick;

    public void PlayHover()
    {
        soundHover.Play();
    }

    public void PlayClick()
    {
        soundClick.Play();
    }

}
