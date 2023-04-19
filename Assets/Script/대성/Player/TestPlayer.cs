using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TestPlayer : MonoBehaviour
{
    [Header("Player Information")]
    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    int heart;
    int maxHeart = 3;
    int coin = 0;

    [Header("Flag")]
    bool isAlive = true;
    bool isJumping = false;
    bool isShilding = false;
    bool isMoving = false;
    bool weaponGet = false;
    bool shieldGet = false;
    bool potionGet = false;
    public bool lookModeThire = true;     //3인칭시점 꺼져있음. 원하면 킬 것.
   

    [Header("Input & Move")]
    Vector3 moveDir = Vector3.zero;
    Vector3 moveDirMouse = Vector3.zero;
    Vector3 playerRotate = Vector3.zero;        //카메라회전
    Vector3 forward = Vector3.zero;
    Vector3 right = Vector3.zero;
    public float smoothness = 5f;           // 마우스가 보는 시점을 따라 회전하는 속도

    [Header("Component")]
    Animator anim;
    InputSystemController inputActions;
    Rigidbody rigid;

    [Header("GameObject & Collider")]
    GameObject weapon;
    Collider weaponCollider;
    GameObject shield;
    Collider shieldCollider;
    GameObject potion;
    GameObject cameraMain;


    [Header("Action")]
    public Action<int> HeartChange;
    public Action<int> CoinChange;
    public Action<bool> PotionChange;
    public Action WeaponGet;
    public Action ShieldGet;
    public Action PlayerDie;
    public Action PlayerUseTry;         //아이템 상호작용


    private void Awake()
    {
        FindComponent();
    }
    void FindComponent()
    {
        anim = GetComponent<Animator>();
        inputActions = new InputSystemController();
        rigid = GetComponent<Rigidbody>();
        
        weapon = transform.GetComponentInChildren<Weapon>().gameObject;
        weaponCollider = weapon.GetComponent<Collider>();
        shield = transform.GetComponentInChildren<Shield>().gameObject;
        shieldCollider = shield.GetComponent<Collider>();
        potion = transform.GetComponentInChildren<Potion>(true).gameObject;

        cameraMain = FindObjectOfType<MainCamera_Action>().transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        //키보드로 화면 회전하는 탑뷰모드
        if(isMoving==true && lookModeThire==true && isShilding==false )
        {
            rigid.MovePosition(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            transform.LookAt(transform.position + moveDir,Vector3.up);
        }
        else if(isMoving == true && lookModeThire==false && isShilding == false)
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            moveDirMouse = forward * moveDir.z + right * moveDir.x;

            rigid.MovePosition(rigid.position + moveDirMouse * moveSpeed * Time.fixedDeltaTime);

            playerRotate = Vector3.Scale(cameraMain.transform.forward, new(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate),Time.deltaTime * smoothness);
        }/*
        else if( isJumping == true && lookModeThire==true )
        {
            rigid.AddForce(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        else if (isJumping == true && lookModeThire == false)
        {
            rigid.AddForce(rigid.position + moveDirMouse * moveSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }*/


    }
    private void OnEnable()
    {
        InitializeSetting();
        InitializeConnecting();
    }
    private void OnDisable()
    {
        InitializeUnConnecting();
    }
    void InitializeSetting()
    {
        //weapon.SetActive(false);
        //shield.SetActive(false);
        //potion.SetActive(false);

        //weaponCollider.enabled=false;
        //shieldCollider.enabled = false;

        heart=maxHeart;
        coin = 0;

        isAlive = true;
        isJumping = false;
        isShilding = false;
        isMoving = false;
        weaponGet = false;
        shieldGet = false;
        potionGet = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && isAlive==true )
        {
            anim.SetTrigger("isHit");
            --heart;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;

            rigid.velocity= Vector3.zero;

            //inputActions.Player.Move.Enable();
            inputActions.Player.PotionKeyboard.Enable();
            inputActions.Player.PotionMouse.Enable();
            inputActions.Player.ShieldKeyboard.Enable();
            inputActions.Player.ShieldMouse.Enable();
            inputActions.Player.AtackKeyboard.Enable();
            inputActions.Player.AtackMouse.Enable();
        }
    }





    void InitializeConnecting()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += PlayerMove;
        inputActions.Player.Move.canceled += PlayerMove;
        inputActions.Player.AtackKeyboard.performed += PlayerAttack;
        inputActions.Player.AtackMouse.performed += PlayerAttack;
        inputActions.Player.ShieldKeyboard.performed += PlayerShield;
        inputActions.Player.ShieldMouse.performed += PlayerShield;
        inputActions.Player.PotionKeyboard.performed += PlayerPotion;
        inputActions.Player.PotionMouse.performed += PlayerPotion;
        inputActions.Player.Jump.performed += PlayerJump;
        inputActions.Player.ViewChange.performed += PlayerViewChange;
        inputActions.Player.Use.performed += (_) => PlayerUseTry?.Invoke();
    }
    void InitializeUnConnecting()
    {
        inputActions.Player.Use.performed -= (_) => PlayerUseTry?.Invoke();
        inputActions.Player.ViewChange.performed -= PlayerViewChange;
        inputActions.Player.Jump.performed -= PlayerJump;
        inputActions.Player.PotionMouse.performed -= PlayerPotion;
        inputActions.Player.PotionKeyboard.performed -= PlayerPotion;
        inputActions.Player.ShieldMouse.performed -= PlayerShield;
        inputActions.Player.ShieldKeyboard.performed -= PlayerShield;
        inputActions.Player.AtackMouse.performed -= PlayerAttack;
        inputActions.Player.AtackKeyboard.performed -= PlayerAttack;
        inputActions.Player.Move.canceled -= PlayerMove;
        inputActions.Player.Move.performed -= PlayerMove;
        inputActions.Player.Disable();
    }

    private void PlayerMove(InputAction.CallbackContext obj)
    {
        Vector3 dir = obj.ReadValue<Vector3>();
        if(dir != Vector3.zero )
        {
            moveDir = dir.normalized;
            isMoving = true;
        }
        else
        {
            moveDir = Vector3.zero;
            isMoving = false;
        }
        anim.SetBool("Move", isMoving);
    }

    private void PlayerAttack(InputAction.CallbackContext obj)    {
        isMoving = false;
        inputActions.Player.Disable();
        anim.SetTrigger("Atack");
    }
    void AtackEnd()    {
        isMoving = true;
        inputActions.Player.Enable();
    }
    void WeaponColliderOn()    {
        weaponCollider.enabled = true;
    }
    void WeaponColliderOff()    {
        weaponCollider.enabled = false;
    }

    private void PlayerShield(InputAction.CallbackContext obj)
    {
        Debug.Log("Press Shield");
        if(isShilding==false)
        {
            isShilding = true;
            anim.SetBool("Shield", isShilding);
            rigid.velocity = Vector3.zero;        //플레이어 멈추기

            inputActions.Player.Move.Disable();
            inputActions.Player.AtackKeyboard.Disable();
            inputActions.Player.AtackMouse.Disable();
            inputActions.Player.Jump.Disable();
            inputActions.Player.PotionKeyboard.Disable();
            inputActions.Player.PotionMouse.Disable();
        }
        else if (isShilding == true)
        {
            isShilding = false;
            anim.SetBool("Shield", isShilding);

            shieldCollider.enabled = false;
            inputActions.Player.Move.Enable();
            inputActions.Player.AtackKeyboard.Enable();
            inputActions.Player.AtackMouse.Enable();
            inputActions.Player.Jump.Enable();
            inputActions.Player.PotionKeyboard.Enable();
            inputActions.Player.PotionMouse.Enable();
        }
    }
    void ShieldStart()
    {
        shieldCollider.enabled = true;
    }
    void ShieldEnd()
    {
        
    }

    private void PlayerPotion(InputAction.CallbackContext obj)
    {
    }

    private void PlayerJump(InputAction.CallbackContext obj)
    {
        if(isJumping == false)
        {
            isJumping = true;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetTrigger("Jump");

            inputActions.Player.PotionKeyboard.Disable();
            inputActions.Player.PotionMouse.Disable();
            inputActions.Player.ShieldKeyboard.Disable();
            inputActions.Player.ShieldMouse.Disable();
            inputActions.Player.AtackKeyboard.Disable();
            inputActions.Player.AtackMouse.Disable();
        }

        // 점프시 벽멈춤 현상은 마찰력으로 인해 발생함.
        // Physic Material을 생성해서 Collider에 넣어줌으로 해결.
        // https://mayquartet.tistory.com/47
    }

    private void PlayerViewChange(InputAction.CallbackContext _)    {
        lookModeThire = !lookModeThire;
    }

    //---------------Animation Event
}
