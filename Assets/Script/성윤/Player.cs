using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 현재 입력된 입력 방향
    public Vector3 inputDir;

    // 플레이어 이동 속도
    public float speed = 5.0f;

    // 플레이어 회전 속도
    public float rotateSpeed = 20.0f;
    
    // 플레이어 점프 속도
    public float jumpPower;
    private bool IsJumping;

    // 플레이어 방패
    public GameObject target;
    private bool state;

    // 입력처리용 인풋액션
    PlayerInputActions inputActions;

    // 플레이어 리지드바디
    private Rigidbody rigid;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += PlayerAttack;
        inputActions.Player.Shield.performed += PlayerShield;
        inputActions.Player.Potion.performed += PlayerPotion;
        inputActions.Player.Jump.performed += PlayerJump;
        inputActions.Player.Move.performed += PlayerMove;
        inputActions.Player.Move.canceled += PlayerMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.canceled -= PlayerMove;
        inputActions.Player.Move.performed -= PlayerMove;
        inputActions.Player.Jump.performed -= PlayerJump;
        inputActions.Player.Potion.performed -= PlayerPotion;
        inputActions.Player.Shield.performed -= PlayerShield;
        inputActions.Player.Attack.performed -= PlayerAttack;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        IsJumping = false;
        state = false;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Move();
    }


    // 플레이어 이동 관련 이벤트 함수
    private void PlayerMove(InputAction.CallbackContext context)
    {
        Vector3 dir = context.ReadValue<Vector3>();
        inputDir = dir;
        Debug.Log(inputDir);
    }

    void Move()
    {
        transform.Translate(Time.fixedDeltaTime * speed * inputDir, Space.World);
        transform.forward = Vector3.Lerp(transform.forward, inputDir, Time.fixedDeltaTime * rotateSpeed); ;
        //transform.rotation = Quaternion.Euler(Time.deltaTime * rotateSpeed * inputDir);

    }

    // 플레이어 점프 관련 이벤트 함수
    private void PlayerJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    
    void Jump()
    {
        if (IsJumping == false)
        {
            IsJumping = true;
            rigid.AddForce(Vector3.up * jumpPower * 0.5f, ForceMode.Impulse);
        }
        else
        {
            return;
        }
    }

    // 플레이어 충돌 관련 이벤트 함수
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsJumping = false;  // 그라운드에 닿았을 때 점프중이 아니다.
        }
    }

    // 플레이어 공격 관련 이벤트 함수
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        
    }

    // 플레이어 실드 관련 이벤트 함수
    private void PlayerShield(InputAction.CallbackContext context)
    {
        Shield();
    }

    // 실드 나오게하기/안나오게하기
    void Shield()
    {
        if(state == true)
        {
            target.SetActive(false);
            state = false;
        }
        else
        {
            target.SetActive(true);
            state = true;
        }
    }

    // 플레이어 포션 관련 이벤트 함수
    private void PlayerPotion(InputAction.CallbackContext context)
    {

    }
}
