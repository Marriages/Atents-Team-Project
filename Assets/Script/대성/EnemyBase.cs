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
    EnemyDetectAtack detector;
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

    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float idleWaitTimeMax=3f;            //정찰가기 전 대기시간 3초
    float idleWaitTime;                                 //정찰대기시간을 저장
    


    IEnumerator waitAtack;

    [Header("Test")]
    InputSystemController inputController;

    public enum EnemyState
    {
        IDLE,
        SCOUT,
        CHASE,
        ATACKWAIT,
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
                Debug.LogWarning("Idle상태 검사.");
                if (State!=EnemyState.IDLE)      //테스트가 모두 끝나고 if를 하나로 합칠 것: if(value==EnemyState.Idle && State!=EnemyState.IDLE)
                {
                    Debug.LogWarning("Idle상태 진입.");

                    anim.SetBool("Scout", false);
                    anim.SetBool("Idle", true);
                    agent.isStopped = true;     //멈춰!
                    idleWaitTime = Time.time;
                
                    _state = value;

                }
            }
            else if (value == EnemyState.SCOUT)
            {
                Debug.LogWarning("Scout상태 진입.");
                if (State!=EnemyState.SCOUT)     //중복 실행 방지를 위하여 조건을 걸어둠.
                {
                    Debug.LogWarning("Scout상태 진입.");
                    anim.SetBool("Idle", false);
                    anim.SetBool("Scout", true);
                    agent.SetDestination( scoutPoint[scoutIndex]);

                    scoutIndex++;
                    if (scoutIndex == scoutPoint.Length)
                    {
                        scoutIndex %= scoutPoint.Length;
                        Debug.Log("scoutIndex가 초기화되었습니다.");
                    }

                    agent.isStopped = false;        //가라!

                    _state = value;
                }
            }
            else if (value == EnemyState.CHASE)
            {
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
    protected virtual void Die()
    {

    }

    private void OnEnable()
    {
        RespownSetting();
    }
    protected virtual void RespownSetting()
    {
        heart = maxHeart;
        scoutIndex = 0;
        transform.position = spownPoint.position; ;
        State = EnemyState.IDLE;
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
        detector = transform.GetComponentInChildren<EnemyDetectAtack>();
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
            Debug.Log("idle 대기 종료");
            State = EnemyState.SCOUT;
        }

    }
    protected virtual void EnemyModeScout()
    {
        if( agent.velocity.sqrMagnitude < 0.1f)
        {
            State = EnemyState.IDLE;
        }

    }
    protected virtual void EnemyModeChase()
    {

    }
    protected virtual void EnemyModeAtackWait()
    {

    }
    protected virtual void EnemyModeAtack()
    {

    }







    //비선공 몬스터가 존재할수 있으므로, Virtual로 선언함.
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            State = EnemyState.CHASE;
        }

        
    }
}
