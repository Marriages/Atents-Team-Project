using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoorDetector : MonoBehaviour
{
    public Action hiddenDoorPass;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            hiddenDoorPass();
            Destroy(this);
        }
            
    }
}
