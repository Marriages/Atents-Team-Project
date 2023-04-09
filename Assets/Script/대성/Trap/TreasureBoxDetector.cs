using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureBoxDetector : MonoBehaviour
{
    public Action treasurezoneEnter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player in");
            treasurezoneEnter?.Invoke();
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
}


