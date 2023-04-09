using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrap : MonoBehaviour
{
    Rigidbody rigid;
    public GameObject collisionEffect;
    public float rockMoveSpeed = 2f;
    public float rockRotateSpeed = 360f;

    private void Awake()
    {
        rigid = transform.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Vector3.left * rockMoveSpeed * Time.fixedDeltaTime);
        rigid.rotation = rigid.rotation * Quaternion.Euler(0 , 0, Time.fixedDeltaTime * rockRotateSpeed) ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            //Debug.Log("Finish");
            GameObject obj = Instantiate(collisionEffect);
            obj.transform.position = transform.position;
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Destroy");
            Player player = other.gameObject.GetComponent<Player>();
            // 플레이어가 죽을 수 있도록 조치를 취할 것
            Rigidbody rigid = other.transform.GetComponent<Rigidbody>();
            //rigid.constraints = RigidbodyConstraints.None;

            GameObject obj = Instantiate(collisionEffect);
            obj.transform.position = transform.position;
            rigid.AddForce(Vector3.left * 15f, ForceMode.Impulse);      //돌에 닿는 즉시 날려버림 


            Destroy(this.gameObject);
        }
    }
}
