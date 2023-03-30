using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
// 분대 시스템. int값을 public으로 선언하여, 생성하고 싶은 몬스터의 수를 입력하면, 분대단위로 움직일 수 있도록 설정.

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;              // 스폰시킬 몬스터의 프리팹
    GameObject enemy;                           // 스폰시킨 후 몬스터를 제어하기 위한 변수
    EnemyBase enemyBase;                        // 델리게이트 연결을 위한 클래스 변수
    Vector3 spawnPoint;                         // 몬스터 시작 위치 제어를 위함. 굳이 있어야 하나 싶기도 함.
    


    private void Awake()
    {
        spawnPoint = transform.GetChild(0).position;            // 본인의 스폰 위치 얻기
        enemy = Instantiate(enemyPrefab, transform);            // 몬스터 생성 후 제어하기위해 enemy 변수에 대입
        enemyBase = enemy.GetComponent<EnemyBase>();            // 스포너 담당 몬스터의 델리게이트를 듣기위해 컴포넌트 얻어내기
        enemyBase.IAmDied += RespawnEnemy;                      // 컴포넌트 연결. 최초 1회만 진행

    }

    private void Start()
    {
        enemy.transform.position = spawnPoint;                  // 스폰시킨 몬스터의 초기 위치 지정
        
    }

    private void RespawnEnemy()
    {
        StartCoroutine(WaitAndRespawn());                       // 스폰시킨 몬스터로부터 Die 함수에서 발생하는 델리게이트 수신 시, 해당 코루틴이 실행됨.

    }
    IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(5f);                    // 5초 뒤, 몬스터의 위치를 스폰포인트로 이동시키고, 활성화시킴.
        enemy.transform.position = spawnPoint;
        enemy.SetActive(true);
    }

    
    private void OnDrawGizmosSelected()                         
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
   
}
