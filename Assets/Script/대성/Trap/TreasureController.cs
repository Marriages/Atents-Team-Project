using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    public Action EndingCreditStart;
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
        EndingCreditStart?.Invoke();
    }
}
