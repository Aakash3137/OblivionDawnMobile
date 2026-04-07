using Unity.VisualScripting;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        transform.forward = -cam.transform.forward;
    }
}