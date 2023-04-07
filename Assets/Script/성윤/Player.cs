using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드


public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    public float moveSpeed = 5.0f;

    // 플레이어 회전 속도
    public float rotateSpeed = 180.0f;

    // 플레이어 점프 속도
    public float jumpPower = 6.0f;
    bool IsJumping = false;

    

    // 플레이어 방패들기 활성화/비활성화 변수
    private bool state;

    // 입력처리용 인풋액션
    protected PlayerInputActions inputActions;

    // 플레이어 입력 방향
    Vector3 inputDir = Vector3.zero;

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
    public GameObject potion;    //수정함----------------------------------------------------------------------------------------------------------------------        포션 넣을 것
    public Collider weaponCol;   //수정함----------------------------------------------------------------------------------------------------------------------        콜라이더가 있는 무기 넣을것
    public Collider shieldCol;       //수정함----------------------------------------------------------------------------------------------------------------------        콜라이더가 있는 방패 넣을 것
    Shield shield;

    // 하트 프로퍼티
    public int Heart
    {
        get => heart;
        set
        {
            //Debug.Log($"heart:{Heart},value:{value}");
            if (heart < value)            //회복
            {
                Debug.Log("회복 시퀀스 가동");
                if (value > 3)
                    Debug.Log("이미 최대 체력입니다.");
                else
                {
                    heart = value;
                    HeartPlus?.Invoke(1);
                }
            }
            else if (heart > value)       //피격
            {
                //Debug.Log("피격 시퀀스 가동");

                inputActions.Player.Move.canceled -= PlayerMove;//수정함---------------------------------------------------------------------------------------------------------------------- 입력시스템 제거
                inputActions.Player.Move.performed -= PlayerMove;//수정함----------------------------------------------------------------------------------------------------------------------입력시스템 제거

                if (value <= 0)
                {
                    Debug.Log("사망 시퀀스 가동");//수정함----------------------------------------------------------------------------------------------------------------------
                    isAlive = false;//수정함----------------------------------------------------------------------------------------------------------------------
                    moveSpeed = 0f;//수정함----------------------------------------------------------------------------------------------------------------------
                    rotateSpeed = 0f;//수정함----------------------------------------------------------------------------------------------------------------------
                    anim.SetBool("IsDie",true);//수정함----------------------------------------------------------------------------------------------------------------------
                                               //  + animation Controller Any State -> Die에  Bool IsDie 및 IsHit 에 의해 작동하게 수정및 변경  /  AnyState -> Potion, Atack에 IsDie가 false여야만 작동할 수 있게 부울 조건 추가함.
                    PlayerDie?.Invoke();
                }
                else
                {
                    Debug.Log("생명력 감소");//수정함----------------------------------------------------------------------------------------------------------------------

                    inputActions.Player.Move.performed += PlayerMove;//수정함----------------------------------------------------------------------------------------------------------------------입력시스템 복구
                    inputActions.Player.Move.canceled += PlayerMove;//수정함----------------------------------------------------------------------------------------------------------------------입력시스템 복구

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

        weaponCol.enabled = false;  //수정함----------------------------------------------------------------------------------------------------------------------     초기시작시 검의 콜라이더 비활성화. 추후 공격떄 활성화할 예정
        shieldCol.enabled = false;  //수정함----------------------------------------------------------------------------------------------------------------------    초기시작시 방패의 콜라이더 비활성화. 추후 방어할때 활성화할 예정
        shield = FindObjectOfType<Shield>();
        Debug.Log(shield.gameObject.name);


        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        

        inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        
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
        inputDir = dir;

        anim.SetBool("IsMove", !context.canceled);

    }

    void Move()
    {
        Vector3 dir = new Vector3(inputDir.x, 0, inputDir.y);


        rigid.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * dir);
        


    }
    //수정함----------------------------------------------------------------------------------------------------------------------시작
    // 로직이 복잡하거나 상속을 해줄 일이 없기에, 불필요한 함수 Jump()를 삭제 병합함.
    // 플레이어 점프 관련 이벤트 함수
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
    //수정함----------------------------------------------------------------------------------------------------------------------끝
   



    //수정함----------------------------------------------------------------------------------------------------------------------시작
    // 플레이어 충돌 관련 이벤트 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isAlive==true)      //Enemy이고 살아있을때만.
        {
            //Debug.Log($"플레이어가 {other.gameObject.name}에게 피격당함!");
            anim.SetTrigger("IsHit");       //수정함----------------------------------------------------------------------------------------------------------------------  Animator Controller 중 Idle -> Hit로가는 IsHit Trigger 설정함(has exit Time 뺐음)
            Heart--;
            
            // 플레이어가 적과 충돌시 PlayerGod레이어로 변경(PlayerGod은 무적상태)
            gameObject.layer = 10;

            
            Invoke("OffGod", 3);
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
