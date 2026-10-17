using UnityEngine;
using UnityEngine.UI;

public class PlayerTriggerScreamer : MonoBehaviour
{
    
    [SerializeField] CapsuleCollider collider;
    [SerializeField] AudioSource[] Audios;
    [SerializeField] GameObject[] AnimScreamer;

    void Start()
    {
        //collider = GetComponent<CapsuleCollider>();
        for (int i = 0; i < AnimScreamer.Length; i++)
        {
            AnimScreamer[0].SetActive(false);
        }
        
    }

    
    //void Update()
    //{
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            AnimScreamer[1].SetActive(true);
            Audios[0].mute = false;
            Audios[0].Play();
        }
        else if(other.CompareTag("Enemy2"))
        {
            Destroy(other.gameObject);
            AnimScreamer[2].SetActive(true);
            Audios[0].mute = false;
            Audios[0].Play();
        }
        else if (other.CompareTag("Gano"))
        {
            AnimScreamer[0].SetActive(true);
            Audios[1].mute = false;
            Audios[1].Play();
        }
    }
}
