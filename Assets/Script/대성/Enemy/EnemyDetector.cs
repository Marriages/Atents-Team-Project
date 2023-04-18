using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Action<GameObject> detectPlayer;                     // EnemyBase에 델리게이트를 송신하기 위한 목적의 액션
    public SphereCollider col;                                  // 콜라이더를 끄고켜기 위한 목적의 변수
    Spawner spawner;
    EnemyBase eb;


    private void Awake()
    {
        col = transform.GetComponent<SphereCollider>();         // enable을 켜고 끄기위해 컴포넌트 받아옴.
        spawner = transform.parent.parent.GetComponent<Spawner>();
        eb = transform.parent.GetComponent<EnemyBase>();
    }
    private void OnEnable()
    {
        eb.detectorEnable += ReEnableCollier;
    }
    private void Start()
    {
        col.enabled = true;
    }

    void ReEnableCollier()
    {
        //Debug.Log("Detector 재설정.");
        col.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayer?.Invoke(other.gameObject);             // 플레이어 발견시, 플레이어 정보를 담은 델리게이트를 EnemyBase에 송신함.
            col.enabled= false;                                 // 추가 감지를 막기위해 디텍터 콜라이더 비활성화
        }
            
    }
/*
    private void OnDrawGizmos()
    {
        if (col.enabled == true)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
    }*/
}
