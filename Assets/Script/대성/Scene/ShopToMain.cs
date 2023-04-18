using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopToMain : MonoBehaviour
{
    public Action shopTomain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shopTomain?.Invoke();
        }
    }
}
