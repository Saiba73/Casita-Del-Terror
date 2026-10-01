using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerScreamer : MonoBehaviour
{
    
    [SerializeField] CapsuleCollider collider;
    [SerializeField] AudioSource[] Audios;
    [SerializeField] RawImage[] imageScreamer;

    void Start()
    {
        //collider = GetComponent<CapsuleCollider>();
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
            Audios[0].mute = false;
            Audios[0].Play();
        }
        else if (other.CompareTag("Gano"))
        {
            imageScreamer[1].enabled = true;
            Audios[1].mute = false;
            Audios[1].Play();
        }
    }
}
