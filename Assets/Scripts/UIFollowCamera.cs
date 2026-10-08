using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        // Automatically find the main camera (Quest XR Camera)
        if (Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Make the canvas face the camera
            transform.LookAt(transform.position + playerCamera.forward);
        }
    }
}