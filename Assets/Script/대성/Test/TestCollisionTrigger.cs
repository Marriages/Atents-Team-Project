using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollisionTrigger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{transform.gameObject.name} : {collision.gameObject.name} 와 Collision 발생");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{transform.gameObject.name} : {other.gameObject.name} 와 Trigger 발생");
    }
}
