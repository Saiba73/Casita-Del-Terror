using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class AnimacionManos : MonoBehaviour
{
    PlayerInput playerinput;
    [SerializeField] RiggeadorManos manoDerecha;
    [SerializeField] RiggeadorManos manoIzquierda;
    [SerializeField] GameObject SpotLight;

    private bool spotlihgtBool = true;
    void Start()
    {
        playerinput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        //Debug.Log(playerinput.actions["Btn_Secundario_Derecho"] + " ---------------------------------------");
        Debug.Log(playerinput.actions["Select Value Izquierdo"].ReadValue<float>());
        manoIzquierda.gripSlider = playerinput.actions["Select Value Izquierdo"].ReadValue<float>();
        manoIzquierda.triggerSlider = playerinput.actions["Activate Value Izquierdo"].ReadValue<float>();
        //manoIzquierda.botonAPresionado = playerinput.actions["BotonesFrontales Izquierdo"].IsPressed();

        manoDerecha.gripSlider = playerinput.actions["Select Value Derecho"].ReadValue<float>();
        manoDerecha.triggerSlider = playerinput.actions["Activate Value Derecho"].ReadValue<float>();
        //manoDerecha.botonAPresionado = playerinput.actions["BotonesFrontales Derecho"].IsPressed();

        if (playerinput.actions["Btn_Secundario_Derecho"].WasPressedThisFrame())
        {
            Debug.Log("HELLO");
            SpotLight.SetActive(!spotlihgtBool);
            spotlihgtBool = !spotlihgtBool;
        }
    }
}
