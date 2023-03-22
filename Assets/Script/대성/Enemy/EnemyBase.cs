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
    CapsuleCollider enemyCollider;
    public Action IAmDied;

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
    public float atackWaitTimeMax = 2f;
    public float atackWaitTime;
    public float atackStayTImeMax=1f;
    public float atackStayTime;
    public float getHitWaitTimeMax = 1f;
    public float getHitWaitTime;
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
        GETHIT,
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
                //Debug.LogWarning("Idle상태 설정완료. 대기 시작");
                anim.SetBool("Scout", false);
                agent.isStopped= true;

                idleWaitTime = Time.time;
                _state = value;

            }
            else if (value == EnemyState.SCOUT)
            {
                //Debug.LogWarning("Scout상태 진입.");
                anim.SetBool("Scout", true);

                //Debug.Log($"agent Dest : {scoutPoint[scoutIndex]}");

                checkPath = agent.SetDestination(scoutPoint[scoutIndex]);
                if (checkPath == false)
                    Debug.LogError("경로를 찾을 수 없습니다.");

                agent.isStopped = false;

                scoutIndex++;
                if (scoutIndex == scoutPoint.Length)
                {
                    scoutIndex %= scoutPoint.Length;
                    //Debug.Log("scoutIndex가 초기화되었습니다.");
                }

                _state = value;
            }
            else if (value == EnemyState.CHASE)
            {
                //Debug.LogWarning("Chase상태 진입.");
                anim.SetBool("ChasePlayer", true);
                anim.SetBool("ArrivePlayer", false);

                agent.isStopped = false;
                StartCoroutine(ChasePlayerRefresh());
                chaseLimitTime= Time.time;
                _state = value;
            }
            else if (value == EnemyState.ATACKWAIT)
            {
                //Debug.LogWarning("AtackWait.....");
                agent.isStopped = true;
                anim.SetBool("ArrivePlayer", true);
                atackWaitTime = Time.time;
                _state = value;
            }
            else if (value == EnemyState.ATACK)
            {
                //Debug.LogWarning("Atack!!!!!! and Wait..");
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    anim.SetTrigger("Atack1");
                else
                    anim.SetTrigger("Atack2");
                atackStayTime = Time.time;
                _state = value;
            }
            else if (value == EnemyState.GETHIT)
            {
                agent.isStopped = true;
                Heart--;
                if (Heart != 0)
                {
                    anim.SetTrigger("GetHit");
                    getHitWaitTime = Time.time;
                    _state = value;

                }
                else
                {
                    Die();
                }
                //이러면 Heart 프로퍼티는 사용하지 않아도 될 것 같음.
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
                //Die();
            }
        }
    }

    IEnumerator ChasePlayerRefresh()
    {
        while(true)
        {
            //Debug.LogWarning("플레이어 추적 갱신");
            checkPath = agent.SetDestination(player.transform.position);        //주기적 목표 갱신
            if (checkPath == false)
                Debug.LogError("경로를 찾을 수 없습니다.");

            yield return chaseRefreshTime;
        }
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            State = EnemyState.GETHIT;
            StartCoroutine(OneSecondInvincibility());
        }
    }

    // 1초무적!
    IEnumerator OneSecondInvincibility()
    {
        enemyCollider.enabled = false;
        yield return chaseRefreshTime;      //0.5초
        yield return chaseRefreshTime;      //0.5초
        enemyCollider.enabled = true;

    }


    protected virtual void Die()
    {
        Debug.LogError("D I E");
        State = EnemyState.NULL;
        StopAllCoroutines();
        agent.isStopped = true;
        player = null;
        anim.SetTrigger("Die");

        anim.ResetTrigger("GetHit");
        anim.ResetTrigger("Atack1");
        anim.ResetTrigger("Atack2");
        anim.ResetTrigger("Defense");
        anim.ResetTrigger("DefenseHit");
        anim.ResetTrigger("Die");
        anim.ResetTrigger("Restart");
        anim.SetBool("Scout",false);
        anim.SetBool("ArrivePlayer", false);
        anim.SetBool("ChasePlayer", false);



        IAmDied?.Invoke();      //Spawner에게 일정시간 후 다시 부활시켜달라 요청
        StartCoroutine(OneSecondAfterDisable());
        

    }
    IEnumerator OneSecondAfterDisable()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
    private void Start()
    {
        State = EnemyState.IDLE;
        detector.detectPlayer += DetectPlayer;      //문제있을시 OnEnable로옮기기
    }

    private void OnEnable()
    {
        RespownSetting();
    }
    private void OnDisable()
    {
        player = null;
    }
    protected virtual void RespownSetting()
    {
        anim.SetTrigger("Restart");
        State = EnemyState.IDLE;
        heart = maxHeart;
        scoutIndex = 0;
        transform.position = spownPoint.position;
        enemyCollider.enabled = true;
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
        enemyCollider = transform.GetComponent<CapsuleCollider>();
    }
    void SetupPath()
    {
        Transform trans = transform.parent;
        spownPoint = trans.GetChild(0).transform;
        scoutPoint = new Vector3[trans.childCount - 2];
        for (int i = 0; i < scoutPoint.Length; i++)
        {
            scoutPoint[i] = trans.GetChild(i + 1).position;
            //Debug.Log($"경로설정 <{i}> 완료. {scoutPoint[i]}");
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
        else if (State == EnemyState.GETHIT)
            EnemyModeGetHit();

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
            //Debug.Log("플레이어에게 도착.");
            StopAllCoroutines();
            State = EnemyState.ATACKWAIT;
        }
        
    }
    protected virtual void EnemyModeAtackWait()
    {

    }
    protected virtual void EnemyModeAtack()
    {

    }
    protected virtual void EnemyModeGetHit()
    {
        if (Time.time - getHitWaitTime > getHitWaitTimeMax)
            State = EnemyState.ATACKWAIT;

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
