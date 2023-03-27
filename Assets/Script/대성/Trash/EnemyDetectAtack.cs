using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectAtack : MonoBehaviour
{
    public Action enemyDamaged;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyDamaged?.Invoke();
        }
            
    }
}
