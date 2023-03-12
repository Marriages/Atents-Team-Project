using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    public float speed = 10.0f;

    // 입력처리용 인풋액션
    PlayerInputActions inputActions;

    // 현재 입력된 입력 방향
    Vector3 inputDir = Vector3.zero;

    public GameObject shield;

    Transform shieldTransform;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        shieldTransform = transform.GetChild(0);

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
        
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * speed * inputDir);
    }

    // 플레이어 이동 관련 이벤트 함수
    private void PlayerMove(InputAction.CallbackContext context)
    {
        Vector3 dir = context.ReadValue<Vector3>();
        inputDir = dir;
    }

    // 플레이어 점프 관련 이벤트 함수
    private void PlayerJump(InputAction.CallbackContext context)
    {

    }

    // 플레이어 공격 관련 이벤트 함수
    private void PlayerAttack(InputAction.CallbackContext context)
    {

    }

    // 플레이어 실드 관련 이벤트 함수
    private void PlayerShield(InputAction.CallbackContext context)
    {
        GameObject obj = Instantiate(shield);
        obj.transform.position = shieldTransform.position;
    }

    // 플레이어 포션 관련 이벤트 함수
    private void PlayerPotion(InputAction.CallbackContext context)
    {

    }
}
