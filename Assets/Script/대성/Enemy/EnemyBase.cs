using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //컴포넌트 이름은 컴포넌트 명의 축소형
    [Header("Component")]
    protected Animator anim;
    protected GameObject player;
    protected NavMeshAgent agent;
    protected SphereCollider detectRangeCollider;
    Spawner spawner;
    EnemyDetector detector;
    Transform spownPoint;

    [Header("Enemy Information")]
    public int heart;
    public int maxHeart = 3;
    public float enemySpeed = 5f;
    public float normalSpeed = 5f;
    public float chaseSpeed = 8f;
    public float detectRange = 5f;
    public float atackRange = 8f;

    [Header("Scout Information")]
    Vector3[] scoutPoint;
    public int scoutIndex = 0;
    Vector3 targetDirection = Vector3.zero;

    //Flag변수는 항상 is로 시작
    [Header("Flag")]
    public bool isdetectPlayer = false;
    public bool isAtacking = false;
    public bool isWaitScout = false;
    public bool isAtackWaiting = false;
    public bool playerDetect = false;
    public bool idleFlag = false;
    public bool scoutFlag = false;
    public bool chaseFlag = false;
    public bool atackFlag = false;
    public bool drawGizmo = false;
    public bool checkPath = false;

    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float idleWaitTimeMax=3f;            //정찰가기 전 대기시간 3초
    float idleWaitTime;
    public float chaseLimitTimeMax=3f;
    float chaseLimitTime;
    public WaitForSeconds chaseRefreshTime = new WaitForSeconds(0.5f);




    IEnumerator waitAtack;

    [Header("Test")]
    InputSystemController inputController;

    public enum EnemyState
    {
        IDLE,
        SCOUT,
        CHASE,
        ATACKWAIT,
        ATACK,
        NULL
    }
    EnemyState _state=EnemyState.NULL;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (value == EnemyState.IDLE)
            {
                Debug.LogWarning("Idle상태 설정완료. 대기 시작");
                //anim.SetBool("Scout", false);
                //anim.SetBool("Idle", true);
                agent.isStopped= true;

                idleWaitTime = Time.time;
                _state = value;

            }
            else if (value == EnemyState.SCOUT)
            {
                Debug.LogWarning("Scout상태 진입.");
                //anim.SetBool("Idle", false);
                //anim.SetBool("Scout", true);

                Debug.Log($"agent Dest : {scoutPoint[scoutIndex]}");

                checkPath = agent.SetDestination(scoutPoint[scoutIndex]);
                if (checkPath == false)
                    Debug.LogError("경로를 찾을 수 없습니다.");

                agent.isStopped = false;

                scoutIndex++;
                if (scoutIndex == scoutPoint.Length)
                {
                    scoutIndex %= scoutPoint.Length;
                    Debug.Log("scoutIndex가 초기화되었습니다.");
                }

                _state = value;
            }
            else if (value == EnemyState.CHASE)
            {
                Debug.LogWarning("Chase상태 진입.");

                agent.isStopped = false;
                StartCoroutine(ChasePlayerRefresh());
                chaseLimitTime= Time.time;
                _state = value;
            }
            else if (value == EnemyState.ATACK)
            {
                _state = value;
            }
            else if (value == EnemyState.ATACKWAIT)
            {
                _state = value;
            }
        }
    }
    public int Heart
    {
        get => heart;
        set
        {
            heart = value;
            if (heart == 0)
            {
                Die();
            }
        }
    }

    IEnumerator ChasePlayerRefresh()
    {
        while(true)
        {
            Debug.LogWarning("플레이어 추적 갱신");
            checkPath = agent.SetDestination(player.transform.position);        //주기적 목표 갱신
            if (checkPath == false)
                Debug.LogError("경로를 찾을 수 없습니다.");

            yield return chaseRefreshTime;
        }
        

    }


    protected virtual void Die()
    {

    }
    private void Start()
    {
        State = EnemyState.IDLE;
    }

    private void OnEnable()
    {
        RespownSetting();
        detector.detectPlayer += DetectPlayer;
    }
    private void OnDisable()
    {
        player = null;
        detector.detectPlayer -= DetectPlayer;
    }
    protected virtual void RespownSetting()
    {
        heart = maxHeart;
        scoutIndex = 0;
        transform.position = spownPoint.position; ;
    }

    void DetectPlayer(GameObject obj)
    {
        player = obj;
        State = EnemyState.CHASE;
    }


    private void Awake()
    {
        SettingInformation();
        FindComponent();
        SetupPath();
    }
    protected virtual void SettingInformation(){}
    void FindComponent()
    {
        detector = transform.GetComponentInChildren<EnemyDetector>();
        spawner = transform.parent.GetComponent<Spawner>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        detectRangeCollider = GetComponent<SphereCollider>();
    }
    void SetupPath()
    {
        Transform trans = transform.parent;
        spownPoint = trans.GetChild(0).transform;
        scoutPoint = new Vector3[trans.childCount - 2];
        for (int i = 0; i < scoutPoint.Length; i++)
        {
            scoutPoint[i] = trans.GetChild(i + 1).position;
            Debug.Log($"경로설정 <{i}> 완료. {scoutPoint[i]}");
        }
    }
    private void FixedUpdate()
    {
        if (State == EnemyState.IDLE)
            EnemyModeIdle();
        else if (State == EnemyState.SCOUT)
            EnemyModeScout();
        else if (State == EnemyState.CHASE)
            EnemyModeChase();
        else if (State == EnemyState.ATACK)
            EnemyModeAtack();
        else if (State == EnemyState.ATACKWAIT)
            EnemyModeAtackWait();
       
    }

    protected virtual void EnemyModeIdle()
    {
        if(Time.time - idleWaitTime > idleWaitTimeMax )
        {
            Debug.Log("idle 대기 종료. Scout시작");
            State = EnemyState.SCOUT;
        }

    }
    protected virtual void EnemyModeScout()
    {
        if( agent.remainingDistance < 0.1f)
        {
            State = EnemyState.IDLE;
        }

    }
    protected virtual void EnemyModeChase()
    {
        //Debug.Log($"남은 추적거리 : {agent.remainingDistance}");
        //if(Time.time-chaseLimitTime > chaseLimitTimeMax ) 
        //{
        //    State = EnemyState.SCOUT;
        //}     추후, 3초이상 추적했는데도 도달 못하면 초기상태로 돌아가는 코드 구현하기.
        if(agent.remainingDistance<2f)
        {
            Debug.Log("플레이어에게 도착.");
            StopAllCoroutines();
            State = EnemyState.ATACK;
        }
        
    }
    protected virtual void EnemyModeAtackWait()
    {

    }
    protected virtual void EnemyModeAtack()
    {

    }






    private void OnDrawGizmos()
    {
        if(drawGizmo==true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
    
}
