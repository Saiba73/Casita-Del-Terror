using UnityEngine;

public class PlayerTriggerScreamer : MonoBehaviour
{
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] AudioSource[] audios;
    [SerializeField] GameObject[] animScreamer;
    [SerializeField] GameObject screamerUI;
    [SerializeField] GameObject ganoUI;

    void Start()
    {
        foreach (var go in animScreamer)
            if (go != null) go.SetActive(false);

        if (screamerUI != null) screamerUI.SetActive(false);
        if (ganoUI != null) ganoUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            Trigger(other.gameObject, 0, screamerUI, 0);
        else if (other.CompareTag("Enemy2"))
            Trigger(other.gameObject, 1, screamerUI, 0);
        else if (other.CompareTag("Gano"))
            Trigger(null, 0, ganoUI, 1);
    }

    void Trigger(GameObject enemy, int animIndex, GameObject ui, int audioIndex)
    {
        Debug.Log($"Screamer triggered by {(enemy ? enemy.name : "Gano")}");

        if (enemy != null) Destroy(enemy);

        if (ui != null) ui.SetActive(true);
        else Debug.LogError("UI reference is missing!", this);

        if (animIndex < animScreamer.Length && animScreamer[animIndex] != null)
            animScreamer[animIndex].SetActive(true);
        else
            Debug.LogError($"AnimScreamer[{animIndex}] is missing!", this);

        if (audioIndex < audios.Length && audios[audioIndex] != null)
        {
            audios[audioIndex].mute = false;
            audios[audioIndex].Play();
        }
    }
}