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
    public Action PlayerDie;
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
            Debug.Log($"heart:{Heart},value:{value}");
            if ( heart<value)            //회복
            {
                
                Debug.Log("회복 시퀀스 가동");
                if( value > 3)
                    Debug.Log("이미 최대 체력입니다.");
                else
                {
                    heart = value;
                    HeartPlus?.Invoke(1);
                }
            }
            else if(heart>value)                               //피격
            {
                Debug.Log("피격 시퀀스 가동");

                if(value<=0)
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


    // heart 프로퍼티
    // coin 프로퍼티


    private void Awake()
    {
        Heart = 3;
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
            Debug.Log("적");
            Heart--;
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            Coin++;
        }
        else if (collision.gameObject.CompareTag("Heart"))
        {
            Debug.Log("하트");
            Heart++;
        }
        else if (collision.gameObject.CompareTag("Shop"))
        {
            Coin--;
        }
        else if (collision.gameObject.CompareTag("Weapon"))
        {
            if (weaponGet == false)
            {
                weaponGet = true;
                WeaponGet?.Invoke();
            }
            else
                Debug.Log("이미 무기가 있습니다.");
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            if(shieldGet==false)
            {
                shieldGet = true;
                ShieldGet?.Invoke();
            }
            else
                Debug.Log("이미 방패가 있습니다.");
        }
        else if (collision.gameObject.CompareTag("Potion"))
        {
            if (potionGet == false)
            {
                potionGet = true;
                PotionGet?.Invoke();
            }
            else
                Debug.Log("이미 포션이 있습니다.");
        }
    }

    void Die()
    {
        Debug.LogWarning("당신은 사망했습니다.");

    }

    void OnPotionUse(InputAction.CallbackContext obj)
    {
        if (potionGet == true)
        {
            Heart++;
            PotionUse?.Invoke();
        }
        else
            Debug.Log("획득한 포션이 없습니다.");
    }

    private void OnEnable()
    {
        inputController.Player.Enable();
        inputController.TestKeyboard.Enable();
        inputController.Player.Move.performed += OnMove;
        inputController.Player.Move.canceled += OnMove;
        inputController.Player.Potion.performed += OnPotionUse;

        PlayerDie += Die;

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

        inputController.Player.Potion.performed -= OnPotionUse;
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
    }
    void OnTest4(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 4 Press");
    }
    void OnTest5(InputAction.CallbackContext obj)
    {
        Debug.Log("Test 5 Press");

    }
    //-------------------------------------------------- T E S T --------------------------------------------------
}
