using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    CinemachineVirtualCamera v1;
    CinemachineVirtualCamera v2;
    

    TestPlayer player;

    private void Awake()
    {
        player = FindObjectOfType<TestPlayer>();

        v1 = FindObjectOfType<VirtualCamera1>().GetComponent<CinemachineVirtualCamera>();
        v1.LookAt = player.transform.GetChild(2).transform;
        v1.Follow = player.transform.GetChild(2).transform;
        try
        {
            v2 = FindObjectOfType<VirtualCamera2>().GetComponent<CinemachineVirtualCamera>();
            v2.LookAt = player.transform.GetChild(2).transform;
            v2.Follow = player.transform.GetChild(2).transform;
        }
        catch(System.NullReferenceException)
        {
            Debug.Log("두번째 카메라 찾기 실패!");
            v2 = null;
        }
    }

    private void OnEnable()
    {
        player.CameraChange += CameraUpdate;
    }
    private void OnDisable()
    {
        player.CameraChange -= CameraUpdate;
    }

    void CameraUpdate()
    {
        int temp;

        temp = v1.Priority;
        v1.Priority = v2.Priority;
        v2.Priority = temp;
        
    }

}
