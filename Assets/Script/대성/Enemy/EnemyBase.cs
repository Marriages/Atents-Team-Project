using System.Collections;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;

// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
/* 해당 위치에 기재된 내용의 경우, 시간이 남을 경우 추가 기능으로써 구현할 것.
 * 1. 플레이어 감지 상태일 경우, 몬스터의 남은 HP를 몬스터 머리위에 캔버스를 이용하여 표시할 것.
 * 2. 몬스터 피격 효과 추가하기
 * 3. 분대 시스템 추가하기 ( 플레이어 발견시 동시에 Chase 진행할 수 있도록 )  -> 아이디어로, 몬스터 군단 프리팹을 따로 생성.
 * 4. 킹 슬라임 추가하기
 */
// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★


public class EnemyBase : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value-

    [Header("Component")]                                       //각종 컴포넌트정보를 담은 헤더.
    protected Animator anim;                                    // 애니메이션 컨트롤러

    protected NavMeshAgent agent;                               // 길찾기 Navigation에 사용될 변수

    protected GameObject player;                                // 목표(플레이어)를 추적하기 위하여 사요오딤
    public GameObject hitEffect;

    private EnemyDetector detector;                                     // 자식오브젝트 Detector로부터 델리게이트를 수신하기 위함

    public Collider enemyCollider;                              // 무적시간 설정을 위하여 선언.
    public Collider enemyWeaponCollider;
    public Collider enemyDetectorCollider;

    private Spawner spawner;
    private Rigidbody rigid;

    public Action IAmDied;                                      // Spawner에게 죽었다는 것을 알리고, 새로 Enable시키기 위함.
    public Action detectorEnable;                               // 플레이어가 스포너틀 나갔을 때, 위치 초기화 후 디텍터를 활성화할 목적의 델리게이트


    //[Header("Enemy Information")]                               // 해당 객체가 Enable되었을 때 셋팅할 SettingInformation()에 들어가게 될 변수들.
    protected int heart;                                           // 현재 생명력
    protected int maxHeart;                                        // 최대 생명력. Enable 되었을 때 heart를 초기화할 목적으로 선언됨.
    protected float normalSpeed;                                   // 정찰 속도
    protected float chaseSpeed;                                    // 추적 속도
    protected float arriveDistance;                                // 추적시 거리가 얼마나 남았을 떄 멈출 것인지 결정


    [Header("Scout Position Information")]
    Vector3[] scoutPoint;                                       // 정찰 포인트. 최초 Awake시 SetPath 함수를 통해서 초기화됨.
    public int scoutIndex = 0;                                  // 정찰 포인트를 제어할 인덱스
    Transform spownPoint;                                       // 초기 



    [Header("Flag")]
    public bool drawGizmo = false;      // 기즈모를 그릴지 말지 결정할 변수. 이게없으면 에러가 나서 사용하게 되었다.
    public bool checkPath = false;      // Enemy Agent가 목표까지의 길이 존재하는지 판단하기 위하여 사용됨. checkPath는 agent.SetDestination의 리턴값임.
    public bool isAlive = true;         // 살아있는지 확인하기 위한 변수. 처음 Enable -> RespawnSetting 단계시 true로 설정되며, Die 상태시 false로 되서 각종 기능들이 막히게 된다.
    public bool playerDetect = false;   // 플레이어를 감지하는 자식오브젝트 Detector가 플레이어 감지시 델리게이트 방송, DetectPlayer 함수로 연결되어 true로 바꾼다. Die, RespawnSetting시 false로 설정.
    public bool debugOnOff = false;


    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float idleWaitTimeMax = 3f;              // Idle상태->정찰상태 로 가기전 대기시간
    public float idleWaitTime;
    public float atackWaitTimeMax = 2f;             // 공격을 하기 전까지의 대기시간
    public float atackWaitTime;
    public float atackStayTImeMax = 1f;               // 공격을 하는 시간
    public float atackStayTime;
    public float getHitWaitTimeMax = 1.5f;          // 피격 후 무적시간
    public float getHitWaitTime;
    public float disappearTimeMax = 4f;             // 사망 후 몬스터가 사라지는 시간.
    public float disappearTime;

    [Header("Wait Time")]
    readonly WaitForSeconds DotFiveSecondWait = new WaitForSeconds(0.5f);            // 5초 기다리기 용 Chase단계에 사용됨
    readonly WaitForSeconds ThreeSecondWait = new WaitForSeconds(3.0f);              // 3초 기다리기 용. 리스폰 단계에 사용됨


    [Header("Drop Item")]
    public GameObject enemyDropHeart;
    public float enemyDropHeartRate=0.5f;
    public GameObject enemyDropCoin;
    public float enemyDropCoinRate = 0.5f;

    

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value-
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------

    public enum EnemyState
    {
        IDLE,                           // 아무것도 하지 않는 상태
        SCOUT,                          // 정해진 정찰 포인트를 향해 걸어가고 있는 상태
        CHASE,                          // 자식 Detector로부터 Player를 감지했다는 델리게이트를 전달받은 후 플레이어를 추격하는 상태
        ATACKWAIT,                      // 플레이어에게 도착 또는 공격 이후 잠시 대기하는 상태
        ATACK,                          // 대기상태가 종료 후 공격 애니메이션을 실행하고 있는 상태
        GETHIT,                         // 어떠한 상태에서든 플레이어에 의해 피격당한 상태
        DIE,
        RETURN,
        NULL                            // 아무것도 실행하지 않는 상태
    }
    protected EnemyState _state = EnemyState.NULL;          //초기 상태를 NULL로 함으로써 아무것도 실행되지 않게끔 설정
    public EnemyState State                                 //상태가 변할시 단 한번씩만 실행되게 할 상태제어프로퍼티. 매우 중요한 역할을 함.
    {
        get => _state;                                      // 현재 상태가 무엇인지 알려주는 Get

        set                                                 //  상태가 바뀔 경우, Value 값에 해당 상태가 들어오는데, 각 value마다 실행될 함수들을 만들어서 실행 
        {                                                   //            -> 필요시 override 할수 있게 protected virtual로 작성함.
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
            else if (value == EnemyState.RETURN)
                StateReturn(value);
            else if (value == EnemyState.DIE)
                StateDie(value);
            
        }
    }
    virtual protected void StateIdle(EnemyState value)                  //  ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ---------- Idle 상태 Set ----------
    {
        if (debugOnOff)
            Debug.LogWarning("Idle상태 설정완료. 대기 시작");

        enemyWeaponCollider.enabled = false;            // 대기상태일때는 무기를 비활성화시켜서 원치않은 공격을 막음.

        anim.SetBool("Return", false);
        anim.SetBool("Scout", false);           // 가만히 서있는 애니메이션 재생
        anim.SetBool("ChasePlayer", false);           // 가만히 서있는 애니메이션 재생
        agent.isStopped = true;                 // agent를 움직이지 못하도록 정지시킴

        idleWaitTime = Time.time;                                       // 대기시간 초기화 -> 일정한 시간만큼 대기하게 됨
        _state = value;                                                 // 상태적용 -> FixedUpdate에서 EnemyModeIdle 실행
    }
    virtual protected void StateScout(EnemyState value)                  //  ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ---------- Scout 상태 Set ----------
    {
        if (debugOnOff)
            Debug.LogWarning("Scout상태 진입.");

        anim.SetBool("Scout", true);                                    // 걸어가는 애니메이션 재생

        agent.speed = normalSpeed;                                      // agent 속도 조절.

        enemyWeaponCollider.enabled = false;                                    // 혹여나 무기가 활성화 될 수 있으니, 비활성화시킴

        //Debug.Log($"agent Dest : {scoutPoint[scoutIndex]}");          // 목적지 확인용 디버그로그

        checkPath = agent.SetDestination(scoutPoint[scoutIndex]);       // 목적지 설정. SetDestination은 목적지가 있으면 해당 목적지를, 없으면 Null을 리턴함.
        if (checkPath == false)                                         // 만약 null이 리턴되었다면, 갈 수 있는 경로가 없기에 에러로그 출력
            Debug.LogError($"{scoutIndex}번 경로를 찾을 수 없습니다.");

        agent.isStopped = false;                                        // agent가 움직일 수 있도록 설정.

        scoutIndex++;                                                   // 정찰포인트 증가시킴
        if (scoutIndex == scoutPoint.Length)                            // 가지고 있는 정찰포인트보다 많게끔 설정된 경우
        {
            scoutIndex %= scoutPoint.Length;                            // 해당 개수만큼 나머지 연산으로 초기화
            //Debug.Log("scoutIndex가 초기화되었습니다.");
        }

        _state = value;                                                 // 상태적용 -> FixedUpdate에서 EnemyModeScout 실행
    }
    virtual protected void StateChase(EnemyState value)                 //  ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ---------- Chase 상태 Set ----------
    {
        if (debugOnOff)
            Debug.LogWarning("Chase상태 진입.");

        enemyWeaponCollider.enabled = false;                                    // 혹여나 무기가 활성화 될 수 있으니, 비활성화시킴

        agent.speed = chaseSpeed;                                       // agent 속도 조절

        anim.SetBool("ChasePlayer", true);                              // 플레이어 추적하는 애니메이션 실행을 위해 ChasePlayer 활성화
        anim.SetBool("ArrivePlayer", false);                            // 플레이어에게 일정거리만큼 접근하게 되면 ArrivePlayer를 true로 변환예정.

        agent.isStopped = false;                                        // agent가 움직일수 있게 false로 설정
        StartCoroutine(ChasePlayerRefresh());                           // 일정시간마다(0.5초) 플레이어 위치를 갱신하기 위하여 코루틴 실행 -> EnemyModeChase에서 도달시 StopCoroutine 실행
        _state = value;                                                 // 상태적용 -> FixedUpdate에서 EnemyModeChase 실행
    }

    IEnumerator ChasePlayerRefresh()                                    // StateChase에서만 실행되는 플레이어 경로 갱신용 코루틴. EnemyModeChase에서 비활성화
    {
        while (true)
        {
            if (debugOnOff)
                Debug.LogWarning("플레이어 추적 갱신");
            checkPath = agent.SetDestination(player.transform.position);        // 목적지 설정(플레이어의 위치). SetDestination은 목적지가 있으면 해당 목적지를, 없으면 Null을 리턴함.
            if (checkPath == false)                                             // 만약 null이 리턴되었다면, 갈 수 있는 경로가 없기에 에러로그 출력
                Debug.LogError("경로를 찾을 수 없습니다.");

            yield return DotFiveSecondWait;                                     // 0.5초 대기. 계속 실행할 waitForSecond라 value파트에서 미리 선언 및 할당하였음.
        }
    }

    // Override를 통해 몬스터마다 다른 공격 패턴을 부여할 것.
    virtual protected void StateAtack(EnemyState value)                  //  ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ---------- Atack 상태 Set ----------
    {
        if (debugOnOff)
            Debug.LogWarning("Atack!!!!!! and Wait..");
        enemyWeaponCollider.enabled = true;
        atackStayTime = Time.time;                                              // 공격시 다른 행동을 할 수 없게끔 타이머 설정.
        _state = value;                                                         // 상태적용 -> FixedUpdate에서 EnemyModeAtack 실행
    }
    virtual protected void StateAtackWait(EnemyState value)                  //  ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- AtackWait 상태 Set ---------- 
    {
        // ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
        //플레이어 추적 후 Chase -> AtackWait로 올 경우, 최초 1회 공격은 바로 할 수 있도록 설정할 것.

        if (debugOnOff)
            Debug.LogWarning("AtackWait.....");

        enemyWeaponCollider.enabled = false;                                            // 공격이 끝난 상황이므로 다음 공격 시작전까지 무기 콜라이더 해제

        agent.isStopped = true;                                                 // 적이 움직이지 않도록 설정.

        anim.SetBool("ChasePlayer", false);                                     // Chase 애니메이션을 실행하기 위해 제어변수 조절 ( 추적중지 )
        anim.SetBool("ArrivePlayer", true);                                     // Chase 애니메이션을 실행하기 위해 제어변수 조절 ( 플레이어에 도착 )
        atackWaitTime = Time.time;                                              // 공격대기를 위하여 시간 초기화
        _state = value;                                                         // 상태적용 -> FixedUpdate에서 EnemyModeChase 실행
    }
    virtual protected void StateGetHit(EnemyState value)                  //  ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- GetHit 상태 Set ---------- 
    {

        // ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
        // 맞을때, 이펙트가 발생해서 맞았는지 알 수 있도록 할 것.
        // Die 상태를 추가해서, 죽었을 떄 아래로 천천히 사라지게 해서 자연스럽게 Disable시키는 연출 할 것.

        if (debugOnOff)
            Debug.LogWarning("Get Hit");
        enemyWeaponCollider.enabled = false;                                            // 맞고 있는 중 플레이어에게 공격이 가해지면 안되기에, 무기 콜라이더 해제
        enemyCollider.enabled = false;                                          // 맞고 있는 중 또 맞지 않게 하기 위하여 적 콜라이더 해제. FixedUpdate의 EnemyModeGetHit에서 피격무적 적용

        agent.isStopped = true;                                                 // 맞고있을 떄 움직이면 안되므로(경직상태) 이동을 멈춤
        heart--;                                                                // hp감소
        if (heart != 0)                                                         // hp가 0이 아니면 살아있는 상태이므로 GetHit애니메이션 연출 후 피격 무적시간 셋팅
        {
            if (debugOnOff)
                Debug.Log("Enemy GetHit 프로퍼티 hp 감소");
            anim.SetTrigger("GetHit");
            getHitWaitTime = Time.time;
            _state = value;

        }
        else if (isAlive == true)                                                  // hp가 0이고 살아있는 상태로 체크되어있으면 Die함수를 실행.
        {
            if (debugOnOff)
                Debug.Log("Enemy GetHit 프로퍼티 DIE 실행");
            State = EnemyState.DIE;     //DIE프로퍼티 실행을 위하여 다르게 갑을 넣어줌.
        }
    }

    protected virtual void StateReturn(EnemyState value)
    {
        //Debug.LogWarning("Return Property 실행");
        StopAllCoroutines();
        bool k = agent.SetDestination(spownPoint.position);
        if (k == false)
            Debug.LogError("StateReturn 경로 에러");
        enemyCollider.enabled = false;
        heart = maxHeart;
        player = null;
        playerDetect = false;
        agent.isStopped = false;
        enemyWeaponCollider.enabled = false;                                    // 혹여나 무기가 활성화 될 수 있으니, 비활성화시킴
        agent.speed = chaseSpeed;                                       // agent 속도 조절
        anim.SetTrigger("Return");
        anim.SetBool("ChasePlayer", false);                              // 플레이어 추적하는 애니메이션 실행을 위해 ChasePlayer 활성화
        anim.SetBool("ArrivePlayer", false);                            // 플레이어에게 일정거리만큼 접근하게 되면 ArrivePlayer를 true로 변환예정.
        _state = value;

    }

    protected virtual void StateDie(EnemyState value)                   //  ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- Die 상태 Set ---------- 
    {
        if (debugOnOff)
            Debug.Log("Enemy Die 프로퍼티 실행");
        StopAllCoroutines();                                                    // 모든 코루틴 해제( ChasePlayerRefresh )
        agent.isStopped = true;                                                 // 죽으면 움직이면 안되기에 정지시킴
        agent.enabled = false;
        player = null;                                                          // 더이상 플레이어를 추적할 수 없도록 타겟이되는 player를 비워줌
        playerDetect = false;                                                   // 공격을 하기위해서는 playerDetect 상태가 true여하므로, 공격이 실행되지 않도록 false로 설정

        // 바닥으로 꺼지게끔 하기 위하여 콜라이더 전부 비활성화.
        enemyCollider.enabled = false;                                          // 죽었는데 피격이 다시 일어나면 안되기에 콜라이더 해제
        enemyDetectorCollider.enabled = false;
        enemyWeaponCollider.enabled = false;
        
        isAlive = false;                                                        // die함수를 다시한번 실행하지 않도록 isAlive false로 설정
        anim.SetTrigger("Die");
        anim.SetBool("Scout", false);
        anim.SetBool("ArrivePlayer", false);
        anim.SetBool("ChasePlayer", false);

        
        if (debugOnOff)
            Debug.Log("Enemy disappearTime 설정 완료.");

        disappearTime = Time.time;

        if (UnityEngine.Random.Range(0f, 1f) > enemyDropCoinRate)
        {
            if(debugOnOff)
                Debug.Log($"{gameObject.name}가 Coin을 드랍했다.");
            GameObject obj = Instantiate(enemyDropCoin);
            obj.transform.position = transform.position;
            Destroy(obj, 10f);
        }
        else if (UnityEngine.Random.Range(0f, 1f) > enemyDropHeartRate)
        {
            if (debugOnOff)
                Debug.Log($"{gameObject.name}가 Heart을 드랍했다.");
            GameObject obj = Instantiate(enemyDropHeart);
            obj.transform.position = transform.position;
            Destroy(obj, 10f);
        }



        _state = value;
    }
    // ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
    // 사라지는 모션을 적용하기 위해, 몬스터의 사망 및 부활 모션 적절하게 조절할 것

    //--------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------상태 프로퍼티----------------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기--------

    private void Awake()                  //  ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake ---------- Awake-----
    {
        SettingInformation();                                               // 적들의 기본정보를 등록. 각각의 스크립트에서 값을 설정함.(EnemyWizard,EnemySward,EnemyBomb)
        FindComponent();                                                    // 컴포넌트들을 찾아주는 함수
        SetupPath();                                                        // Scout 단계에 진행할 정찰 경로를 찾아주는 함수.
    }
    protected virtual void SettingInformation() { }
    void FindComponent()
    {
        detector = transform.GetComponentInChildren<EnemyDetector>();       // 자식 개체인 디텍터를 찾아줌
        anim = GetComponent<Animator>();                                    // 애니메이터 컨트롤러 연걸
        agent = GetComponent<NavMeshAgent>();                               // 길찾기 알고리즘을 위한 네브매시에이전트
        enemyCollider = transform.GetComponent<Collider>();                 // 본인의 히트판정을 결정할 콜라이더를 키고 끄기 위함. ( CapsuleCollider 기준. 다른 콜라이더는 잘 작동 안함. )
        enemyDetectorCollider = detector.GetComponent<Collider>();
        spawner = transform.parent.GetComponent<Spawner>();
        rigid = GetComponent<Rigidbody>();

    }
    void SetupPath()
    {
        Transform transParent = transform.parent;                           // 부모 Transform 찾음. 자식인 spownPoint,scoutPoint를 얻기 위함                                           
        spownPoint = transParent.GetChild(0).transform;                     // spownPoint 구함
        Transform spownParent = transParent.GetChild(1);                    // scoutPoint 부모 구함
        scoutPoint = new Vector3[spownParent.childCount];                   // scoutPoint 안에 있는 자식들의 위치들을 구해 아래의 for문에서 위치를 저장시킴.
        for (int i = 0; i < scoutPoint.Length; i++)
            scoutPoint[i] = spownParent.GetChild(i).position;
    }
    private void OnEnable()                  //  ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ---------- OnEnable ------
    {
        RespownSetting();                   // 몬스터의 부활,초기 생성시 기본 셋팅 진행.
    }
    protected virtual void RespownSetting()
    {
        anim.SetTrigger("Restart");
        agent.enabled = true;
        State = EnemyState.IDLE;                                        // 대기상태로 전환
        heart = maxHeart;                                               // HP 초기화
        scoutIndex = 0;                                                 // 정찰포인트 초기화
        transform.position = spownPoint.position;                       // 리스폰 위치 초기화
        enemyWeaponCollider.enabled = false;                                    // 무기 콜리더 끄기
        enemyCollider.enabled = false;                                  // 적 콜리더 끄기 ( 플레이어 감지한 후 활성화 )
        enemyDetectorCollider.enabled = true;
        isAlive = true;                                                 // 살아있다고 표시
        playerDetect = false;
        
        
        if(transform.GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
    }

    private void Start()                  //  ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start ---------- Start
    {
        detector.detectPlayer += DetectPlayer;                          // 자식개체 Detector로부터 수신하게 될 델리게이트 연결.        
        spawner.playerOut += PlayerOut;
        
    }

    //--------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기----------------생명주기--------
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신

    void DetectPlayer(GameObject obj)
    {
        if (debugOnOff)
            Debug.Log("플레이어 발견");
        enemyCollider.enabled = true;                                   // 전투의 가능성이 있으므로 적이 맞을 수 있게 콜라이더 활성화
        playerDetect = true;                                            // 플레이어 감지상태로 전환 -> 공격을 맞을떄 playerDetect상태도 true야 하므로 상태 전환.
        player = obj;                                                   // 플레이어정보를 자식개체 Detector 델리게이트로부터 수신. 위치 추적에 사용됨.
        State = EnemyState.CHASE;                                       // Chase 상태로 전환
    }
    void PlayerOut()
    {
        if(isAlive==true && playerDetect==true)
            State = EnemyState.RETURN;
    }

    //--------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신----------------델리게이트수신
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드

    private void FixedUpdate()                 //  ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ---------- FixedUpdate ----------
    {                                                                   //State Property로부터 상태가 변경될 경우, 해당 Mode가 실행되는 구조.
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
        else if(State==EnemyState.RETURN)
            EnemyModeReturn();
        else if (State == EnemyState.DIE)
            EnemyModeDie();
    }

    protected virtual void EnemyModeIdle()                  //  ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle ---------- Idle
    {
        if (Time.time - idleWaitTime > idleWaitTimeMax)                 // idleWaitTime 대기시간 초과시, scout상태로 변경하는 프로퍼티 실행.
        {
            if (debugOnOff)
                Debug.Log("idle 대기 종료. Scout시작");
            State = EnemyState.SCOUT;
        }

    }
    protected virtual void EnemyModeScout()                  //  ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout ---------- Scout
    {
        if (agent.remainingDistance < 0.1f)                             // agent가 남은 거리가 0.1보다 작을 경우, idle 상태로 변경
            State = EnemyState.IDLE;
    }
    protected virtual void EnemyModeChase()                  //  ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ---------- Chase ----------
    {
        if (agent.remainingDistance < arriveDistance)                   // 몬스터의 남은 거리가 미리 설정해둔, 도착 거리보다 작아질 경우, AtackWait 상태로 변경.     
        {
            if (debugOnOff)
                Debug.Log("플레이어에게 도착.");
            StopAllCoroutines();                                        // 모든 코루틴 해제( ChasePlayerRefresh , OneSecondAfterDisable )
            State = EnemyState.ATACKWAIT;
        }

    }
    protected virtual void EnemyModeAtackWait()             //  ---------- AtackWait  ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ---------- AtackWait ----------
    {
        if (player != null)
        {
            transform.LookAt(player.transform);
            //너무 멀어졌으면 다시 추적
            if ((player.transform.position - transform.position).sqrMagnitude > arriveDistance * arriveDistance)
            {
                if (debugOnOff)
                    Debug.LogWarning("거리가 너무 멀어짐. 추적 다시 시작");
                State = EnemyState.CHASE;
            }
            else if (Time.time - atackWaitTime > atackWaitTimeMax)
            {
                if (debugOnOff)
                    Debug.LogWarning("대기시간 종료. AtackWait로 이동");
                State = EnemyState.ATACK;
            }
        }
    }

    protected virtual void EnemyModeAtack()                  //  ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ---------- Atack ----------
    {
        if (Time.time - atackStayTime > atackStayTImeMax)               // 공격하는 시간(약 1초)동안 아무것도 실행하지 않고, 공격 시간이 지난 후 AtackWait상태로 변경
            State = EnemyState.ATACKWAIT;
    }
    protected virtual void EnemyModeGetHit()                  //  ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit ---------- GetHit
    {
        if (debugOnOff)
            Debug.Log($"{Time.time - getHitWaitTime}");
        if (Time.time - getHitWaitTime > getHitWaitTimeMax)             // 피격 무적시간 getHitWaitTime.
        {
            enemyCollider.enabled = true;                               // 피격되는 순간 enemyCollider가 false가 되는데, 피격 무적시간이 지난 이후 enabled로 변경
            if (debugOnOff)
                Debug.Log("무적시간 끝!");

            if (agent.remainingDistance < 2f)                           // if 피격 이후 플레이어와의 거리가 가까우면 AtackWait, else 그렇지 않으면 Chase 모드로 변경
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

    protected virtual void EnemyModeReturn()
    {
        if (agent.remainingDistance < 0.1f)                             // agent가 남은 거리가 0.1보다 작을 경우, idle 상태로 변경
        {
            detectorEnable?.Invoke();
            State = EnemyState.IDLE;
        }
    }

    protected virtual void EnemyModeDie()                   //  ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die ---------- Die
    {
        if (Time.time - disappearTime > 2f)
        {
            if (Time.time - disappearTime < disappearTimeMax)
            {
                //transform.Translate(Vector3.down * 2f * Time.fixedDeltaTime);
                transform.position = transform.position + Vector3.down * Time.fixedDeltaTime;
                //Debug.Log($"{transform.position} / {transform.position + Vector3.down * Time.fixedDeltaTime}");
            }
            else
            {
                transform.gameObject.SetActive(false);
                IAmDied?.Invoke();
            }
        }
    }

    //--------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드----------------FixedUpdate & 몬스터 모드
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //--------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트----------------충돌 이벤트--------


    private void OnCollisionEnter(Collision collision)
    {/* 히트 이펙트를 넣고 싶었는데, 뭔가 이상함.
        Debug.Log("Collision Enter");
        if (collision.gameObject.CompareTag("Weapon") && isAlive == true && playerDetect == true)      //피격당하기 위해서는, weapon이며, 살아있고, 플레이어를 감지한 상태여야만 함.
        {
            if (debugOnOff)
                Debug.Log("플레이어에게 공격당함");
            GameObject obj = Instantiate(hitEffect);
            obj.transform.position = collision.contacts[0].point;
            Destroy(obj.gameObject, 1f);
            enemyCollider.enabled = false;                      // 추가적으로 맞지 않게끔 적 콜라이더 비활성화. 
            State = EnemyState.GETHIT;                          // 이후 무적시간 적용을 위해 GetHit 상태로 변경
        }*/
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && isAlive == true && playerDetect == true)      //피격당하기 위해서는, weapon이며, 살아있고, 플레이어를 감지한 상태여야만 함.
        {
            if (debugOnOff)
                Debug.Log("플레이어에게 공격당함");
            GameObject obj = Instantiate(hitEffect);
            obj.transform.position = transform.position + Vector3.forward;
            //obj.transform.position = other.contacts[0].point;
            Destroy(obj.gameObject, 1f);
            enemyCollider.enabled = false;                      // 추가적으로 맞지 않게끔 적 콜라이더 비활성화. 
            State = EnemyState.GETHIT;

        }
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


