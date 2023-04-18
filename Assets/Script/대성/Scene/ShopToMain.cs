using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopToMain : MonoBehaviour
{
    public Action shopToMain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shopToMain?.Invoke();
        }
    }
}
