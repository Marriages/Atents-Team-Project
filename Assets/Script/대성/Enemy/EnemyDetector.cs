using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public Action<GameObject> detectPlayer;
    SphereCollider col;

    private void Awake()
    {
        col = transform.GetComponent<SphereCollider>();
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

    private void OnDrawGizmos()
    {
        if (col.enabled == true)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
    }
}
