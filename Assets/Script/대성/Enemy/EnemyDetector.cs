using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Action<GameObject> detectPlayer;
    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        col.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayer?.Invoke(other.gameObject);
            col.enabled= false;
        }
            
    }
}
