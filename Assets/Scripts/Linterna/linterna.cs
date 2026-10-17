using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class linterna : MonoBehaviour
{

    public bool lightGrab = false;
    public Transform linternaTransform;
    public Transform playerTransform;

    private void Update()
    {
        if(linternaTransform.position.y < 0)
        {
            linternaTransform.position = playerTransform.position + new Vector3(0.0f, 3.0f, 1.5f);
        }
    }


    public void LightGrabbed()
    {
        lightGrab = true;
    }

    public void LightLetGo()
    {
        lightGrab = false;
    }
}
