using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
// 분대 시스템. int값을 public으로 선언하여, 생성하고 싶은 몬스터의 수를 입력하면, 분대단위로 움직일 수 있도록 설정.

public class Spawner : MonoBehaviour
{
    public GameObject respawnEffect;
    public GameObject enemyPrefab;              // 스폰시킬 몬스터의 프리팹
    public float spawnRadius=10f;
    bool playerInPlace=false;
    Vector3 spawnPoint;                         // 몬스터 시작 위치 제어를 위함. 굳이 있어야 하나 싶기도 함.
    GameObject enemy;                           // 스폰시킨 후 몬스터를 제어하기 위한 변수
    EnemyBase enemyBase;                        // 델리게이트 연결을 위한 클래스 변수
    SphereCollider spawnerCollider;
    WaitForSeconds retryRespawnWaitTime = new WaitForSeconds(2f);
    public Action playerOut;
    


    private void Awake()
    {
        spawnPoint = transform.GetChild(0).position;            // 본인의 스폰 위치 얻기
        spawnerCollider = GetComponent<SphereCollider>();
        enemy = Instantiate(enemyPrefab, transform);            // 몬스터 생성 후 제어하기위해 enemy 변수에 대입
        enemyBase = enemy.GetComponent<EnemyBase>();            // 스포너 담당 몬스터의 델리게이트를 듣기위해 컴포넌트 얻어내기
    }
    private void OnEnable()
    {   
        enemyBase.IAmDied += Respawn;                           // Enemy 사망 후, 리스폰을 부탁하는 델리게이트 연결.
    }
    private void OnDisable()
    {
        enemyBase.IAmDied -= Respawn;
    }

    private void Start()
    {
        enemy.transform.position = spawnPoint;                  // 스폰시킨 몬스터의 초기 위치 지정
        spawnerCollider.radius = spawnRadius;                   // 콜라이더 반지름 길이를 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            playerInPlace = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Spawner : 플레이어 나감");
            playerInPlace = false;
            playerOut?.Invoke();
        }

    }
    void Respawn()
    {
        if(playerInPlace == false)              // 만약 플레이어가 범위 밖으로 나간 상황일 경우
        {
            GameObject obj = Instantiate(respawnEffect);
            obj.transform.position = transform.position;
            Destroy(obj, 3f);
            enemy.transform.position = spawnPoint;          // Enemy의 위치를 본인 위치로 변경 후
            enemy.SetActive(true);                          // Enemy를 활성화시킴.
        }
        else                                    // 만약 플레이어가 범위 안에 있는 상황일 경우
        {
            StartCoroutine(RetryRespawn());     // 일정시간 후 다시 본인을 호출하는 재귀함수.
        }
    }

    IEnumerator RetryRespawn()                  // Respawn에 실패했을 경우, 일정 시간 뒤 다시 함수를 호출.
    {
        yield return retryRespawnWaitTime;
        Respawn();
    }

    
    private void OnDrawGizmosSelected()                         
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
   
}
