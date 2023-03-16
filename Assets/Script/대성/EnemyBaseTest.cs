using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
/*
 * 문제점
 * 1. 애니메이션 컨트롤러가 내마음대로 움직이지 않는다.
 * 트리거가 움직이는 속도가 너무나도 빨라서 그런것일까? 이부분은 물어보아야 할 것 같다.
 * Atack쪽에서 발생하는 문제.
 * 
 * 
 * 추가적으로 구현해야 할 것
 * 감지범위가 5이며, 이 안에 플레이어가 들어올 경우, 공격을 시작.
 * 플레이어가 이 감지범위에서 깔짝깔짝 할 수 있기 때문에, 추적하는 범위는 8정도로 설정해둘 필요.
 * 감지범위에 들어온 플레이어가 추적범위보다 많이 나갈 경우 추적을 종료하는 스크립트 구현 필요.
 * 해결!
*/
public class EnemyBaseTest : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //컴포넌트 이름은 컴포넌트 명의 축소형
    [Header("Component")]
    Animator anim;
    GameObject player;
    NavMeshAgent agent;
    SphereCollider detectRangeCollider;
    Spawner spawner;

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
    public float intervalAtack = 1f;
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
                Debug.Log("SCOUT Property");
                anim.SetBool("Chase", false);
                anim.ResetTrigger("Idle");
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
                anim.SetBool("Chase", false);
                agent.speed = 0f;
                //agent.isStopped = true;
                //agent.Stop();
            }

            _state = value;
        }
    }

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void Awake()
    {

        spawner = transform.parent.GetComponent<Spawner>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        detectRangeCollider = GetComponent<SphereCollider>();

        waitAtack = WaitAtack();

        Transform trans = transform.parent;
        scoutPoint = new Vector3[trans.childCount-1];
        for (int i = 0; i < scoutPoint.Length; i++)
            scoutPoint[i] = trans.GetChild(i).position;

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
        Heart = maxHeart;
        scoutIndex = 0;
        State = EnemyState.IDLE;
        detectRangeCollider.radius = detectRange;
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
            //Debug.Log("정찰 시작");
            State = EnemyState.SCOUT;
        }
    }
    protected virtual void EnemyModeScout()
    {
        if(agent.remainingDistance < 0.2f)
        {
            //Debug.Log("목표 도착");
            State = EnemyState.IDLE;
        }
    }
    protected virtual void EnemyModeChase()
    {
        if (agent.remainingDistance < 2.5f)
        {
            State = EnemyState.ATACK;
        }
        
    }
    protected virtual void EnemyModeAtack()
    {
        StartCoroutine(waitAtack);
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
        isWaitScout = true;
        yield return new WaitForSeconds(intervalScout);
        isWaitScout = false;
    }

    IEnumerator WaitAtack()
    {
        transform.LookAt(player.transform.position);
        yield return new WaitForSeconds(3f);      // 공격 대기시간동안 플레이어를 바라보게
        anim.SetTrigger("Atack");
        
        
        
    }

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && playerDetect==false)
        {
            playerDetect = true;
            //Debug.LogWarning("감지범위 들어옴");
            //Debug.Log($"★ '{transform.gameObject.name}' 가 '{other}' 를 발견");
            detectRangeCollider.radius = atackRange;
            player = other.gameObject;
            State = EnemyState.CHASE;
        }
        if (other.CompareTag("Weapon"))
        {
            Debug.LogWarning("아야!");
            Heart--;
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