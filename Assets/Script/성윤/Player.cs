using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드


public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    public float moveSpeed = 5.0f;

    // 플레이어 회전 속도
    public float rotateSpeed = 20.0f;

    // 플레이어 점프 속도
    public float jumpPower = 6.0f;
    bool IsJumping = false;

    // 플레이어 방패 활성화/비활성화 변수
    private bool state;

    // 입력처리용 인풋액션
    PlayerInputActions inputActions;

    // 플레이어 입력 방향
    Vector3 inputDir = Vector3.zero;

    // 플레이어 리지드바디
    private Rigidbody rigid;

    // 애니메이터 컨트롤러
    Animator anim;

    // 플레이어 애니메이션 끝나는 시간
    float exitTime = 0.9f;

    // 플레이어 생명
    private int heart = 3;

    // 플레이어 점수
    private int coin = 0;
    public bool weaponGet = false;
    public bool shieldGet = false;
    public bool potionGet = false;

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
            else if (heart > value)                               //피격
            {
                Debug.Log("피격 시퀀스 가동");

                if (value <= 0)
                {
                    PlayerDie?.Invoke();
                }
                else
                {
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
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        inputActions = new PlayerInputActions();
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

    }
    private void OnDisable()
    {
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
        transform.Translate(Time.fixedDeltaTime * moveSpeed * dir, Space.World);
        if (inputDir.sqrMagnitude > 0)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.fixedDeltaTime * rotateSpeed);
        }
    }

    // 플레이어 점프 관련 이벤트 함수
    private void PlayerJump(InputAction.CallbackContext context)
    {
        Jump();

    }

    void Jump()
    {
        if (!IsJumping)         // 점프 중이 아닐 때만
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);  // 월드의 Up방향으로 힘을 즉시 가하기
            IsJumping = true;   // 점프중이라고 표시
        }
    }

    // 착지했을 때 처리 함수
    void OnGround()
    {
        IsJumping = false;      // 점프가 끝났다고 표시
    }

    // 플레이어 충돌 관련 이벤트 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))   // Ground와 충돌했을 때만
        {
            OnGround();     // 착지 함수 실행
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            anim.SetTrigger("IsHit");
            Heart--;
            StartCoroutine(HitAnimationState());
        }
    }

    // 히트애니메이션 코루틴
    IEnumerator HitAnimationState()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0)
        .IsName("Hit"))
        {
            // 전환 중일 때 실행되는 부분
            yield return null;
        }
        while (anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < exitTime)
        {
            // 애니메이션 재생 중 실행되는 부분
            inputActions.Player.Disable();  // 플레이어 입력 키 비활성화
            yield return null;
        }
        // 애니메이션 완료 후 실행되는 부분
        inputActions.Player.Enable();       // 플레이어 입력 키 활성화
    }

    // 플레이어 공격 관련 이벤트 함수
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        anim.SetTrigger("IsAttack"); //playerAnimator의 isAttack 트리거를 작동한다
        StartCoroutine(AttackAnimationState());
    }

    // 공격애니메이션 코루틴
    IEnumerator AttackAnimationState()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0)
        .IsName("Attack"))
        {
            // 전환 중일 때 실행되는 부분
            yield return null;
        }
        while (anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < exitTime)
        {
            // 애니메이션 재생 중 실행되는 부분
            inputActions.Player.Disable();  // 플레이어 입력 키 비활성화
            yield return null;
        }
        // 애니메이션 완료 후 실행되는 부분
        inputActions.Player.Enable();       // 플레이어 입력 키 활성화
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
            inputActions.Player.Move.Disable();
        }
        else
        {
            anim.SetBool("IsSheild", false);
            state = false;
            inputActions.Player.Move.Enable();
        }
    }

    // 플레이어 포션 관련 이벤트 함수
    private void PlayerPotion(InputAction.CallbackContext context)
    {
        anim.SetTrigger("IsPotion");
        StartCoroutine(PotionAnimationState());
    }

    // 포션애니메이션 코루틴
    IEnumerator PotionAnimationState()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0)
        .IsName("Potion"))
        {
            // 전환 중일 때 실행되는 부분
            yield return null;
        }
        while (anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < exitTime)
        {
            // 애니메이션 재생 중 실행되는 부분
            inputActions.Player.Disable();      // 플레이어 입력 키 비활성화
            yield return null;
        }
        // 애니메이션 완료 후 실행되는 부분
        inputActions.Player.Enable();           // 플레이어 입력 키 활성화
    }
}
