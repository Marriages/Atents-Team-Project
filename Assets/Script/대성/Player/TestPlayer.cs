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

    public int Heart
    {
        get => heart;
        set
        {
            if (value - heart == 1)        //1 회복
            {
                heart = value % maxHeart;
                healEffect.SetActive(true);
            }
            else if (value - heart == 2)
            {
                heart = value % maxHeart;
                potionEffect.SetActive(true);
            }
            else
            {
                if (value < 1)
                {
                    heart = 0;
                    PlayerDie();
                    anim.SetTrigger("Die");
                }
            }
        }
    }
    public int COin
    {
        get => coin;
        set => coin = value;
    }

    [Header("Flag")]
    bool isAlive = true;
    bool isJumping = false;
    bool isShilding = false;
    bool isMoving = false;
    bool weaponGet = false;
    bool shieldGet = false;
    bool potionGet = false;
    bool isGodMode = false;
    public bool lookModeThire = true;     //3인칭시점 꺼져있음. 원하면 킬 것.

    public bool WeaponSetting
    {
        get => weaponGet;
        set
        {
            if (value == true)
            {
                //Debug.Log("무기 활성화");
                weapon.gameObject.SetActive(true);
                weaponGet = value;
            }
            else
            {
                weapon.gameObject.SetActive(false);
                weaponGet = value;
            }
        }
    }
    public bool ShieldSetting
    {
        get => shieldGet;
        set
        {
            if (value == true)
            {
                //Debug.Log("방어구 활성화");
                shield.gameObject.SetActive(true);
                shieldGet = value;
            }
            else
            {
                shield.gameObject.SetActive(false);
                shieldGet = value;
            }
        }
    }
    public bool PotionSetting
    {
        get => potionGet;
        set
        {
            if (value == true)
            {
                PotionChange(true);         //UI에 신호보내기
                potionGet = value;
            }
            else
            {
                PotionChange(false);        //UI에 신호보내기
                potionGet = value;
            }
        }
    }


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
    Shield shield;
    Collider shieldCollider;
    GameObject potion;
    GameObject potionEffect;
    GameObject cameraMain;
    GameObject healEffect;
    Collider playerCollider;


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
        shield = transform.GetComponentInChildren<Shield>();
        shieldCollider = shield.GetComponent<Collider>();
        potion = transform.GetComponentInChildren<Potion>(true).gameObject;
        potionEffect = transform.GetChild(3).gameObject;
        healEffect = transform.GetChild(4).gameObject;
        playerCollider = transform.GetComponent<Collider>();

        cameraMain = FindObjectOfType<MainCamera_Action>().transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        //키보드로 화면 회전하는 탑뷰모드
        if (isMoving == true && lookModeThire == true && isShilding == false)
        {
            rigid.MovePosition(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            transform.LookAt(transform.position + moveDir, Vector3.up);
        }
        else if (isMoving == true && lookModeThire == false && isShilding == false)
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            moveDirMouse = forward * moveDir.z + right * moveDir.x;

            rigid.MovePosition(rigid.position + moveDirMouse * moveSpeed * Time.fixedDeltaTime);

            playerRotate = Vector3.Scale(cameraMain.transform.forward, new(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
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

        heart = maxHeart;
        coin = 0;

        isAlive = true;
        isJumping = false;
        isShilding = false;
        isMoving = false;
        WeaponSetting = false;
        ShieldSetting = false;
        potionGet = false;
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

        shield.SuccessDefense += DefenseSuccess;
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


    private void OnTriggerEnter(Collider other)
    {
        // 맞더라도 GodMode에서는 맞지말자!
        if (other.gameObject.CompareTag("Enemy") && isAlive == true && isGodMode == false)
        {
            Debug.Log($"{other.gameObject.name} 에게 한대 처맞음");
            anim.SetTrigger("Hit");
            --heart;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;

            rigid.velocity = Vector3.zero;

            //inputActions.Player.Move.Enable();
            inputActions.Player.PotionKeyboard.Enable();
            inputActions.Player.PotionMouse.Enable();
            inputActions.Player.ShieldKeyboard.Enable();
            inputActions.Player.ShieldMouse.Enable();
            inputActions.Player.AtackKeyboard.Enable();
            inputActions.Player.AtackMouse.Enable();
        }
    }

    private void PlayerMove(InputAction.CallbackContext obj)
    {
        Vector3 dir = obj.ReadValue<Vector3>();
        if (dir != Vector3.zero)
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

    //-----------------------------------------------------------------------

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (weaponGet == true)
        {
            isMoving = false;
            inputActions.Player.Disable();
            anim.SetTrigger("Atack");
        }
    }
    void AtackEnd()
    {
        isMoving = true;
        inputActions.Player.Enable();
    }
    void WeaponColliderOn()
    {
        weaponCollider.enabled = true;
    }
    void WeaponColliderOff()
    {
        weaponCollider.enabled = false;
    }

    //-----------------------------------------------------------------------

    private void PlayerShield(InputAction.CallbackContext obj)
    {
        if (shieldGet == true)
        {
            if (isShilding == false)
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
    }

    void ShieldStart()
    {
        shieldCollider.enabled = true;
    }
    void DefenseSuccess()
    {
        if (isGodMode == false)
        {
            Debug.Log("Defense Success");
            anim.SetTrigger("Defense");
            GodModeOn();
            rigid.AddForce(-transform.forward, ForceMode.Impulse);
        }
    }
    void DefenseSuccessEnd()
    {
        Debug.Log("Defense Motion End");
        rigid.velocity = Vector3.zero;
        GodModeOff();
    }
    //-----------------------------------------------------------------------

    private void PlayerPotion(InputAction.CallbackContext obj)
    {
        if (PotionSetting == true)
        {
            anim.SetTrigger("Potion");

            PotionSetting = false;
        }
    }
    void PotionStart()
    {
        potionEffect.SetActive(false);
    }
    void PotionEnd()
    {
        potionEffect.SetActive(false);
        Heart = Heart + 2;
    }

    //-----------------------------------------------------------------------

    private void PlayerJump(InputAction.CallbackContext obj)
    {
        if (isJumping == false)
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

    //-----------------------------------------------------------------------

    private void PlayerViewChange(InputAction.CallbackContext _)
    {
        lookModeThire = !lookModeThire;
    }

    //---------------Animation Event
    void GodModeOn()
    {
        isGodMode = true;
        Debug.Log("God Mode");
        //gameObject.layer = 10;      //적과 충돌하지 않게끔 레이어 변경
    }
    void GodModeOff()
    {
        isGodMode = false;
        Debug.Log("God Mode End");
        //gameObject.layer = 6;       //적과 충돌하게끔 원래 Player 레이어 변경
    }
    void RestoreState()
    {
        isMoving = false;
        inputActions.Player.Enable();
        weaponCollider.enabled = false;
        shieldCollider.enabled = false;
    }
}
