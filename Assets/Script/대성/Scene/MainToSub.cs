using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainToSub : MonoBehaviour
{
    public Action mainToSub;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainToSub?.Invoke();
        }
    }
}
