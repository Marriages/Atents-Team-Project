using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    FireTrapDetector detector;
    Transform upFireTrap;
    Transform downFireTrap;
    public GameObject fireBullet;
    GameObject tempObject;
    WaitForSeconds fireInterval = new WaitForSeconds(0.6f);


    private void Awake()
    {
        detector = transform.GetChild(2).GetComponent<FireTrapDetector>();
        upFireTrap = transform.GetChild(0).GetChild(0).transform;
        downFireTrap = transform.GetChild(1).GetChild(0).transform;
    }
    private void OnEnable()
    {
        detector.playerDetect += OperateFireTrap;
    }
    private void OnDisable()
    {
        detector.playerDetect -= OperateFireTrap;
        StopAllCoroutines();
    }


    private void OperateFireTrap(bool isDetect)
    {
        if(isDetect==true)
        {
            StartCoroutine(FireBullet(isDetect));
        }
        else
        {
            StopAllCoroutines();
        }
    }

    IEnumerator FireBullet(bool isDetect)
    {
        // 추후 최적화 작업 매우매우 필요함. 일단은 돌아가게 코드만 작성해두었음.
        while(isDetect)
        {
            tempObject = Instantiate(fireBullet);
            tempObject.transform.position = upFireTrap.position;
            tempObject.transform.LookAt(Vector3.right);
            yield return fireInterval;
            tempObject = Instantiate(fireBullet);
            tempObject.transform.position = downFireTrap.position;
            tempObject.transform.LookAt(Vector3.right);
            yield return fireInterval;
        }
    }
}
