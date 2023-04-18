using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubToMain : MonoBehaviour
{
    public Action subToMain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            subToMain?.Invoke();
        }
    }
}
