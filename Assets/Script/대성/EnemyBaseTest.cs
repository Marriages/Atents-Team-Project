using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
/*
 * 문제점
 * 1. 피격이 뭔가 이상함. 한번에 여러번 공격을 당하게 됨.
 * 
 * 
*/
public class EnemyBaseTest : MonoBehaviour
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

    [Header("Enemy Information")]
    public int heart;
    public int maxHeart = 3;
    public float enemySpeed = 5f;
    public float normalSpeed = 5f;
    public float chaseSpeed = 8f;
    public float detectRange=5f;
    public float atackRange=8f;


    
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
                anim.SetTrigger("Idle");
                agent.isStopped = true;
                //agent.Stop();
                StartCoroutine(WaitScout());
                
            }
            else if (value == EnemyState.SCOUT)
            {
                Debug.Log($"SCOUT Property, Destiny : {scoutPoint[scoutIndex]}");
                anim.SetBool("Chase", false);
                anim.SetTrigger("Scout");

                targetDirection = scoutPoint[scoutIndex];
                scoutIndex++;
                if(scoutIndex==scoutPoint.Length)
                    scoutIndex %= scoutPoint.Length;

                agent.speed = enemySpeed;
                agent.SetDestination(targetDirection);
                agent.isStopped = false;
                //agent.Resume();
            }
            else if (value == EnemyState.CHASE)
            {
                agent.destination = player.transform.position;
                Debug.Log("CHASE Property");
                anim.SetBool("Chase",true);
                agent.speed = chaseSpeed;
                scoutIndex = 0;
                agent.isStopped = false;
                //agent.Resume();
            }
            else if (value == EnemyState.ATACK)
            {
                
                Debug.Log("ATACK Property");
                anim.SetBool("Chase", false);       //해제함과 동시에 1회 공격할예정.
                //agent.speed = 0f;
                agent.isStopped = true;

                intervalAtackCurrent = Time.time;       //최초 1회 공격 이후 타이머마다 공격

            }

            _state = value;
        }
    }

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void Awake()
    {
        detector = transform.GetComponentInChildren<EnemyDetectAtack>();
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


        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Scout");
        anim.ResetTrigger("Atack");
        anim.ResetTrigger("AtackWait");
        anim.ResetTrigger("Ouch");
        Debug.Log("초기화 완료");

        State = EnemyState.IDLE;
        detectRangeCollider.radius = detectRange;
        detector.enemyDamaged += OnDamaged;

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
            State = EnemyState.CHASE;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerDetect = false;
            //Debug.LogWarning("감지범위 나감");
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