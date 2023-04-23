using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestCinemachine : TestBase
{
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;

    protected override void Test1(InputAction.CallbackContext obj)
    {
        vcam1.Priority = 1;
        vcam2.Priority = 2;
    }
    protected override void Test2(InputAction.CallbackContext obj)
    {
        vcam1.Priority = 2;
        vcam2.Priority = 1;
    }
}
