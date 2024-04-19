using Cinemachine;
using UnityEngine;

public class CameraBehaviors : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void FollowChar(Transform player)
    {
        virtualCamera.Follow = player;
    }
}
