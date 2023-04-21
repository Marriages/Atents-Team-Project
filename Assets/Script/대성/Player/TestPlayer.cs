using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
/*
 * 1. JUMP하며 움직이던 중 사망시, 미끄러지듯이 사망함.
 *    Velocity를 OnGround에서 초기화가되지만, fixedUPdate가 계속 실행됨.  -> DIe할 경우 MoveSpeed=0을 하던 IsALive로 제어를 하던
 *    해결
 *    
 * 2. 포션을 먹는 도중 처맞게 될 경우 문제 발생함
 * 3. JUMP할떄 Disable -> OnGround의 Enable하는 것의 개수가 차이가 남.
 *    HIT가 끝났을 때, 점프가 끝났을 떄, Restore함수를 호출함으로써 초기 상태로 되돌리는 방법을 "생각"해보기 / God모드 끝날때도
 *    해결
 *    
 * 4. 플레이어가 씬 이동한 후, 초기 위치 지정하는..... Virtual Camera를 LookMode가 바뀔떄 같이 바꿔보기 ( 3인칭, 탑뷰모드 )
 * 5. 상점 / 서브신 플레이어 정상적으로 움직이고, MapManager가 시점을 변경할 수 있게끔?
 * 6. 메인맵의 나무들에 콜라이더 넣어주기
 * 7. 메인맵의 끝에서 일정시간 가만히 있을 시, 주위를 쫙 둘러봐주는 Dolly 구현하기.
 * 8. 사망 패널은 구현했으나, Button을 만들어서 처음부터 할 수 있도록 할 것.
 * 9. Enemy가 플레이어 발견시(특히 SwardMan) 바로 공격할 수 있도록 할 것
 * 10. 효과음 / 배경음
 * 11. 로딩씬, 엔딩씬 구현하기
 * 12. 맞으면 카메라 흔들기?
 * 13. 이동시 카메라 기준으로 캐릭터가 회전하게 만들기(20일 수업시간에 배움)
 * 14. 
 * 
 */
public class TestPlayer : MonoBehaviour
{
    [Header("Player Information")]
    public float moveSpeed = 5f;
    float repairMoveSpeed;
    public float jumpPower = 5f;
    int heart=3;
    int maxHeart = 3;
    public int coin = 0;

    public int Heart
    {
        get => heart;
        set
        {
            if (value - heart == 1)        //1 회복
            {
                if (value > maxHeart)
                    heart = maxHeart;
                else
                    heart = value;
                healEffect.SetActive(true);
            }
            else if (value - heart == 2)
            {
                if (value > maxHeart)
                    heart = maxHeart;
                else
                    heart = value;
                potionEffect.SetActive(true);
            }
            else
            {
                if (value < 1)
                {
                    heart = 0;
                    Die();
                }
                else
                {
                    heart = value;
                }
            }
            Debug.Log($"Heart : {heart}");
            HeartChange?.Invoke(heart);
        }
    }
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            CoinChange(coin);
        }
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
    public  bool haveScroll = false; // test를 위하여 public으로 선언하였음. 테스트 종료 후 private로 변환할 것
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
            WeaponChange?.Invoke(weaponGet);
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
            ShieldChange?.Invoke(shieldGet);
        }
    }
    public bool PotionSetting
    {
        get => potionGet;
        set
        {
            if (value == true)
            {
                PotionGet?.Invoke();         //UI에 신호보내기
                potionGet = value;
            }
            else
            {
                PotionUse?.Invoke();        //UI에 신호보내기
                potionGet = value;
            }
            PotionChange?.Invoke(potionGet);
        }
    }
    public bool ScrollSetting
    {
        get => haveScroll;
        set
        {
            if(value==true)
            {
                Debug.Log("스크롤 구입 완료. 비밀문이 열립니다.");
                OpenSecretDoor?.Invoke();
            }
        }
    }


    [Header("Input & Move")]
    Vector3 moveDir = Vector3.zero;
    Vector3 moveDirMouse = Vector3.zero;
    Vector3 playerRotate = Vector3.zero;        //카메라회전
    Vector3 forward = Vector3.zero;
    Vector3 right = Vector3.zero;
    float turnSpeed = 10f;
    Quaternion turnDir=Quaternion.identity;
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
    GameObject playerHitEffect;
    Collider playerCollider;


    [Header("Action")]
    //public Action<int> HeartChange;
    //public Action<int> CoinChange;
    //public Action<bool> PotionChange;
    public Action<int> HeartChange;
    public Action<int> CoinChange;
    public Action PotionGet;
    public Action PotionUse;

    public Action<bool> WeaponChange;
    public Action<bool> ShieldChange;
    public Action<bool> PotionChange;
    public Action PlayerDie;

    public Action OpenSecretDoor;

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
        playerHitEffect = transform.GetChild(5).gameObject;
        playerCollider = transform.GetComponent<Collider>();

        cameraMain = FindObjectOfType<MainCamera_Action>().transform.GetChild(0).gameObject;
    }
    private void FixedUpdate()
    {
        //키보드로 화면 회전하는 탑뷰모드
        if (isMoving == true && lookModeThire == true && isShilding == false && isAlive==true)
        {
            rigid.MovePosition(rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            //transform.LookAt(transform.position + moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation , turnDir, Time.fixedDeltaTime * turnSpeed);
        }
        else if (isMoving == true && lookModeThire == false && isShilding == false && isAlive==true)
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
        repairMoveSpeed = moveSpeed;
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
            //Debug.Log($"{other.gameObject.name} 에게 한대 처맞음");
            anim.SetTrigger("Hit");

            rigid.velocity = Vector3.zero;

            playerHitEffect.SetActive(true);
            potion.SetActive(false);

            Heart -= 1;
        }
        else if(other.gameObject.CompareTag("Coin") && isAlive==true)
        {
            Destroy(other.gameObject);
            Coin += 1;
        }
        else if (other.gameObject.CompareTag("Heart") && isAlive == true)
        {
            Destroy(other.gameObject);
            Heart += 1;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Ground!");
            isJumping = false;

            rigid.velocity = Vector3.zero;

            inputActions.Player.Move.Enable();
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
            turnDir = Quaternion.LookRotation(moveDir, transform.up);
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
            //Debug.Log("Defense Success");
            anim.SetTrigger("Defense");
            GodModeOn();
            rigid.AddForce(-transform.forward, ForceMode.Impulse);
        }
    }
    void DefenseSuccessEnd()
    {
        //Debug.Log("Defense Motion End");
        rigid.velocity = Vector3.zero;
        GodModeOff();
    }
    //-----------------------------------------------------------------------

    private void PlayerPotion(InputAction.CallbackContext obj)
    {
        if (PotionSetting == true)      //포션이 있는 상태에서만
        {
            anim.SetTrigger("Potion");
            potion.SetActive(true);
            if(shieldGet==true)
                shield.gameObject.SetActive(false);
        }
    }
    void PotionApply()
    {
        //Debug.Log($"Potion Before Heart : {Heart}");
        potionEffect.SetActive(true);
        Heart = Heart + 2;
        PotionSetting = false;
    }
    void PotionEnd()
    {
        potion.SetActive(false);
        if(shieldGet==true)
            shield.gameObject.SetActive(true);
    }

    //-----------------------------------------------------------------------

    private void PlayerJump(InputAction.CallbackContext obj)
    {
        if (isJumping == false)
        {
            //Debug.Log("Jump!");
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
        //Debug.Log("God Mode On");
        isGodMode = true;
        moveSpeed = 0;
        //Debug.Log("God Mode");
        //gameObject.layer = 10;      //적과 충돌하지 않게끔 레이어 변경
    }
    void GodModeOff()
    {
        //Debug.Log("God Mode Off");
        isGodMode = false;
        moveSpeed = repairMoveSpeed;
        //Debug.Log("God Mode End");
        //gameObject.layer = 6;       //적과 충돌하게끔 원래 Player 레이어 변경
    }
    void RestoreState()
    {
        //Debug.Log("Restore");
        inputActions.Player.Enable();
        GodModeOff();
        InitializeConnecting();     // 혹시몰라 다시실행함.

        weaponCollider.enabled = false;
        shieldCollider.enabled = false;

        potion.SetActive(false);

        if (shieldGet == true)
            shield.gameObject.SetActive(true);      // 포션마실때방패 끄는데, 처맞으면 다시복구
    }

    void Die()
    {
        rigid.velocity=Vector3.zero;
        isAlive = false;
        anim.SetTrigger("Die");
        PlayerDie?.Invoke();
        InitializeUnConnecting();
    }
}
