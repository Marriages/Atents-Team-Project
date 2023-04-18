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


    private void Awake()
    {
        FindComponent();
    }
    void FindComponent()
    {
        anim = GetComponent<Animator>();
        inputActions = new InputSystemController();
        rigid = GetComponent<Rigidbody>();
        //weapon = transform.GetComponentInChildren<Weapon>().gameObject;
        //shield = transform.GetComponentInChildren<Shield>().gameObject;
        //potion = transform.GetComponentInChildren<Potion>().gameObject;

        cameraMain = FindObjectOfType<MainCamera_Action>().transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        //키보드로 화면 회전하는 탑뷰모드
        if(isMoving==true && lookModeThire==true && isAlive==true )
        {
            rigid.MovePosition(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            transform.LookAt(transform.position + moveDir,Vector3.up);
        }
        else if(isMoving == true && lookModeThire==false && isAlive==true)
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            moveDirMouse = forward * moveDir.z + right * moveDir.x;

            rigid.MovePosition(rigid.position + moveDirMouse * moveSpeed * Time.fixedDeltaTime);

            playerRotate = Vector3.Scale(cameraMain.transform.forward, new(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate),Time.deltaTime * smoothness);
        }
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
        inputActions.Player.Use.performed += PlayerUse;
        inputActions.Player.ViewChange.performed += PlayerViewChange;
    }
    void InitializeUnConnecting()
    {
        inputActions.Player.ViewChange.performed -= PlayerViewChange;
        inputActions.Player.Use.performed -= PlayerUse;
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

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        isMoving = false;
        anim.SetTrigger("Atack");
    }
    void AtackEnd()
    {
        isMoving = true;
    }
    void WeaponColliderOn()
    {
        weaponCollider.enabled = true;
    }
    void WeaponColliderOff()
    {
        weaponCollider.enabled = false;
    }

    private void PlayerShield(InputAction.CallbackContext obj)
    {/*
        if (isShilding == false)
        {
            anim.SetBool("IsSheild", true);
            isShilding = true;
            moveSpeed = 0;
            inputActions.Player.Attack.Disable();
            inputActions.Player.Potion.Disable();
            inputActions.Player.Jump.Disable();
            shieldCollider.enabled = true;
        }
        else
        {
            anim.SetBool("IsSheild", false);
            isShilding = false;
            moveSpeed = 5.0f;
            inputActions.Player.Attack.Enable();
            inputActions.Player.Potion.Enable();
            inputActions.Player.Jump.Enable();
            shieldCollider.enabled = false;
        }*/
    }

    private void PlayerPotion(InputAction.CallbackContext obj)
    {
    }

    private void PlayerJump(InputAction.CallbackContext obj)
    {/*
        if (isJumping == false && isAlive == true)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJumping = true;
            anim.SetTrigger("IsJump");
            inputActions.Player.Potion.Disable();
            inputActions.Player.Shield.Disable();
            inputActions.Player.Attack.Disable();
        }
    */
    }

    private void PlayerUse(InputAction.CallbackContext obj)
    {
    
    }
    private void PlayerViewChange(InputAction.CallbackContext _)
    {
        lookModeThire = !lookModeThire;
    }

    //---------------Animation Event
}
