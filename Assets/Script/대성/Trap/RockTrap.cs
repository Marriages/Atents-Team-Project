using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrap : MonoBehaviour
{
    Rigidbody rigid;
    public float rockMoveSpeed = 5f;

    private void Awake()
    {
        rigid = transform.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigid = collision.transform.GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.None;
        rigid.AddForce(transform.forward * 10f, ForceMode.Impulse);
    }
}
