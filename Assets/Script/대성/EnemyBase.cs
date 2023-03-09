using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    public Action<bool> EnemyDetectPlayer;
    public Rigidbody rigid;

    public float moveSpeed;             // 적 이동속도
    public float coinDropRate;          // 적 사망시 코인 드랍 확률
    public float heartDropRate;         // 적 사망시 코인 드랍 확률
    public int heart;                           // 적 생명력
    public float detectRange;           // 플에이어 감지 거리
    public bool detectPlayer = false;   //플레이어 감지 여부
    public Vector3 returnPosition=Vector3.zero;
    public Vector3 targetDirection= Vector3.zero;

    

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        returnPosition = transform.position;
    }
    private void Start()
    {
        EnemyModeScout();
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * targetDirection);
        //도착하면 멈춰야하는뎅... 도착하면 EnemyModeScout실행하기.

        
        if( (targetDirection- transform.position).magnitude < 1f )
        {
            targetDirection = Vector3.zero;
            StartCoroutine(WaitScout());
            EnemyModeScout();
        }
    }
    IEnumerator WaitScout()
    {
        yield return new WaitForSeconds(5);
    }

    void EnemyModeScout()
    {
        float x = transform.position.x + Random.Range(0f, 10f);
        float z = transform.position.y + Random.Range(0f, 10f);
        Debug.Log($"{gameObject.name}이 {x},{z}로 정찰.");
        targetDirection = new Vector3(x, 0, z).normalized;

    }
    void EnemyModeMove(Vector3 playerPosition)
    {
        targetDirection = (playerPosition - transform.position).normalized;
        //너무 오랫동안 쫓아가지 못하도록 코루틴을 달아서 Return 실행시키기.
        
    }
    void EnemyModeAtack()
    {

    }
    void EnemyModeReturn()
    {
        targetDirection = (returnPosition - transform.position).normalized;
    }
    void EnemyDie()
    {

    }
    void EnemyDamaged(int damageNum)
    {

    }


    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player") && !detectPlayer )
        {
            Debug.Log($"{gameObject.name}이 player를 발견했다.");
            EnemyModeMove(obj.transform.position);
            detectPlayer = true;
            //EnemyDetectPlayer?.Invoke(detectPlayer);
        }
    }
    private void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player") && detectPlayer)
        {
            Debug.Log($"{gameObject.name}에게서 플레이어가 떠났다.");
            EnemyModeReturn();
            detectPlayer = false;
            //EnemyDetectPlayer?.Invoke(detectPlayer);
        }
    }
   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);     //감지범위

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetDirection);
        //Gizmos.DrawSphere(transform.position, detectRange);
    }
    
}
