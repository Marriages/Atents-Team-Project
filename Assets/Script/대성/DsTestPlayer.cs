using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드

public class DsTestPlayer : MonoBehaviour
{
    InputSystemController inputController;
    Rigidbody rigidbody;
    Vector3 dir = Vector3.zero;

    [Range(1f,10f)]
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


    private void Awake()
    {
        inputController = new InputSystemController();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + Time.fixedDeltaTime * speed * dir);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            int coin = 1;
            CoinPlus?.Invoke(coin);
        }

    }

    // W(위) S(아래) A(왼쪽) D(오른쪽) , Space(점프)
    private void OnMove(InputAction.CallbackContext obj) {
        dir = obj.ReadValue<Vector3>();
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
}
