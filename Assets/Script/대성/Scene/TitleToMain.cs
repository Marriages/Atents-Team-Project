using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToMain : MonoBehaviour
{
    public Action titleToMain;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            titleToMain?.Invoke();
        }
    }

}
