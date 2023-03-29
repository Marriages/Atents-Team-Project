using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    Vector3 dir;
    Transform target;
    Rigidbody rigid;
    public GameObject bombEffect;

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
        dir = (target.transform.position - transform.position);
        dir.y = 0;
        dir = dir.normalized;
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Time.fixedDeltaTime * speed * dir );
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("총알 충돌! 폭발!");
        GameObject obj = Instantiate(bombEffect);
        obj.transform.position = transform.position;
        Destroy(obj.gameObject, 1f);
        Destroy(this.gameObject);
    }
}
