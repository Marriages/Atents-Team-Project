using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSetting : MonoBehaviour
{
    CinemachineVirtualCamera v1;
    CinemachineVirtualCamera v2;
    


    TestPlayer player;

    private void Awake()
    {
        //Debug.Log("Camera Awak");
        this.player = TestPlayer.player;

        v1 = FindObjectOfType<VirtualCamera1>().GetComponent<CinemachineVirtualCamera>();
        v1.LookAt = player.transform.GetChild(2).transform;
        v1.Follow = player.transform.GetChild(2).transform;
        try
        {
            v2 = FindObjectOfType<VirtualCamera2>().GetComponent<CinemachineVirtualCamera>();
            v2.LookAt = player.transform.GetChild(2).transform;
            v2.Follow = player.transform.GetChild(2).transform;
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("두번째 카메라 찾기 실패!");
            v2 = null;
        }
    }

    //아니왜 Title -> Main -> Shop -> Main하면 Missing이고
    // Main -> Shop -> Main하면 제대로 잡히는거지? ㄹㅇ루다가알수강벗네 시팝ㄹ

    private void OnEnable()
    {
        //Debug.Log("Camera Enabled");
        player.CameraChange += CameraUpdate;
        SceneManager.sceneLoaded += CameraFindRetry;

        
    }

    private void CameraFindRetry(Scene scene, LoadSceneMode sceneMode)
    {/*
        //Debug.Log("Camera Scene Change");
        player = FindObjectOfType<TestPlayer>();
        player.CameraChange += CameraUpdate;

        v1 = FindObjectOfType<VirtualCamera1>().GetComponent<CinemachineVirtualCamera>();
        v1.LookAt = player.transform.GetChild(2).transform;
        v1.Follow = player.transform.GetChild(2).transform;
        try
        {
            v2 = FindObjectOfType<VirtualCamera2>().GetComponent<CinemachineVirtualCamera>();
            v2.LookAt = player.transform.GetChild(2).transform;
            v2.Follow = player.transform.GetChild(2).transform;
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("두번째 카메라 찾기 실패!");
            v2 = null;
        }*/
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
