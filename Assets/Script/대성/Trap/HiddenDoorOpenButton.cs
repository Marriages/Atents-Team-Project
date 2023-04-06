using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoorOpenButton : MonoBehaviour
{
    public Action<bool> buttonPress;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player감지");
            buttonPress?.Invoke(true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player떠남");
            buttonPress?.Invoke(false);
        }
    }
}
