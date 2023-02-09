using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraSetup : MonoBehaviour
{
	Animator CameraObject;

    private void Start()
    {
		CameraObject = transform.GetComponent<Animator>();
	}


    public void Position2()
    {
        CameraObject.SetFloat("Play", 1);
    }

    public void Position1()
    {
        CameraObject.SetFloat("Play", 0);
    }

    public void Position4()
    {
        CameraObject.SetFloat("Settings", 1);
    }

    public void Position3()
    {
        CameraObject.SetFloat("Settings", 0);
    }

}
