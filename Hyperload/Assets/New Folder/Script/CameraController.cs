using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public CinemachineVirtualCamera virtualCam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCameraTarget(Transform followTarget)
    {
        if (virtualCam != null)
        {
            virtualCam.Follow = followTarget;
            virtualCam.LookAt = followTarget;
        }
        else
        {
            Debug.LogError("CameraController: Virtual Camera not assigned!");
        }
    }
}
