using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerScreamer : MonoBehaviour
{
    
    [SerializeField] CapsuleCollider collider;
    AudioSource Audios;
    [SerializeField] RawImage[] imageScreamer;

    void Start()
    {
        //collider = GetComponent<CapsuleCollider>();
        Audios = GetComponent<AudioSource>();
        imageScreamer[0].enabled = false;
        imageScreamer[1].enabled = false;
    }

    
    //void Update()
    //{
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLISION");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("ENEMIGO");
            imageScreamer[0].enabled = true;
        }
        else if (other.CompareTag("Gano"))
        {
            imageScreamer[1].enabled = true;
        }
    }
}
