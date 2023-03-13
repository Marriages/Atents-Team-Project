using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemyBaseTest : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Action")]
    public Action<bool> EnemyDetectPlayer;

    [Header("Component")]
    public Rigidbody rigid;
    Animator anim;
    GameObject player;
    NavMeshAgent agent;

    [Header("Enemy Information")]
    private float enemySpeed = 5f;
    public float normalSpeed = 5f;             // 적 이동속도
    public float chaseSpeed = 8f;
    public float coinDropRate;          // 적 사망시 코인 드랍 확률
    public float heartDropRate;         // 적 사망시 코인 드랍 확률
    public int heart = 2;                           // 적 생명력

    public float detectRange = 5;           // 플레이어 감지 거리
    public float atackRange = 10;

    [Header("Enemy Scout Root")]
    public Vector3[] scoutPoint;
    public Vector3 returnPosition = Vector3.zero;     //감지를 끝내고 돌아갈 처음 위치
    public Vector3 targetDirection = Vector3.zero;
    public int scoutPointRoot = 0;          //정찰할 포인트를 결정지을 변수.
    public bool detectPlayer = false;   //플레이어 감지 여부

    [Header("Enemy Condiction Checking")]
    bool callScoutFlag = true;
    public bool moveStart = false;
    public float scoutWaitTime = 3f;
    GameObject targetObject = null;

    public float atackDelayTime = 1f;
    private bool atackFlag = true;
    private bool returnFlag = false;

    //animation 제어플레그
    private string aniIdleFlag = "Idle";
    private string aniScoutFlag="Scout";
    private string aniChaseFlag="Chase";
    private string aniAtackMeleeFlag = "AtackMelee";
    private string aniAtackBulletFlag = "AtackBullet";

    // Idea2 상태 들어올떄의 델리게이트, 상대 나올때의 델리게이트.
    //Exit 델리게이트, Enter델리게이트
    // idea1ENUM을 프로퍼티로 설정해서 애니메이션, 상태 컨트롤 할수 있도록 SET처리할 것.
    enum EnemyState
    {
        IDLE,
        SCOUT,
        CHASE,
        ATACK_MELEE,
        ATACK_BULLET
            
    }
    EnemyState _state;

    EnemyState State
    {
        get => _state;

        set
        {
            if (value == EnemyState.IDLE)
            {
                anim.SetTrigger(aniIdleFlag);
            }
            else if(value== EnemyState.SCOUT)
            {
                
                scoutPointRoot++;
                if (scoutPointRoot == scoutPoint.Length)
                {   //정찰지점의0번째가 아닌 1번째로 돌림.( 0번째는 초기 위치 )
                    scoutPointRoot %= scoutPoint.Length + 1;
                }
                targetDirection = scoutPoint[scoutPointRoot];
                anim.SetTrigger(aniScoutFlag);
            }
            else if(value==EnemyState.CHASE)
            {
                anim.SetTrigger(aniChaseFlag);
            }
            else if(value==EnemyState.ATACK_MELEE)
            {
                anim.SetTrigger(aniAtackMeleeFlag);
            }
            else if (value == EnemyState.ATACK_BULLET)
            {
                anim.SetTrigger(aniAtackBulletFlag);
            }

            _state = value;
        }
    }

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------
    private void OnEnable()
    {
        Transform trans = transform.GetChild(3);
        returnPosition = transform.position;

        scoutPoint = new Vector3[trans.childCount + 1];
        scoutPoint[0] = returnPosition;
        for (int i = 1; i < trans.childCount; i++)
            scoutPoint[i] = trans.GetChild(i).position;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        scoutPointRoot = 0;
        State = EnemyState.IDLE;
    }

    private void FixedUpdate()
    {
        if(State == EnemyState.IDLE)        //대기모드.
            EnemyModeIdle();
        else if(State==EnemyState.SCOUT)     //기존의 정찰모드. 복귀모드도 합칠 것.
            EnemyModeScout();
        else if (State==EnemyState.CHASE)   //적을 향해 달려가는 모드. 도착시 ATACK
            EnemyModeChase();
        else if (State==EnemyState.ATACK_MELEE)  //멀면 원거리, 가까우면 근거리 공격 할 것
            EnemyModeAtackMelee();
        else if (State == EnemyState.ATACK_BULLET)  //멀면 원거리, 가까우면 근거리 공격 할 것
            EnemyModeAtackBullet();
    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    //초기화시키는 구문. 
    virtual protected void EnemyModeIdle()
    {
        if(returnFlag==true)
        {
            agent.SetDestination(returnPosition);
            if(agent.velocity.sqrMagnitude<0.4f)
            {
                scoutPointRoot = 0;
                returnFlag = false;
            }
        }
        else
        {
            StartCoroutine(EnemyIdleCoroutine());
            //이후 별 문제가 없을 경우 Scout 모드로 들어가게 됨.
        }
    }
    virtual protected void EnemyModeScout()
    {
        agent.SetDestination(targetDirection);
        
        if(agent.velocity.sqrMagnitude<0.1f)        //거의 멈춘상태라면
            State = EnemyState.IDLE;
    }
    virtual protected void EnemyModeChase()
    {
    }
    virtual protected void EnemyModeAtackMelee()
    {
    }
    virtual protected void EnemyModeAtackBullet()
    {
    }

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------


    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------
    IEnumerator EnemyIdleCoroutine()
    {
        yield return new WaitForSeconds(2f);

        State = EnemyState.SCOUT;
    }
    IEnumerator EnemyScoutCoroutine()
    {
        Debug.Log("EnemyState : Start EnemyScoutCoroutine");
        callScoutFlag = false;
        Debug.Log("EnemyState : ScoutCoroutine.....");
        yield return new WaitForSeconds(scoutWaitTime);
        targetDirection = scoutPoint[scoutPointRoot];
        scoutPointRoot++;
        if (scoutPointRoot == scoutPoint.Length)
            scoutPointRoot %= scoutPoint.Length;
        callScoutFlag = true;
    }
    IEnumerator EnemyAtackCoroutine()
    {
        Debug.LogWarning("Start ATack Coroutine");
        atackFlag = false;
        enemySpeed = 0f;
        anim.SetTrigger("Atack");
        yield return new WaitForSeconds(atackDelayTime);
        atackFlag = true;
        enemySpeed = chaseSpeed;
        Debug.LogWarning("End Atack Coroutine");
    }



    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------


    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player") )
        {
            Debug.Log($"{gameObject.name}이 player를 발견했다.");
            enemySpeed = chaseSpeed;
            StopCoroutine(EnemyScoutCoroutine());
            targetObject = obj.gameObject;
            State = EnemyState.CHASE;

        }
    }
    private void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player") )
        {
            Debug.Log($"{gameObject.name}에게서 플레이어가 떠났다.");
            returnFlag = true;
            State = EnemyState.IDLE;
        }
    }

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    //------------------------GIZMO------------------------------------------------GIZMO------------------------------------------------GIZMO------------------------

    private void OnDrawGizmos()
    {
        //적의 감지범위를 파랑 구로 표현.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);     //감지범위

        //적의 현재 위치-목적지 까지를 빨간 선으로 표현
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetDirection);
    }
    //------------------------GIZMO------------------------------------------------GIZMO------------------------------------------------GIZMO------------------------
}
