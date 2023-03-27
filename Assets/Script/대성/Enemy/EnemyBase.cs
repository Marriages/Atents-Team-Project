using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;

public class EnemyBase : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Component")]       //각종 컴포넌트정보를 담은 헤더.
    protected Animator anim;                    // 애니메이션 컨트롤러
    protected GameObject player;            // 목표(플레이어)를 추적하기 위하여 사요오딤
    protected NavMeshAgent agent;        // 길찾기 Navigation에 사용될 변수
    EnemyDetector detector;                    // 자식오브젝트 Detector로부터 델리게이트를 수신하기 위함
    Transform spownPoint;                       // 초기 
    Collider enemyCollider;                      // 무적시간 설정을 위하여 선언.
    public Action IAmDied;                       // Spawner에게 죽었다는 것을 알리고, 새로 Enable시키기 위함.

    [Header("Enemy Information")]          // 해당 객체가 Enable되었을 때 셋팅할 SettingInformation()에 들어가게 될 변수들.
    public int heart;                                  // 현재 생명력
    public int maxHeart;                           // 최대 생명력. Enable 되었을 때 heart를 초기화할 목적으로 선언됨.
    public float enemySpeed;                  // 현재 속도를 제어하기 위하여 설정됨. normalSpeed 는 정찰속도 / chaseSpsed는 추적 속도로 사용됨
    public float normalSpeed;                  // 정찰 속도
    public float chaseSpeed;                   // 추적 속도
    public float detectRange;                   // 적 감지범위
    public float atackRange;                    // 공격 범위
    public float arriveDistance;                // 추적시 거리가 얼마나 남았을 떄 멈출 것인지 결정

    [Header("Scout Information")]
    Vector3[] scoutPoint;                      // 정찰 포인트. 최초 Awake시 SetPath 함수를 통해서 초기화됨.
    public int scoutIndex = 0;               // 정찰 포인트를 제어할 인덱스



    [Header("Flag")]
    public bool drawGizmo = false;      // 기즈모를 그릴지 말지 결정할 변수. 이게없으면 에러가 나서 사용하게 되었다.
    public bool checkPath = false;      //  Enemy Agent가 목표까지의 길이 존재하는지 판단하기 위하여 사용됨. checkPath는 agent.SetDestination의 리턴값임.
    public bool isAlive = true;
    public bool playerDetect = false;

    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float idleWaitTimeMax=3f;            //정찰가기 전 대기시간 3초
    float idleWaitTime;
    public float atackWaitTimeMax = 2f;
    public float atackWaitTime;
    public float atackStayTImeMax=1f;
    public float atackStayTime;
    public float getHitWaitTimeMax = 1.5f;
    public float getHitWaitTime;


    public WaitForSeconds chaseRefreshTime = new WaitForSeconds(0.5f);
    public WaitForSeconds OneSecondWait = new WaitForSeconds(1.0f);
    public WaitForSeconds DotFiveSecondWait = new WaitForSeconds(0.5f);



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
    protected EnemyState _state = EnemyState.NULL;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (value == EnemyState.IDLE)
                StateIdle(value);
            else if (value == EnemyState.SCOUT)
                StateScout(value);
            else if (value == EnemyState.CHASE)
                StateChase(value);
            else if (value == EnemyState.ATACKWAIT)
                StateAtackWait(value);
            else if (value == EnemyState.ATACK)
                StateAtack(value);
            else if (value == EnemyState.GETHIT)
                StateGetHit(value);
        }
    }
    virtual protected void StateIdle(EnemyState value)
    {
        Debug.LogWarning("Idle상태 설정완료. 대기 시작");

        anim.SetBool("Scout", false);
        agent.isStopped = true;

        idleWaitTime = Time.time;
        _state = value;
    }
    virtual protected void StateScout(EnemyState value)
    {
        Debug.LogWarning("Scout상태 진입.");

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
    virtual protected void StateChase(EnemyState value)
    {
        Debug.LogWarning("Chase상태 진입.");

        anim.SetBool("ChasePlayer", true);
        anim.SetBool("ArrivePlayer", false);

        agent.isStopped = false;
        StartCoroutine(ChasePlayerRefresh());
        _state = value;
    }
    virtual protected void StateAtack(EnemyState value)
    {
        Debug.LogWarning("Atack!!!!!! and Wait..");

        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
            anim.SetTrigger("Atack1");
        else
            anim.SetTrigger("Atack2");
        atackStayTime = Time.time;
        _state = value;
    }
    virtual protected void StateAtackWait(EnemyState value)
    {
        Debug.LogWarning("AtackWait.....");

        agent.isStopped = true;

        anim.SetBool("ChasePlayer", false);
        anim.SetBool("ArrivePlayer", true);
        atackWaitTime = Time.time;
        _state = value;
    }
    virtual protected void StateGetHit(EnemyState value)
    {
        Debug.LogWarning("Get Hit");

        agent.isStopped = true;
        heart--;
        if (heart != 0)
        {
            Debug.Log("GetHit 프로퍼티 hp 감소");
            anim.SetTrigger("GetHit");
            getHitWaitTime = Time.time;
            _state = value;

        }
        else if(isAlive==true)
        {
            Debug.Log("GetHit 프로퍼티 DIE 실행");
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.LogError("D I E");
        StopAllCoroutines();
        agent.isStopped = true;
        player = null;
        playerDetect = false;
        isAlive = false;
        anim.SetTrigger("Die");
        anim.SetBool("Scout",false);
        anim.SetBool("ArrivePlayer", false);
        anim.SetBool("ChasePlayer", false);



        IAmDied?.Invoke();      //Spawner에게 일정시간 후 다시 부활시켜달라 요청
        StartCoroutine(OneSecondAfterDisable());
        

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
        if(other.CompareTag("Weapon") && isAlive== true && playerDetect==true)               ///자식의 Triggeㄱ발동으로 강제 실행됨.
        {
            Debug.Log("아야!");
            enemyCollider.enabled = false;          //추가적으로 맞지 않게끔 비활성화. EnemyModeGetHit에서 무적시간 적용.
            State = EnemyState.GETHIT;
        }
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
        //anim.SetTrigger("Restart");
        State = EnemyState.IDLE;
        heart = maxHeart;
        scoutIndex = 0;
        transform.position = spownPoint.position;
        enemyCollider.enabled = true;
        isAlive = true;
        playerDetect = false;
    }

    void DetectPlayer(GameObject obj)
    {
        Debug.Log("플레이어 발견 델리게이트 수신 완료. 콜라이더 활성화");
        enemyCollider.enabled = true;
        playerDetect = true;
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
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = transform.GetComponent<Collider>();
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
        if (agent.remainingDistance < arriveDistance)
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
        if (Time.time - atackStayTime > atackStayTImeMax)
        {
            State = EnemyState.ATACKWAIT;
        }
    }
    protected virtual void EnemyModeGetHit()
    {
        //Debug.Log($"{Time.time - getHitWaitTime}");
        if (Time.time - getHitWaitTime > getHitWaitTimeMax)
        {
            enemyCollider.enabled = true;
            Debug.Log("무적시간 끝!");

            if (agent.remainingDistance < 2f)
            {
                anim.SetBool("ChasePlayer", false);
                anim.SetBool("ArrivePlayer", true);
                State = EnemyState.ATACKWAIT;
            }
            else
            {
                anim.SetBool("ChasePlayer", true);
                anim.SetBool("ArrivePlayer", false);
                State = EnemyState.CHASE;
            }
        }

    }


    /*
    private void OnDrawGizmos()
    {
        if(drawGizmo==true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
    */
    
}
