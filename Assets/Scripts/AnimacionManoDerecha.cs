using UnityEngine;
using UnityEngine.InputSystem;

public class AnimacionManoDerecha : MonoBehaviour
{
    PlayerInput playerinput;
    [SerializeField] RiggeadorManos manoDerecha;
    void Start()
    {
        playerinput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Debug.Log(playerinput.actions["XRI Right Interaction/Select Value"].ReadValue<float>());
        manoDerecha.gripSlider = playerinput.actions["XRI Right Interaction/Select Value"].ReadValue<float>();
        manoDerecha.triggerSlider = playerinput.actions["XRI Right Interaction/Activate Value"].ReadValue<float>();
        manoDerecha.botonAPresionado = playerinput.actions["XRI Right Interaction/BotonesFrontales"].IsPressed();
    }
}
