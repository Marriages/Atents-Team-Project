using System.Collections;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Component")]       //각종 컴포넌트정보를 담은 헤더.
    protected Animator anim;                    // 애니메이션 컨트롤러
    protected GameObject player;            // 목표(플레이어)를 추적하기 위하여 사요오딤
    protected NavMeshAgent agent;        // 길찾기 Navigation에 사용될 변수
    EnemyDetector detector;                    // 자식오브젝트 Detector로부터 델리게이트를 수신하기 위함
    Transform spownPoint;                       // 초기 
    public Collider enemyCollider;                      // 무적시간 설정을 위하여 선언.
    public Action IAmDied;                       // Spawner에게 죽었다는 것을 알리고, 새로 Enable시키기 위함.
    public Collider enemyWeapon;

    [Header("Enemy Information")]          // 해당 객체가 Enable되었을 때 셋팅할 SettingInformation()에 들어가게 될 변수들.
    public int heart;                                  // 현재 생명력
    public int maxHeart;                           // 최대 생명력. Enable 되었을 때 heart를 초기화할 목적으로 선언됨.
    public float enemySpeed;                  // 현재 속도를 제어하기 위하여 설정됨. normalSpeed 는 정찰속도 / chaseSpsed는 추적 속도로 사용됨
    public float normalSpeed;                  // 정찰 속도
    public float chaseSpeed;                   // 추적 속도
    public float detectRange;                   // 적 감지범위
    public float atackRange;                    // 공격 범위
    public float arriveDistance;                // 추적시 거리가 얼마나 남았을 떄 멈출 것인지 결정


    [Header("Scout Position Information")]
    Vector3[] scoutPoint;                      // 정찰 포인트. 최초 Awake시 SetPath 함수를 통해서 초기화됨.
    public int scoutIndex = 0;               // 정찰 포인트를 제어할 인덱스



    [Header("Flag")]
    public bool drawGizmo = false;      // 기즈모를 그릴지 말지 결정할 변수. 이게없으면 에러가 나서 사용하게 되었다.
    public bool checkPath = false;      //  Enemy Agent가 목표까지의 길이 존재하는지 판단하기 위하여 사용됨. checkPath는 agent.SetDestination의 리턴값임.
    public bool isAlive = true;                 //살아있는지 확인하기 위한 변수. 처음 Enable -> RespawnSetting 단계시 true로 설정되며, Die 상태시 false로 되서 각종 기능들이 막히게 된다.
    public bool playerDetect = false;       // 플레이어를 감지하는 자식오브젝트 Detector가 플레이어 감지시 델리게이트 방송, DetectPlayer 함수로 연결되어 true로 바꾼다. Die, RespawnSetting시 false로 설정.


    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float idleWaitTimeMax = 3f;            // Idle상태->정찰상태 로 가기전 대기시간
    public float idleWaitTime;
    public float atackWaitTimeMax = 2f;     // 공격을 하기 전까지의 대기시간
    public float atackWaitTime;
    public float atackStayTImeMax=1f;       // 공격을 하는 시간
    public float atackStayTime;
    public float getHitWaitTimeMax = 1.5f;  // 피격 후 무적시간
    public float getHitWaitTime;

    [Header("Wait Time")]
    WaitForSeconds DotFiveSecondWait = new WaitForSeconds(0.5f);            // 5초 기다리기 용 Chase단계에 사용됨
    WaitForSeconds ThreeSecondWait = new WaitForSeconds(3.0f);              // 3초 기다리기 용. 리스폰 단계에 사용됨

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value--
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------

    public enum EnemyState
    {
        IDLE,                    // 아무것도 하지 않는 상태
        SCOUT,                // 정해진 정찰 포인트를 향해 걸어가고 있는 상태
        CHASE,                // 자식 Detector로부터 Player를 감지했다는 델리게이트를 전달받은 후 플레이어를 추격하는 상태
        ATACKWAIT,         // 플레이어에게 도착 또는 공격 이후 잠시 대기하는 상태
        ATACK,                 // 대기상태가 종료 후 공격 애니메이션을 실행하고 있는 상태
        GETHIT,               // 어떠한 상태에서든 플레이어에 의해 피격당한 상태
        NULL                    // 아무것도 실행하지 않는 상태
    }
    protected EnemyState _state = EnemyState.NULL;          //초기 상태를 NULL로 함으로써 아무것도 실행되지 않게끔 설정
    public EnemyState State             //상태가 변할시 단 한번씩만 실행되게 할 상태제어프로퍼티. 매우 중요한 역할을 함.
    {
        get => _state;                  // 현재 상태가 무엇인지 알려주는 Get

        set                                     //  상태가 바뀔 경우, Value 값에 해당 상태가 들어오는데, 각 value마다 실행될 함수들을 만들어서 실행 -> 필요시 override 할수 있게 protected virtual로 작성함.
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
    virtual protected void StateIdle(EnemyState value)                  //  ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ----------
    {
       // Debug.LogWarning("Idle상태 설정완료. 대기 시작");

        enemyWeapon.enabled = false;

        anim.SetBool("Scout", false);
        agent.isStopped = true;

        idleWaitTime = Time.time;
        _state = value;
    }
    virtual protected void StateScout(EnemyState value)                  //  ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ----------
    {
        //Debug.LogWarning("Scout상태 진입.");

        anim.SetBool("Scout", true);

        enemyWeapon.enabled = false;

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
    virtual protected void StateChase(EnemyState value)                  //  ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ----------
    {
        //Debug.LogWarning("Chase상태 진입.");

        enemyWeapon.enabled = false;

        anim.SetBool("ChasePlayer", true);
        anim.SetBool("ArrivePlayer", false);

        agent.isStopped = false;
        StartCoroutine(ChasePlayerRefresh());
        _state = value;
    }
    IEnumerator ChasePlayerRefresh()
    {
        while (true)
        {
            //Debug.LogWarning("플레이어 추적 갱신");
            checkPath = agent.SetDestination(player.transform.position);        //주기적 목표 갱신
            if (checkPath == false)
                Debug.LogError("경로를 찾을 수 없습니다.");

            yield return DotFiveSecondWait;
        }
    }
    virtual protected void StateAtack(EnemyState value)                  //  ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ----------
    {
        Debug.LogWarning("Atack!!!!!! and Wait..");

        enemyWeapon.enabled = true;

        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
            anim.SetTrigger("Atack1");
        else
            anim.SetTrigger("Atack2");
        atackStayTime = Time.time;
        _state = value;
    }
    virtual protected void StateAtackWait(EnemyState value)                  //  ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- 
    {
        
        //Debug.LogWarning("AtackWait.....");

        enemyWeapon.enabled = false;

        agent.isStopped = true;

        anim.SetBool("ChasePlayer", false);
        anim.SetBool("ArrivePlayer", true);
        atackWaitTime = Time.time;
        _state = value;
    }
    virtual protected void StateGetHit(EnemyState value)                  //  ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- 
    {
        //Debug.LogWarning("Get Hit");


        enemyWeapon.enabled = false;
        enemyCollider.enabled = false;

        agent.isStopped = true;
        heart--;
        if (heart != 0)
        {
           // Debug.Log("Enemy GetHit 프로퍼티 hp 감소");
            anim.SetTrigger("GetHit");
            getHitWaitTime = Time.time;
            _state = value;

        }
        else if(isAlive==true)
        {
            //Debug.Log("GetHit 프로퍼티 DIE 실행");
            Die();
        }
    }

    protected virtual void Die()                  //  ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- 
    {
        //Debug.LogError("D I E");
        StopAllCoroutines();
        agent.isStopped = true;
        player = null;
        playerDetect = false;
        enemyCollider.enabled = false;
        isAlive = false;
        anim.SetTrigger("Die");
        anim.SetBool("Scout",false);
        anim.SetBool("ArrivePlayer", false);
        anim.SetBool("ChasePlayer", false);
        IAmDied?.Invoke();      //Spawner에게 일정시간 후 다시 부활시켜달라 요청
        StartCoroutine(OneSecondAfterDisable());
    }
    IEnumerator OneSecondAfterDisable()
    {
        yield return ThreeSecondWait;
        gameObject.SetActive(false);
    }

    //--------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기--------

    private void Awake()                  //  ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake-----
    {
        SettingInformation();
        FindComponent();
        SetupPath();
    }
    protected virtual void SettingInformation() {  }
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
            scoutPoint[i] = trans.GetChild(i + 1).position;
    }
    private void OnEnable()                  //  ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ------
    {
        RespownSetting();
    }
    protected virtual void RespownSetting()
    {
        //anim.SetTrigger("Restart");
        State = EnemyState.IDLE;
        heart = maxHeart;
        scoutIndex = 0;
        transform.position = spownPoint.position;
        enemyWeapon.enabled = false;
        enemyCollider.enabled = false;
        isAlive = true;
        playerDetect = false;
    }

    private void Start()                  //  ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start
    {
        detector.detectPlayer += DetectPlayer;      
    }

    //--------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기--------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신

    void DetectPlayer(GameObject obj)
    {
        Debug.Log("플레이어 발견");
        enemyCollider.enabled = true;
        playerDetect = true;
        player = obj;
        State = EnemyState.CHASE;
    }

    //--------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드

    private void FixedUpdate()                 //  ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ----------
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

    protected virtual void EnemyModeIdle()                  //  ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle
    {
        if (Time.time - idleWaitTime > idleWaitTimeMax)
        {
            //Debug.Log("idle 대기 종료. Scout시작");
            State = EnemyState.SCOUT;
        }

    }
    protected virtual void EnemyModeScout()                  //  ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout
    {
        if (agent.remainingDistance < 0.1f)
            State = EnemyState.IDLE;
    }
    protected virtual void EnemyModeChase()                  //  ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ----------
    {
        if (agent.remainingDistance < arriveDistance)
        {
            //Debug.Log("플레이어에게 도착.");
            StopAllCoroutines();
            State = EnemyState.ATACKWAIT;
        }

    }
    protected virtual void EnemyModeAtackWait() { }                   //  ---------- AtackWait  ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ----------
    // 각각의 Enemy Class에서 따로 구현할 예정. 할 필요가 없으니 합치는 작업 진행할 것.

    protected virtual void EnemyModeAtack()                  //  ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ----------
    {
        if (Time.time - atackStayTime > atackStayTImeMax)
            State = EnemyState.ATACKWAIT;
    }
    protected virtual void EnemyModeGetHit()                  //  ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit
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

    //--------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트--------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && isAlive == true && playerDetect == true) 
        {
            //Debug.Log("플레이어에게 공격당함");
            enemyCollider.enabled = false;          //추가적으로 맞지 않게끔 비활성화. EnemyModeGetHit에서 무적시간 적용.
            State = EnemyState.GETHIT;
        }
    }

    //--------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트--------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos----------------Gizmos

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
