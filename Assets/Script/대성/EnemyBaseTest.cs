using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyBaseTest : MonoBehaviour
{
    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------

    //컴포넌트 이름은 컴포넌트 명의 축소형
    [Header("Component")]
    Rigidbody rigid;
    Animator anim;
    GameObject player;
    NavMeshAgent agent;

    [Header("Enemy Information")]
    public int heart = 3;
    public float enemySpeed = 5f;
    public float normalSpeed = 5f;
    public float chaseSpeed = 8f;
    public float detectRange=5f;

    
    [Header("Scout Information")]
    Vector3[] scoutPoint;
    public int scoutIndex=0;
    Vector3 initializePosition= Vector3.zero;
    Vector3 targetDirection= Vector3.zero;

    //Flag변수는 항상 is로 시작
    [Header("Flag")]
    public bool isdetectPlayer = false;
    public bool isAtacking = false;
    public bool isWaitScout = false;

    // 시간 간격 변수명은 interval로 시작
    [Header("Timer")]
    public float intervalScout = 3f;
    public float intervalAtack = 1f;



    [Header("Test")]
    InputSystemController inputController;

    //--------Value----------------Value----------------Value----------------Value----------------Value----------------Value----------------Value-------------

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------
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
            if (_state == EnemyState.IDLE)
            {
                agent.Stop();
                StartCoroutine(WaitScout());
                anim.SetTrigger("Idle");
            }
            else if (_state == EnemyState.SCOUT)
            {
                
                anim.SetTrigger("Scout");

                targetDirection = scoutPoint[scoutIndex];
                scoutIndex++;
                if(scoutIndex==scoutPoint.Length)
                    scoutIndex %= scoutPoint.Length;

                agent.speed = enemySpeed;
                agent.SetDestination(targetDirection);
                agent.Resume();
            }
            else if (_state == EnemyState.CHASE)
            {
                anim.SetTrigger("Chase");
            }
            else if (_state == EnemyState.ATACK)
            {
                anim.SetTrigger("Atack");
            }

            _state = value;
        }
    }

    //--------Property----------------Property----------------Property----------------Property----------------Property----------------Property----------------

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        initializePosition = transform.position;
        Transform trans = transform.GetChild(3);
        scoutPoint = new Vector3[trans.childCount];
        for (int i = 0; i < scoutPoint.Length; i++)
            scoutPoint[i] = trans.GetChild(i).position;

        //TEST//
        inputController = new InputSystemController();
        //TEST//
    }
    private void Start()
    {
        scoutIndex = 0;
        State = EnemyState.IDLE;
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
    }

    private void OnDisable()
    {
        TestSettingOff();
    }

    //--------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------LifeCycle----------------

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------

    void EnemyModeIdle()
    {
        if(isWaitScout==false)
            State = EnemyState.SCOUT;
    }
    void EnemyModeScout()
    {
        if(agent.remainingDistance < 0.2f)
            State = EnemyState.IDLE;
    }
    void EnemyModeChase()
    {

    }
    void EnemyModeAtack()
    {

    }

    //--------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI----------------AI--------
    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    IEnumerator WaitScout()
    {
        isWaitScout = true;
        yield return new WaitForSeconds(intervalScout);
        isWaitScout = false;

    }

    //--------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------Coroutine----------------

    //--------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON----------------ON--------

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"★ '{transform.gameObject.name}' 가 '{other}' 를 발견");
        State = EnemyState.ATACK;
           
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"☆ '{transform.gameObject.name}' 가 '{other}' 를 떠남");
        scoutIndex = 0;
        State = EnemyState.SCOUT;
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
}