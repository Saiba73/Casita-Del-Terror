using UnityEngine;
using UnityEngine.InputSystem;

public class linterna : MonoBehaviour
{
    PlayerInput playerInput;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    
    void Update()
    {
        if(playerInput.actions["button"].WasPressedThisFrame())
        {
            Debug.Log("HELLO");
        }
    }
}
