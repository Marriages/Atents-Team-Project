using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드

/*
 * 성능 높이기 위해 노력 할 것 
 * https://docs.unity3d.com/kr/530/Manual/LightProbes.html
 * https://ko.wikipedia.org/wiki/%EA%B8%80%EB%A1%9C%EB%B2%8C_%EC%9D%BC%EB%A3%A8%EB%AF%B8%EB%84%A4%EC%9D%B4%EC%85%98
 * 
 * 
 */


public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 90.0f;

    // 플레이어 점프 속도
    public float jumpPower = 6.0f;
    bool IsJumping = false;

    // 플레이어 방패들기 활성화/비활성화 변수
    private bool state;

    // 플레이어 입력 방향
    Vector2 moveDir = Vector2.zero;

    // 카메라 관련 변수
    GameObject cameraMain;
    public float smoothness = 10.0f;
    public bool toggleCameraRotation;

    // 입력처리용 인풋액션
    protected PlayerInputActions inputActions;

    // 플레이어 리지드바디
    private Rigidbody rigid;

    // 애니메이터 컨트롤러
     Animator anim;

    // 플레이어 생명
    private int heart = 3;


    private SpriteRenderer spriteRenderer;




    // 플레이어 점수
    private int coin = 0;
    public bool weaponGet = false;
    public bool shieldGet = false;
    public bool potionGet = false;
    public bool isAlive = true;     //수정함----------------------------------------------------------------------------------------------------------------------

    // 델리게이트
    public Action<int> HeartPlus;
    public Action<int> HeartMinus;
    public Action<int> CoinPlus;
    public Action<int> CoinMinus;
    public Action PotionGet;
    public Action PotionUse;
    public Action WeaponGet;
    public Action ShieldGet;
    public Action PlayerDie;
    

    // 포션,무기, 방패 관리용
    public GameObject potion;
    public Collider weaponCol;
    public Collider shieldCol;
    Shield shield;

    // 하트 프로퍼티
    public int Heart
    {
        get => heart;
        set
        {
            if (heart < value && value<3)            //회복
            {
                //값이 현재 heart보다 크고, 3보다 작다면 1 회복
                heart = value;
                HeartPlus?.Invoke(1);
            }
            else if (heart > value)       //피격
            {
                //Debug.Log("피격 시퀀스 가동");

                inputActions.Player.Move.canceled -= PlayerMove;
                inputActions.Player.Move.performed -= PlayerMove;

                if (value <= 0)
                {
                    Debug.Log("사망 시퀀스 가동");
                    isAlive = false;
                    moveSpeed = 0f;
                    anim.SetBool("IsDie",true);
                                               //  + animation Controller Any State -> Die에  Bool IsDie 및 IsHit 에 의해 작동하게 수정및 변경  /  AnyState -> Potion, Atack에 IsDie가 false여야만 작동할 수 있게 부울 조건 추가함.
                    PlayerDie?.Invoke();
                }
                else
                {
                    Debug.Log("생명력 감소");

                    inputActions.Player.Move.performed += PlayerMove;
                    inputActions.Player.Move.canceled += PlayerMove;

                    heart = value;
                    HeartMinus?.Invoke(1);
                }


            }
        }
    }

    // 코인 프로퍼티
    public int Coin
    {
        get => coin;
        set
        {
            if (coin > value)
            {
                coin = value;
                CoinMinus?.Invoke(1);
            }

            else if (coin < value)
            {
                coin = value;
                CoinPlus?.Invoke(1);
            }
        }
    }

    //--------생명주기--------생명주기--------생명주기--------생명주기--------생명주기--------생명주기
    private void Awake()
    {
        Heart = 3;

        weaponCol.enabled = false;
        shieldCol.enabled = false;
        shield = FindObjectOfType<Shield>();

        cameraMain = GameObject.FindWithTag("MainCamera");


        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        

        inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(cameraMain.transform.forward, new(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate),
                Time.deltaTime * smoothness);
        }
    }

    private void FixedUpdate()
    {
        Move();
        
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += PlayerMove;
        inputActions.Player.Move.canceled += PlayerMove;
        inputActions.Player.Attack.performed += PlayerAttack;
        inputActions.Player.Shield.performed += PlayerShield;
        inputActions.Player.Potion.performed += PlayerPotion;
        inputActions.Player.Jump.performed += PlayerJump;
        inputActions.Player.Use.performed += PlayerUse;

    }
    private void OnDisable()
    {
        inputActions.Player.Use.performed -= PlayerUse;
        inputActions.Player.Jump.performed -= PlayerJump;
        inputActions.Player.Potion.performed -= PlayerPotion;
        inputActions.Player.Shield.performed -= PlayerShield;
        inputActions.Player.Attack.performed -= PlayerAttack;
        inputActions.Player.Move.canceled -= PlayerMove;
        inputActions.Player.Move.performed -= PlayerMove;
        inputActions.Player.Disable();
    }

    // 플레이어 이동 관련 이벤트 함수
    private void PlayerMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        moveDir = dir;

        anim.SetBool("IsMove", !context.canceled);

    }

    void Move()
    {
        Vector2 moveInput = new Vector2(moveDir.x, moveDir.y);
        bool isMove = moveDir.magnitude != 0;
        if (isMove)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;


            rigid.MovePosition(rigid.position + moveSpeed * Time.fixedDeltaTime * moveDirection);
        }
    }







    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (IsJumping==false && isAlive==true)         // 점프 중이 아닐 때만 그리고 살아있을때만
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);  // 월드의 Up방향으로 힘을 즉시 가하기
            IsJumping = true;   // 점프중이라고 표시
            anim.SetTrigger("IsJump");
            inputActions.Player.Potion.Disable();
            inputActions.Player.Shield.Disable();
            inputActions.Player.Attack.Disable();
        } 
    }
   


    // 플레이어 충돌 관련 이벤트 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isAlive==true)      //Enemy이고 살아있을때만.
        {
            //Debug.Log($"플레이어가 {other.gameObject.name}에게 피격당함!");
            anim.SetTrigger("IsHit");       //수정함----------------------------------------------------------------------------------------------------------------------  Animator Controller 중 Idle -> Hit로가는 IsHit Trigger 설정함(has exit Time 뺐음)
            --Heart;
            
            // 플레이어가 적과 충돌시 PlayerGod레이어로 변경(PlayerGod은 무적상태)
            gameObject.layer = 10;

            
            Invoke("OffGod", 3);
        }
        else if(other.gameObject.CompareTag("Coin") && isAlive==true)
        {
            ++Coin;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Heart") && isAlive == true)
        {
            ++Heart;
            Destroy(other.gameObject);
        }
    }

    // 무적시간 해제 함수
    void OffGod()
    {
        gameObject.layer = 6;
        
    }

    // 새로 추가한 부분
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isAlive==true)   // Ground와 충돌했을 때만, 살아있을때만
        {
            OnGround();     // 착지 함수 실행
            
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            moveSpeed = 0;
            if (IsJumping == false)
            {
                moveSpeed = 5.0f;
            }

        }

    }

    

    // 착지했을 때 처리 함수
    void OnGround()
    {
        IsJumping = false;      // 점프가 끝났다고 표시
        moveSpeed = 5.0f;
        inputActions.Player.Potion.Enable();
        inputActions.Player.Shield.Enable();
        inputActions.Player.Attack.Enable();
    }

    // 플레이어 공격 관련 이벤트 함수
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        anim.SetTrigger("IsAttack"); //playerAnimator의 isAttack 트리거를 작동한다
        
    }

    // 플레이어 실드 관련 이벤트 함수
    private void PlayerShield(InputAction.CallbackContext context)
    {
        Shield();
    }

    // 실드 애니메이션 활성화/비활성화
    void Shield()
    {
        if (state == false)
        {
            anim.SetBool("IsSheild", true);
            state = true;
            moveSpeed = 0;
            inputActions.Player.Attack.Disable();
            inputActions.Player.Potion.Disable();
            inputActions.Player.Jump.Disable();
            shieldCol.enabled = true;
        }
        else
        {
            anim.SetBool("IsSheild", false);
            state = false;
            moveSpeed = 5.0f;
            inputActions.Player.Attack.Enable();
            inputActions.Player.Potion.Enable();
            inputActions.Player.Jump.Enable();
            shieldCol.enabled = false;
        }
    }

    // 플레이어 포션 관련 이벤트 함수
    private void PlayerPotion(InputAction.CallbackContext context)
    {
        anim.SetTrigger("IsPotion");
    }

    // 플레이어 상호작용 관련 이벤트 함수
    private void PlayerUse(InputAction.CallbackContext obj)
    {
        
    }

    void PotionStart()
    {
        inputActions.Player.Disable();
        potion.SetActive(true);
    }
    void PotionEnd()
    {
        inputActions.Player.Enable();
        potion.SetActive(false);
    }
    void AttackStart()
    {
        inputActions.Player.Disable();
    }
    void AttackEnd()
    {
        inputActions.Player.Enable();
    }
    void AttackDamegeOn()
    {
        weaponCol.enabled = true;
    }
    void AttackDamegeOff()
    {
        weaponCol.enabled = false;
    }
    void HitStart()
    {
        inputActions.Player.Disable();

    }
    void HitEnd()
    {
        inputActions.Player.Enable();
    }

}
