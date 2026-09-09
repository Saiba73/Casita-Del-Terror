using UnityEngine;
using UnityEngine.InputSystem;

public class AnimacionManoIzquierda : MonoBehaviour
{
    PlayerInput playerinput;
    [SerializeField] RiggeadorManos manoIzquierda;
    void Start()
    {
        playerinput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        manoIzquierda.gripSlider = playerinput.actions["Select Value"].ReadValue<float>();
        manoIzquierda.triggerSlider = playerinput.actions["Activate Value"].ReadValue<float>();
        manoIzquierda.botonAPresionado = playerinput.actions["BotonesFrontales"].IsPressed();
    }
}
