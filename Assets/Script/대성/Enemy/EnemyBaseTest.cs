using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class EnemyBaseTest : MonoBehaviour
{ }
    /*
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //컴포넌트 이름은 컴포넌트 명의 축소형
    [Header("Component")]
    protected Animator anim;
    protected GameObject player;
    protected NavMeshAgent agent;
    protected SphereCollider detectRangeCollider;
    Spawner spawner;
    EnemyDetector detector;

    [Header("Enemy Information")]
    public int heart;
    public int maxHeart = 3;
    public float enemySpeed = 5f;
    public float normalSpeed = 5f;
    public float chaseSpeed = 8f;
    public float detectRange=5f;
    public float atackRange=8f;
    Transform spownPoint;

    [Header("Scout Information")]
    Vector3[] scoutPoint;
    public int scoutIndex=0;
    Vector3 targetDirection= Vector3.zero;

    //Flag변수는 항상 is로 시작
    [Header("Flag")]
    public bool isdetectPlayer = false;
    public bool isAtacking = false;
    public bool isWaitScout = false;
    public bool isAtackWaiting = false;
    public bool playerDetect = false;

    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float intervalScout = 3f;
    
    public float intervalAtack = 3f;            //공격 쿨타임.
    public float intervalAtackCurrent = 3f;     //점점 줄어들어 0보다 작아지면 공격을 실행. 이후 intervalAtack 값으로 복귀
    public float intervalAtackWait = 0.7f;        //공격 시 움직이지 않을 타이머
    public float intervalAtackWaitCurrent = 0.7f; //점점 줄어들어 0보다 작아지면 플레이어 추적여부를 결정.

    IEnumerator waitAtack;


    [Header("Test")]
    InputSystemController inputController;

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value-------------

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------
    
    public int Heart
    {
        get => heart;
        set
        {
            // 피격 애니메이션 실행할 것
            Debug.Log("Ouch");
            anim.SetTrigger("Ouch");
            heart = value;
            if(heart==0)
            {
                Die();
            }
        }
    }
    
    public enum EnemyState
    {
        IDLE,
        SCOUT,
        CHASE,
        ATACK
    }
    EnemyState _state;
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
                Debug.Log("IDLE Property");
                anim.SetBool("Scout", false);
                agent.isStopped = true;
                //agent.Stop();
                StartCoroutine(WaitScout());


                _state = value;

            }
            else if (value == EnemyState.SCOUT)
            {
                Debug.Log($"SCOUT Property, Destiny : {scoutPoint[scoutIndex]}");
                anim.SetBool("Scout", true);

                targetDirection = scoutPoint[scoutIndex];
                scoutIndex++;
                if(scoutIndex==scoutPoint.Length)
                    scoutIndex %= scoutPoint.Length;

                agent.speed = enemySpeed;
                agent.SetDestination(targetDirection);
                agent.isStopped = false;
                //agent.Resume();


                _state = value;
            }
            else if (value == EnemyState.CHASE)
            {
                agent.destination = player.transform.position;
                Debug.Log("CHASE Property");
                anim.SetBool("Scout", false);
                Debug.Log("Scout Bool False");
                anim.SetBool("Chase",true);
                agent.speed = chaseSpeed;
                scoutIndex = 0;
                agent.isStopped = false;
                //agent.Resume();


                _state = value;
            }
            else if (value == EnemyState.ATACK)
            {
                
                Debug.Log("ATACK Property");
                anim.SetBool("Chase", false);       //해제함과 동시에 1회 공격할예정.
                //agent.speed = 0f;
                agent.isStopped = true;

                intervalAtackCurrent = Time.time;       //최초 1회 공격 이후 타이머마다 공격


                _state = value;

            }
        }
    }

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    void Awake()
    {
        SettingInformation();
        detector = transform.GetComponentInChildren<EnemyDetector>();
        spawner = transform.parent.GetComponent<Spawner>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        detectRangeCollider = GetComponent<SphereCollider>();


        Transform trans = transform.parent;
        scoutPoint = new Vector3[trans.childCount-2];
        for (int i = 0; i < scoutPoint.Length; i++)
        {
            scoutPoint[i] = trans.GetChild(i + 1).position;
            Debug.Log($"경로설정 <{i}> 완료. {scoutPoint[i]}");
        }
            

        //TEST//
        inputController = new InputSystemController();
        //TEST//
    }

    virtual protected void SettingInformation()
    {
        heart=3;
        maxHeart = 3;
        enemySpeed = 5f;
        normalSpeed = 5f;
        chaseSpeed = 8f;
        detectRange = 5f;
        atackRange = 8f;
    }

    private void FixedUpdate()
    {
        if(State==EnemyState.IDLE)
            EnemyModeIdle();
        else if(State==EnemyState.SCOUT)
            EnemyModeScout();
        else if(State==EnemyState.CHASE)
            EnemyModeChase();
        else if(State==EnemyState.ATACK)
            EnemyModeAtack();
    }

    private void OnEnable()
    {
        
        TestSettingOn();
        heart = maxHeart;
        scoutIndex = 0;


        Debug.Log("초기화 완료");

        State = EnemyState.IDLE;
        detectRangeCollider.radius = detectRange;
       // detector.enemyDamaged += OnDamaged;

    }

    private void OnDisable()
    {
        TestSettingOff();
    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    protected virtual void EnemyModeIdle()
    {
        if (isWaitScout == false)
        {
            Debug.Log("정찰 시작");
            State = EnemyState.SCOUT;
            
        }
    }
    protected virtual void EnemyModeScout()
    {
        if(agent.remainingDistance < 0.2f)
        {
            Debug.Log("목표 도착");
            State = EnemyState.IDLE;
        }
    }
    protected virtual void EnemyModeChase()
    {
        if (agent.remainingDistance < 3f)
        {
            State = EnemyState.ATACK;
        }
        
    }
    protected virtual void EnemyModeAtack()
    {
        
    }

    protected virtual void Die()
    {
        spawner.IsAlive = false;
        this.gameObject.SetActive(false);
    }

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------
    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    IEnumerator WaitScout()
    {
        Debug.Log("대기 시작");
        isWaitScout = true;
        yield return new WaitForSeconds(intervalScout);
        Debug.Log("대기 끝");
        isWaitScout = false;
    }


    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && playerDetect==false)
        {
            playerDetect = true;
            Debug.LogWarning("감지범위 들어옴");
            //Debug.Log($"★ '{transform.gameObject.name}' 가 '{other}' 를 발견");
            detectRangeCollider.radius = atackRange;
            player = other.gameObject;
            //anim.SetBool("Scout", false);
            State = EnemyState.CHASE;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerDetect = false;
            Debug.LogWarning("감지범위 나감");
            //Debug.Log($"☆ '{transform.gameObject.name}' 가 '{other}' 를 떠남");
            detectRangeCollider.radius = detectRange;
            player = null;
            agent.speed = normalSpeed;
            heart = maxHeart;
            State = EnemyState.SCOUT;
        }
    }

    void OnDamaged()
    {
        Debug.LogError("Damaged!");
        //피격했기에, 공격대기시간 초기화
        intervalAtackCurrent = Time.time;
        Heart--;
    }
    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    //--------TEST----------------TEST----------------TEST----------------TEST----------------TSET----------------TSET----------------TSET----------------TSET----------------TSET--------
    void TestSettingOn()
    {
        inputController.TestKeyboard.Enable();
        inputController.TestKeyboard.Test1.performed += OnTest1;
        inputController.TestKeyboard.Test2.performed += OnTest2;
        inputController.TestKeyboard.Test3.performed += OnTest3;
        inputController.TestKeyboard.Test4.performed += OnTest4;
        inputController.TestKeyboard.Test5.performed += OnTest5;
    }
    void TestSettingOff()
    {
        inputController.TestKeyboard.Test5.performed -= OnTest5;
        inputController.TestKeyboard.Test4.performed -= OnTest4;
        inputController.TestKeyboard.Test3.performed -= OnTest3;
        inputController.TestKeyboard.Test2.performed -= OnTest2;
        inputController.TestKeyboard.Test1.performed -= OnTest1;
        inputController.TestKeyboard.Disable();
    }
    private void OnTest1(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Idle");
    }
    private void OnTest2(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Scout");
    }
    private void OnTest3(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Chase");
    }
    private void OnTest4(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Atack");
    }
    private void OnTest5(InputAction.CallbackContext obj)
    {
        
    }
    //--------TEST----------------TEST----------------TEST----------------TEST----------------TSET----------------TSET----------------TSET----------------TSET----------------TSET--------

    private void OnDrawGizmos()
    {
        //적의 감지범위를 파랑 구로 표현.
        Gizmos.color = Color.blue;
        if(detectRangeCollider!=null)
            Gizmos.DrawWireSphere(transform.position, detectRangeCollider.radius);     //감지범위

        //적의 현재 위치-목적지 까지를 빨간 선으로 표현
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetDirection);
    }
}




/* 과거의 유산
    
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







 


    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    [Header("Action")]
    public Action<bool> EnemyDetectPlayer;

    [Header("Component")]
    public Rigidbody rigid;

    [Header("Enemy Information")]
    public float moveSpeed=5;             // 적 이동속도
    public float 
    DropRate;          // 적 사망시 코인 드랍 확률
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