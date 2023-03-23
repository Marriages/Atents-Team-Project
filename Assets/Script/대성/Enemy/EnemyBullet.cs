using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 6f;
    Transform target;
    Rigidbody rigid;

    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Destroy(gameObject,4f);
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Time.fixedDeltaTime * speed * target.transform.position);
    }
}
