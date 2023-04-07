using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrapDetector : MonoBehaviour
{
    public Action<bool> playerDetect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerDetect?.Invoke(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetect?.Invoke(false);
        }
    }
}
