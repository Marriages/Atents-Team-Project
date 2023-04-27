using Cinemachine;
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
            CinemachineVirtualCamera cam = transform.parent.GetChild(5).GetChild(0).GetComponent<CinemachineVirtualCamera>();
            cam.Priority = 10;
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
}


