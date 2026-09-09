using UnityEngine;

public class RiggeadorManos : MonoBehaviour
{
    [Header("Referencia Dedos")]
    // ------------------------------------------------------ GRIP
    [Header("Grip")]
    [SerializeField] Transform middle1;
    [SerializeField] Transform middle2;
    [SerializeField] Transform middle3;

    [SerializeField] Transform ring1;
    [SerializeField] Transform ring2;
    [SerializeField] Transform ring3;

    [SerializeField] Transform pinky1;
    [SerializeField] Transform pinky2;
    [SerializeField] Transform pinky3;

    Vector3 middle1Original;
    Vector3 middle2Original;
    Vector3 middle3Original;

    Vector3 middle1Nuevo;
    Vector3 middle2Nuevo;
    Vector3 middle3Nuevo;

    Vector3 ring1Original;
    Vector3 ring2Original;
    Vector3 ring3Original;

    Vector3 ring1Nuevo;
    Vector3 ring2Nuevo;
    Vector3 ring3Nuevo;

    Vector3 pinky1Original;
    Vector3 pinky2Original;
    Vector3 pinky3Original;

    Vector3 pinky1Nuevo;
    Vector3 pinky2Nuevo;
    Vector3 pinky3Nuevo;

    // ------------------------------------------------------ TRIGGER
    [Header("Trigger")]
    [SerializeField] Transform index1;
    [SerializeField] Transform index2;
    [SerializeField] Transform index3;

    Vector3 index1Original;
    Vector3 index2Original;
    Vector3 index3Original;

    Vector3 index1Nuevo;
    Vector3 index2Nuevo;
    Vector3 index3Nuevo;

    // ------------------------------------------------------ BÓTÓN A / PULGAR
    [Header("Pulgar")]
    [SerializeField] Transform thumb1;
    [SerializeField] Transform thumb2;
    [SerializeField] Transform thumb3;

    Vector3 thumb1Original;
    Vector3 thumb2Original;
    Vector3 thumb3Original;

    Vector3 thumb1Nuevo;
    Vector3 thumb2Nuevo;
    Vector3 thumb3Nuevo;

    float thumbValor;

    [Header("Inputs presión")]

    [Range(0f, 1f)]
    public float gripSlider;
    [Range(0f, 1f)]
    public float triggerSlider;

    public bool botonAPresionado;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grip
        if (middle1)
        {
            middle1Original = middle1.localRotation.eulerAngles;
            middle2Original = middle2.localRotation.eulerAngles;
            middle3Original = middle3.localRotation.eulerAngles;
        }
        if (ring1)
        {
            ring1Original = ring1.localRotation.eulerAngles;
            ring2Original = ring2.localRotation.eulerAngles;
            ring3Original = ring3.localRotation.eulerAngles;
        }
        if (pinky1)
        {
            pinky1Original = pinky1.localRotation.eulerAngles;
            pinky2Original = pinky2.localRotation.eulerAngles;
            pinky3Original = pinky3.localRotation.eulerAngles;
        }
        if (middle1)
        {
            middle1.Rotate(Vector3.back * 60);
            middle2.Rotate(Vector3.back * 60);
            middle3.Rotate(Vector3.back * 60);
        }
        if (ring1)
        {
            ring1.Rotate(Vector3.back * 65);
            ring2.Rotate(Vector3.back * 65);
            ring3.Rotate(Vector3.back * 65);
        }
        if (pinky1)
        {
            pinky1.Rotate(Vector3.back * 70);
            pinky2.Rotate(Vector3.back * 70);
            pinky3.Rotate(Vector3.back * 70);
        }
        if (middle1)
        {
            middle1Nuevo = middle1.localRotation.eulerAngles;
            middle2Nuevo = middle2.localRotation.eulerAngles;
            middle3Nuevo = middle3.localRotation.eulerAngles;
        }
        if (ring1)
        {
            ring1Nuevo = ring1.localRotation.eulerAngles;
            ring2Nuevo = ring2.localRotation.eulerAngles;
            ring3Nuevo = ring3.localRotation.eulerAngles;
        }
        if (pinky1)
        {
            pinky1Nuevo = pinky1.localRotation.eulerAngles;
            pinky2Nuevo = pinky2.localRotation.eulerAngles;
            pinky3Nuevo = pinky3.localRotation.eulerAngles;
        }
        // Trigger
        if (index1)
        {
            index1Original = index1.localRotation.eulerAngles;
            index2Original = index2.localRotation.eulerAngles;
            index3Original = index3.localRotation.eulerAngles;

            index1.Rotate(Vector3.back * 60);
            index2.Rotate(Vector3.back * 60);
            index3.Rotate(Vector3.back * 60);

            index1Nuevo = index1.localRotation.eulerAngles;
            index2Nuevo = index2.localRotation.eulerAngles;
            index3Nuevo = index3.localRotation.eulerAngles;
        }
        // Botón pulgar A
        if (thumb1)
        {
            thumb1.Rotate(Vector3.forward * 5);
            thumb2.Rotate(Vector3.forward * 5);
            thumb3.Rotate(Vector3.forward * 5);

            thumb1Original = thumb1.localRotation.eulerAngles;
            thumb2Original = thumb2.localRotation.eulerAngles;
            thumb3Original = thumb3.localRotation.eulerAngles;

            thumb1.Rotate(Vector3.back * 10);
            thumb2.Rotate(Vector3.back * 10);

            thumb1Nuevo = thumb1.localRotation.eulerAngles;
            thumb2Nuevo = thumb2.localRotation.eulerAngles;
            thumb3Nuevo = thumb3.localRotation.eulerAngles;

            thumbValor = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dist;

        // Grip
        if (middle1)
        {
            dist = middle1Nuevo - middle1Original;
            middle1.localRotation = Quaternion.Euler(middle1Original + dist * gripSlider);
            dist = middle2Nuevo - middle2Original;
            middle2.localRotation = Quaternion.Euler(middle2Original + dist * gripSlider);
            dist = middle3Nuevo - middle3Original;
            middle3.localRotation = Quaternion.Euler(middle3Original + dist * gripSlider);
        }

        if (ring1)
        {
            dist = ring1Nuevo - ring1Original;
            ring1.localRotation = Quaternion.Euler(ring1Original + dist * gripSlider);
            dist = ring2Nuevo - ring2Original;
            ring2.localRotation = Quaternion.Euler(ring2Original + dist * gripSlider);
            dist = ring3Nuevo - ring3Original;
            ring3.localRotation = Quaternion.Euler(ring3Original + dist * gripSlider);
        }

        if (pinky1)
        {
            dist = pinky1Nuevo - pinky1Original;
            pinky1.localRotation = Quaternion.Euler(pinky1Original + dist * gripSlider);
            dist = pinky2Nuevo - pinky2Original;
            pinky2.localRotation = Quaternion.Euler(pinky2Original + dist * gripSlider);
            dist = pinky3Nuevo - pinky3Original;
            pinky3.localRotation = Quaternion.Euler(pinky3Original + dist * gripSlider);
        }

        // Trigger
        if (index1)
        {
            dist = index1Nuevo - index1Original;
            index1.localRotation = Quaternion.Euler(index1Original + dist * triggerSlider);
            dist = index2Nuevo - index2Original;
            index2.localRotation = Quaternion.Euler(index2Original + dist * triggerSlider);
            dist = index3Nuevo - index3Original;
            index3.localRotation = Quaternion.Euler(index3Original + dist * triggerSlider);
        }

        // Botón pulgar A

        if (botonAPresionado)
        {
            thumbValor += Time.deltaTime * 15;
            if (thumbValor > 1)
            {
                thumbValor = 1;
            }
        }
        else
        {
            thumbValor -= Time.deltaTime * 15;
            if (thumbValor < 0)
            {
                thumbValor = 0;
            }
        }

        if (thumb1)
        {
            dist = thumb1Nuevo - thumb1Original;
            thumb1.localRotation = Quaternion.Euler(thumb1Original + dist * thumbValor);
            dist = thumb2Nuevo - thumb2Original;
            thumb2.localRotation = Quaternion.Euler(thumb2Original + dist * thumbValor);
            dist = thumb3Nuevo - thumb3Original;
            thumb3.localRotation = Quaternion.Euler(thumb3Original + dist * thumbValor);
        }
    }
}
