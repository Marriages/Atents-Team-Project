using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    public Action potionMove;

    public new GameObject camera;
    //Transform cameraTreasurePosition;
    TreasureBoxDetector detector;
    GameObject treasureZoneCover;
    GameObject treasureBoxHinge;
    GameObject treasureBoxLight;
    

    public float hingeOpenSpeed = 30f;
    public float cameraMoveSpeed = 0.05f;



    private void Awake()
    {
        detector = transform.GetChild(3).GetComponent<TreasureBoxDetector>();
        treasureBoxHinge = transform.GetChild(0).gameObject;
        treasureBoxLight = transform.GetChild(2).gameObject;
        treasureZoneCover = transform.GetChild(4).gameObject;
        //cameraTreasurePosition = transform.GetChild(7).transform;
    }

    private void OnEnable()
    {
        detector.treasurezoneEnter += TreasureBoxOpen;

        treasureZoneCover.SetActive(false);
        treasureBoxLight.SetActive(false);
    }

    private void TreasureBoxOpen()
    {
        treasureZoneCover.SetActive(true);
        treasureBoxLight.SetActive(true);
        StartCoroutine(OpenHinge());
        //StartCoroutine(CameraMove());
    }
    IEnumerator OpenHinge()
    {
        float timeCount=0;
        while(timeCount < 90f)
        {
            //Debug.Log(timeCount);
            timeCount += hingeOpenSpeed * Time.deltaTime;
            treasureBoxHinge.transform.rotation *= Quaternion.Euler(-hingeOpenSpeed * Time.deltaTime, 0, 0);
            yield return null;
        }
        potionMove?.Invoke();

    }
    /*
    IEnumerator CameraMove()
    {
        camera.transform.rotation = cameraTreasurePosition.rotation;
        while((cameraTreasurePosition.position - camera.transform.position).sqrMagnitude > 0.1f )
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, cameraTreasurePosition.position, cameraMoveSpeed);
            yield return null;
        }
    }*/
}
