using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//강대성 UI 테스트용 Script 코드

public class DsTestPlayerMovement : MonoBehaviour
{
    InputSystemController inputController;
    Rigidbody rigidbody;
    Vector3 dir = Vector3.zero;

    [Range(1f,10f)]
    public float speed = 5f;


    private void Awake()
    {
        inputController = new InputSystemController();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + Time.fixedDeltaTime * speed * dir);
    }


    // W(위) S(아래) A(왼쪽) D(오른쪽) , Space(점프)
    private void OnMove(InputAction.CallbackContext obj) {
        dir = obj.ReadValue<Vector3>();
    }

    private void OnEnable()
    {
        inputController.Player.Enable();
        inputController.Player.Move.performed += OnMove;
        inputController.Player.Move.canceled += OnMove;
    }
    private void OnDisable()
    {
        inputController.Player.Move.performed -= OnMove;
        inputController.Player.Move.canceled -= OnMove;
        inputController.Player.Disable();
    }
}
