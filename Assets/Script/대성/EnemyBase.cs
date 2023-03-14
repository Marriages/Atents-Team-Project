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
    Animator anim;
    GameObject player;

    [Header("Enemy Information")]
    private float enemySpeed = 5f;
    public float normalSpeed = 5f;             // 적 이동속도
    public float chaseSpeed = 8f;
    public float coinDropRate;          // 적 사망시 코인 드랍 확률
    public float heartDropRate;         // 적 사망시 코인 드랍 확률
    public int heart = 2;                           // 적 생명력
    public float detectRange = 5;           // 플레이어 감지 거리

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

    // Idea2 상태 들어올떄의 델리게이트, 상대 나올때의 델리게이트.
    //Exit 델리게이트, Enter델리게이트
    // idea1ENUM을 프로퍼티로 설정해서 애니메이션, 상태 컨트롤 할수 있도록 SET처리할 것.
    public enum EnemyState
    {
        Idle,
        Scout,
        Chase,
        Atack,
        Comeback
    }
    EnemyState enemyState;
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void OnEnable()
    {

    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        anim = GetComponent<Animator>();

        Transform trans = transform.GetChild(3);
        scoutPoint = new Vector3[trans.childCount];
        for (int i = 0; i < trans.childCount; i++)
            scoutPoint[i] = trans.GetChild(i).position;

    }
    private void Start()
    {
        scoutPointRoot = 0;
        returnPosition = transform.position;
        targetDirection = scoutPoint[scoutPointRoot];       // 초기 정찰위치 지정
        enemyState = EnemyState.Idle;
    }

    private void FixedUpdate()
    {
        if (enemyState == EnemyState.Idle)
            EnemyModeIdle();
        else if (enemyState == EnemyState.Scout)
            EnemyModeScout();
        else if (enemyState == EnemyState.Chase)
            EnemyModeChaseAndAtack();
        else if (enemyState == EnemyState.Comeback)
            EnemyModeComeback();
    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

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

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    //초기화시키는 구문. 
    virtual protected void EnemyModeIdle()
    {
        Debug.LogWarning("EnemyMode : IDLE");
        scoutPointRoot = 0;
        enemySpeed = normalSpeed;
        callScoutFlag = true;
        targetDirection = returnPosition;
        enemyState = EnemyState.Scout;
    }
    virtual protected void EnemyModeScout()
    {
        if ((targetDirection - transform.position).sqrMagnitude > 0.1f)
        {
            transform.LookAt(targetDirection);
            //Debug.Log("EnemyState : Scout Move");
            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetDirection, enemySpeed * Time.fixedDeltaTime)); ;
        }
        else if ((targetDirection - transform.position).sqrMagnitude < 0.1f)
        {
            //Debug.Log("EnemyState : Scout Move End");
            if (callScoutFlag == true)
            {
                StartCoroutine(EnemyScoutCoroutine());
            }
        }


    }
    virtual protected void EnemyModeChaseAndAtack()
    {
        targetDirection = targetObject.transform.position;
        if ((targetDirection - transform.position).sqrMagnitude > 2f)
        {
            //Debug.Log("EnemyState : Chase Move");
            transform.LookAt(targetDirection);
            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetDirection, enemySpeed * Time.fixedDeltaTime));
        }
        else if ((targetDirection - transform.position).sqrMagnitude < 2f)
        {
            Debug.Log("Process Atack Logic");
            if (atackFlag == true)
            {
                StartCoroutine(EnemyAtackCoroutine());
            }

        }
    }
    virtual protected void EnemyModeComeback()
    {
        Debug.LogWarning("EnemyMode : Comeback");
        if ((targetDirection - transform.position).sqrMagnitude > 0.1f)
        {
            transform.LookAt(targetDirection);
            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetDirection, enemySpeed * Time.fixedDeltaTime)); ;
        }
        else if ((targetDirection - transform.position).sqrMagnitude < 0.1f)
        {
            enemyState = EnemyState.Idle;
        }

    }

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name}이 player를 발견했다.");
            enemySpeed = chaseSpeed;
            StopCoroutine(EnemyScoutCoroutine());
            targetObject = obj.gameObject;
            enemyState = EnemyState.Chase;

        }
    }
    private void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name}에게서 플레이어가 떠났다.");
            StopAllCoroutines();
            targetObject = null;
            atackFlag = true;
            targetDirection = returnPosition;
            enemyState = EnemyState.Comeback;
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

/* 과거의 유산
 
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Action")]
    public Action<bool> EnemyDetectPlayer;

    [Header("Component")]
    public Rigidbody rigid;

    [Header("Enemy Information")]
    public float moveSpeed=5;             // 적 이동속도
    public float coinDropRate;          // 적 사망시 코인 드랍 확률
    public float heartDropRate;         // 적 사망시 코인 드랍 확률
    public int heart;                           // 적 생명력
    public float detectRange=5;           // 플레이어 감지 거리

    [Header("Enemy Scout Root")]
    public Vector3[] scoutPoint;
    public Vector3 returnPosition=Vector3.zero;     //감지를 끝내고 돌아갈 처음 위치
    public Vector3 targetDirection= Vector3.zero;
    public int scoutPointRoot=0;          //정찰할 포인트를 결정지을 변수.
    public bool detectPlayer = false;   //플레이어 감지 여부

    [Header("Enemy Condiction Checking")]
    bool callScoutFlag = true;
    public bool moveStart= false;
    public float scoutWaitTime = 3f;

    Animator anim;
    GameObject player;

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        returnPosition = transform.position;
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
        Transform trans = transform.GetChild(3);
        scoutPoint = new Vector3[trans.childCount];
        for (int i = 0; i < trans.childCount; i++)
        {
            scoutPoint[i] = trans.GetChild(i).position;
        }
    }
    private void Start()
    {
        targetDirection = scoutPoint[scoutPointRoot];       // 초기 정찰위치 지정
        EnemyModeScout();
    }

    private void FixedUpdate()
    {
        transform.LookAt(targetDirection);
        if(detectPlayer==true)
            targetDirection = player.transform.position;
        

        //(목표와 떨어져있고, moveStart가 treu) 또는 플레이어 감지상태라면, 해당 방향으로 계속 이동.
        if ( ((targetDirection - transform.position).sqrMagnitude > 0.25f && moveStart == true ) )
        {
            Debug.Log("플레이어 감지 또는 이동");
            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetDirection, moveSpeed * Time.deltaTime));
        }
        //목표와 가깝지만 플레이어 감지상태가 아닐 경우, 다시 정찰모드.
        else if ((targetDirection - transform.position).sqrMagnitude < 0.25f && detectPlayer==false)       //목표와 떨어져있지만, moveStart가 false라면 정지
        {
            Debug.Log("플레이어 감지X, 대기");
            moveStart = false;
            //Debug.Log($"{scoutPointRoot}번째 목표 도착");
            //rigid.MovePosition(transform.position);
            // EnemyModeScout을 호출하기. 한 번 만
            if (callScoutFlag == true)
            {
                //anim.SetTrigger("Atack");
                callScoutFlag = false;
                EnemyModeScout();
            }
        }
        //목표와 가깝고 플레이어 감지상태인 경우, 공격 실행.
        else if ((targetDirection - transform.position).sqrMagnitude < 0.2f && detectPlayer == true)       //목표와 가깝고, 플레이어를 감지한 상태라면
        {
            Debug.Log("플레이어 감지, 공격!");
            EnemyModeAtack();
        }

    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    
    // 정찰에 사용될 알고리즘 코루틴
    IEnumerator WaitAndTargetSetting()
    {
        yield return new WaitForSeconds(scoutWaitTime);

        
        targetDirection = scoutPoint[scoutPointRoot];
        Debug.Log($"{gameObject.name}이 {scoutPoint[scoutPointRoot]}로 정찰 시작.");

        scoutPointRoot++;
        scoutPointRoot = scoutPointRoot % scoutPoint.Length;
        moveStart = true;
        callScoutFlag = true;
        
    }
    IEnumerator WaitAndTargetAtack()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Atack");
    }

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    void EnemyModeScout()       // 순찰모드
    {
        StartCoroutine(WaitAndTargetSetting());
    }
    void EnemyModeMove(GameObject playerObject)      //플레이어를 발견해서 플레이어 방향으로 이동함.
    {
        StopAllCoroutines();
        player = playerObject;
        moveStart = true;
        targetDirection = player.transform.position;
        
    }
    void EnemyModeAtack()                           //일정거리 도달 후 플레이어를 공격.
    {
        StartCoroutine(WaitAndTargetAtack());


    }
    void EnemyModeReturn()                          //플레이어가 감지 범위 밖으로 벗어나서 순찰모드로 복귀
    {
        moveStart = false;
        EnemyModeScout();
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

            detectPlayer = true;
            EnemyModeMove(obj.gameObject);
            //EnemyDetectPlayer?.Invoke(detectPlayer);
        }
    }
    private void OnTriggerExit(Collider obj)
    {
        if (obj.gameObject.CompareTag("Player") && detectPlayer)
        {
            Debug.Log($"{gameObject.name}에게서 플레이어가 떠났다.");

            detectPlayer = false;
            EnemyModeReturn();
            //EnemyDetectPlayer?.Invoke(detectPlayer);
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

 */