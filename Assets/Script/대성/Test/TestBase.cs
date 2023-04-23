using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    InputSystemController controller;

    private void Awake()
    {
        controller = new InputSystemController();
    }

    private void OnEnable()
    {
        controller.Test.Enable();
        controller.Test.Test1.performed += Test1;
        controller.Test.Test2.performed += Test2;
        controller.Test.Test3.performed += Test3;
        controller.Test.Test4.performed += Test4;
        controller.Test.Test5.performed += Test5;
    }
    private void OnDisable()
    {
        controller.Test.Test5.performed -= Test5;
        controller.Test.Test4.performed -= Test4;
        controller.Test.Test3.performed -= Test3;
        controller.Test.Test2.performed -= Test2;
        controller.Test.Test1.performed -= Test1;
        controller.Test.Disable();
    }


    protected virtual void Test1(InputAction.CallbackContext obj)
    {
        
    }
    protected virtual void Test2(InputAction.CallbackContext obj)
    {

    }
    protected virtual void Test3(InputAction.CallbackContext obj)
    {

    }
    protected virtual void Test4(InputAction.CallbackContext obj)
    {

    }
    protected virtual void Test5(InputAction.CallbackContext obj)
    {

    }






}
