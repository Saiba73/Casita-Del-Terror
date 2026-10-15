using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class linterna : MonoBehaviour
{
    PlayerInput playerInput;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Debug.Log(playerInput.name);
    }

    
    void Update()
    {
        if(playerInput.actions["Btn_Secundario_Derecho"].WasPressedThisFrame())
        {
            Debug.Log("HELLO");
        }
    }
}
