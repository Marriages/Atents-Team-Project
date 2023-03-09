using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드

public class DsTestPlayer : MonoBehaviour
{
    InputSystemController inputController;
    Rigidbody rigid;
    Vector3 dir = Vector3.zero;

    [Range(1f, 10f)]
    public float speed = 5f;

    [Header("Delegate Action")]
    public Action<int> HeartPlus;
    public Action<int> HeartMinus;
    public Action<int> CoinPlus;
    public Action<int> CoinMinus;
    public Action PotionGet;
    public Action PotionUse;
    public Action WeaponGet;
    public Action ShieldGet;
    public bool weaponGet = false;
    public bool shieldGet = false;
    public bool potionGet = false;

    private int heart = 3;
    private int coin = 0;

    public int Heart
    {
        get => heart;
        set
        {

        }
    }
    public int Coin
    {
        get => coin;
        set
        {

        }
    }


    // heart 프로퍼티
    // coin 프로퍼티


    private void Awake()
    {
        inputController = new InputSystemController();
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Time.fixedDeltaTime * speed * dir);
    }

    // W(위) S(아래) A(왼쪽) D(오른쪽) , Space(점프)
    private void OnMove(InputAction.CallbackContext obj) {
        dir = obj.ReadValue<Vector3>();
        //움직임에따라 회전도 구현할 것
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Heart--;
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            Coin++;
        }
        else if (collision.gameObject.CompareTag("Heart"))
        {
            Heart++;
        }
        else if (collision.gameObject.CompareTag("Shop"))
        {
            Coin--;
        }
        else if (collision.gameObject.CompareTag("Weapon"))
        {
            if (weaponGet == true)
                WeaponGet?.Invoke();
            else
                Debug.Log("이미 무기가 있습니다.");
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            if(shieldGet==false)
                ShieldGet?.Invoke();
            else
                Debug.Log("이미 방패가 있습니다.");
        }
        else if (collision.gameObject.CompareTag("Potion"))
        {
            if (potionGet == false)
                PotionGet?.Invoke();
            else
                Debug.Log("이미 포션이 있습니다.");
        }
    }

    void die()
    {

    }

    void OnPotionKey(InputAction.CallbackContext obj)
    {
        if(potionGet==true)
            PotionUse?.Invoke();
    }

    private void OnEnable()
    {
        inputController.Player.Enable();
        inputController.TestKeyboard.Enable();
        inputController.Player.Move.performed += OnMove;
        inputController.Player.Move.canceled += OnMove;

        inputController.TestKeyboard.Test1.performed += OnTest1;
        inputController.TestKeyboard.Test2.performed += OnTest2;
        inputController.TestKeyboard.Test3.performed += OnTest3;
        inputController.TestKeyboard.Test4.performed += OnTest4;
        inputController.TestKeyboard.Test5.performed += OnTest5;
    }
    private void OnDisable()
    {
        inputController.TestKeyboard.Test1.performed -= OnTest1;
        inputController.TestKeyboard.Test2.performed -= OnTest2;
        inputController.TestKeyboard.Test3.performed -= OnTest3;
        inputController.TestKeyboard.Test4.performed -= OnTest4;
        inputController.TestKeyboard.Test5.performed -= OnTest5;

        inputController.Player.Move.performed -= OnMove;
        inputController.Player.Move.canceled -= OnMove;
        inputController.Player.Disable();
    }




    //-------------------------------------------------- T E S T --------------------------------------------------

    void OnTest1(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 1 Press");
    }
    void OnTest2(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 2 Press");
    }
    void OnTest3(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 3 Press");
        HeartPlus?.Invoke(1);
    }
    void OnTest4(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 4 Press");
        HeartMinus?.Invoke(1);
    }
    void OnTest5(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 5 Press");

    }
    //-------------------------------------------------- T E S T --------------------------------------------------
}
