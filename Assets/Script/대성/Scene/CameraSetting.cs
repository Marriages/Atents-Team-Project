using Cinemachine;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    CinemachineVirtualCamera v1;
    Player player;

    private void Awake()
    {
        //Debug.Log("Camera Awak");
        this.player = Player.player;

        v1 = FindObjectOfType<VirtualCamera1>().GetComponent<CinemachineVirtualCamera>();
        v1.LookAt = player.transform.GetChild(2).transform;
        v1.Follow = player.transform.GetChild(2).transform;
    }

}
