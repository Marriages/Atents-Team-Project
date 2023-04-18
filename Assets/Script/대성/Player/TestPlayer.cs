using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

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
    bool weaponGet = false;
    bool shieldGet = false;
    bool potionGet = false;


    

    [Header("Input & Move")]
    Vector3 moveDir = Vector3.zero;

    [Header("Component")]
    Animator anim;
    PlayerInputActions inputActions;
    Rigidbody rigid;

    [Header("GameObject & Collider")]
    GameObject weapon;
    Collider weaponCollider;
    GameObject shield;
    Collider shieldCollider;
    GameObject potion;


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
        inputActions = GetComponent<PlayerInputActions>();
        rigid = GetComponent<Rigidbody>();
        weapon = transform.GetComponentInChildren<Weapon>().gameObject;
        shield = transform.GetComponentInChildren<Shield>().gameObject;
        potion = transform.GetComponentInChildren<Potion>().gameObject;
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
        weapon.SetActive(false);
        shield.SetActive(false);
        potion.SetActive(false);

        weaponCollider.enabled=false;
        shieldCollider.enabled = false;

        heart=maxHeart;
        coin = 0;

        isAlive = true;
        isJumping = false;
        isShilding = false;
        weaponGet = false;
        shieldGet = false;
        potionGet = false;
    }








    void InitializeConnecting()
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
    void InitializeUnConnecting()
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

    private void PlayerMove(InputAction.CallbackContext obj)
    {
        Vector2 dir = obj.ReadValue<Vector2>();
        moveDir = dir;

        anim.SetBool("IsMove", !obj.canceled);
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void PlayerShield(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void PlayerPotion(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void PlayerJump(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void PlayerUse(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }
}
