using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    public float speed = 10.0f;

    // 플레이어 회전 속도
    public float rotateSpeed = 10.0f;
    float h, v;

    // 플레이어 점프 속도
    public float jumpPower;
    private bool IsJumping;

    // 플레이어 방패
    public GameObject target;
    private bool state;

    // 입력처리용 인풋액션
    PlayerInputActions inputActions;

    // 현재 입력된 입력 방향
    Vector3 inputDir = Vector3.zero;

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
        
    }

    void Move()
    {
        //transform.Translate(Time.deltaTime * speed * inputDir);
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v); // new Vector3(h, 0, v)가 자주 쓰이게 되었으므로 dir이라는 변수에 넣고 향후 편하게 사용할 수 있게 함

        // 바라보는 방향으로 회전 후 다시 정면을 바라보는 현상을 막기 위해 설정
        if (!(h == 0 && v == 0))
        {
            // 이동과 회전을 함께 처리
            transform.position += dir * speed * Time.deltaTime;
            // 회전하는 부분. Point 1.
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
        }
    }

    // 플레이어 점프 관련 이벤트 함수
    private void PlayerJump(InputAction.CallbackContext context)
    {
        Jump();
    }
    
    void Jump()
    {
        if (!IsJumping)
        {
            IsJumping = true;
            rigid.AddForce(Vector3.up * jumpPower * 0.5f, ForceMode.Impulse);
            Debug.Log("점프");
        }
        else
        {
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsJumping = false;
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
