using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Action")]
    public Action<bool> EnemyDetectPlayer;

    [Header("Component")]
    public Rigidbody rigid;

    [Header("Enemy Information")]
    public float moveSpeed;             // 적 이동속도
    public float coinDropRate;          // 적 사망시 코인 드랍 확률
    public float heartDropRate;         // 적 사망시 코인 드랍 확률
    public int heart;                           // 적 생명력
    public float detectRange;           // 플에이어 감지 거리
    public bool detectPlayer = false;   //플레이어 감지 여부
    public Vector3 returnPosition=Vector3.zero;
    public Vector3 targetDirection= Vector3.zero;



    // --- test value
    bool testFlag = true;
    int testPointNum;
    Vector3[] testPoint = new Vector3[5];
    //float x = 5f;
    //float z = 5f;
    public bool moveStart=false;
    // --- test value

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        returnPosition = transform.position;
    }
    private void Start()
    {
        testPointNum = 0;
        testPoint[0] = new Vector3(5,0,5);
        testPoint[1] = new Vector3(5,0,0);
        testPoint[2] = new Vector3(0,0,5);
        testPoint[3] = new Vector3(-5, 0, 5);
        testPoint[4] = new Vector3(-5, 0, 0);

        //EnemyModeScout();  이새끼가 처음부터 플래그들을 다 망쳐놨네 시발

        StartCoroutine(TestCorutine());
    }
    private void FixedUpdate()
    {
        
        if ( (targetDirection - transform.position).sqrMagnitude > 0.25f && moveStart==true)       //목표와 떨어져있고, moveStart가 treu라면 
        {
            //transform.LookAt(-targetDirection);
            rigid.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * targetDirection.normalized);
        }
        else if((targetDirection - transform.position).sqrMagnitude < 0.25f )       //목표와 떨어져있지만, moveStart가 false라면 정지
        {
            moveStart = false;
            Debug.Log($"{testPointNum}번째 목표 도착");
            //rigid.MovePosition(transform.position);
            // EnemyModeScout을 호출하기. 한 번 만
            if(testFlag==true)
            {
                Debug.Log("testFlag Enter");
                testFlag = false;
                EnemyModeScout();
            }
            
        }

        //Vector3.Lerp(transform.position, targetDirection , Time.fixedDeltaTime ));
        //도착하면 멈춰야하는뎅... 도착하면 EnemyModeScout실행하기.
        /*if((targetDirection - transform.position).magnitude>0.5f && moveStart )       //목표까지 남은 거리가 0.5보다 크다면 계속 이동
        {
            transform.LookAt(targetDirection);
            rigid.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * targetDirection.normalized);
        }
        else
        {
            moveStart = false;
            EnemyModeScout();
        }
        if( (targetDirection- transform.position).magnitude < 1f )                  //목적지까지 도착했는지 ?
        {
            targetDirection = Vector3.zero;
            StartCoroutine(WaitScout());
            EnemyModeScout();
        }*/
    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    IEnumerator TestCorutine()
    {
        //Vector3.MoveTowards(transform.position, targetDirection, 0.1f);
        while (true)
        {
            Debug.Log($"현재위치 : {transform.position}, 목표벡터: {targetDirection-transform.position} / 남은거리 : {(targetDirection-transform.position).magnitude} / T3 :");
            yield return new WaitForSeconds(1);
            
        }
    }

    IEnumerator WaitThreeSecond()
    {
        Debug.Log("코루틴 시작");
        yield return new WaitForSeconds(5f);
        Debug.Log("코루틴 끝");
    }

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    void EnemyModeScout()       // 순찰모드
    {
        //float x = transform.position.x + Random.Range(0f, 10f);
        //float z = transform.position.y + Random.Range(0f, 10f);

        //0,0,0 -> 5,0,5로 이동하는 것 테스트하기.
        Debug.Log($"{gameObject.name}이 {testPoint[testPointNum]}로 정찰.");
        StartCoroutine(WaitThreeSecond());
        // 기다린 후 위치값을 바꿔야 update가 돌아가지 않는다.

        targetDirection = testPoint[testPointNum] - transform.position;         //목적지까지의 방향벡터 설정 -> Fixed Update에 영향

        
        testPointNum++;
        moveStart = true;
        testFlag = true;
        Debug.Log("moveStart 플래그 On");

    }
    void EnemyModeMove(Vector3 playerPosition)      //플레이어를 발견해서 플레이어 방향으로 이동함.
    {
        
    }
    void EnemyModeAtack()                           //일정거리 도달 후 플레이어를 공격.
    {

    }
    void EnemyModeReturn()                          //플레이어가 감지 범위 밖으로 벗어나서 순찰모드로 복귀
    {
        targetDirection = (returnPosition - transform.position).normalized;
    }
    void EnemyDie()
    {

    }
    void EnemyDamaged(int damageNum)
    {

    }
    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

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

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    //------------------------GIZMO------------------------------------------------GIZMO------------------------------------------------GIZMO------------------------

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);     //감지범위

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetDirection);
        //Gizmos.DrawSphere(transform.position, detectRange);
    }
    //------------------------GIZMO------------------------------------------------GIZMO------------------------------------------------GIZMO------------------------

}
