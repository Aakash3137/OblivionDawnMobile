using UnityEngine;

public class FollowTarget3dAudio : MonoBehaviour
{
    public Transform target;
    private Quaternion mainCameraQuaternion;

    private void Start()
    {
        mainCameraQuaternion = Camera.main.transform.rotation;
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x * Mathf.Cos(mainCameraQuaternion.eulerAngles.x), 0, target.position.z * Mathf.Sin(mainCameraQuaternion.eulerAngles.y));
    }
}
