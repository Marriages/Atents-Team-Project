using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainToShop : MonoBehaviour
{
    public Action mainToShop;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainToShop?.Invoke();
        }
    }
}
