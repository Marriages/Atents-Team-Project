using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
// Rigid가 필요할까? 고민해보기

public class EnemyBullet : MonoBehaviour
{
    public float speed = 6f;                        // 총알 속도
    Vector3 dir;                                    // 총알 방향
    Transform target;                               // 날아갈 방향 계산용
    Rigidbody rigid;                                // 움직임을 위한 rigid   -> 굳이 필요할까 생각이 들긴 함.
    public GameObject bombEffect;

    public Transform Target                         // EnemyWizard에서 타겟을 지정해주기 위함.
    {
        get { return target; }
        set { target = value; }
    }
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();          // 움직이기 위해 컴포넌트 얻어옴. 진짜 굳이 필요할까? Translate로 할까...
    }
    private void Start()
    {
        Destroy(gameObject,4f);                                     // 어쨋든 4초뒤에는 확정적으로 없어지게
        dir = (target.transform.position - transform.position);     // 목표의 벡터를 구함 ( 방향, 크기 모두 가지고 있음 )
        dir.y = 0;                                                  // 직선으로 날아가게 하기 위해 y축의 크기를 없앰
        dir = dir.normalized;                                       // 크기를 없애고 방향만 남김
    }
    private void FixedUpdate()
    {
        rigid.MovePosition(transform.position + Time.fixedDeltaTime * speed * dir );        // 해당 방향으로 일정한 시간마다 움직이게끔 설정.
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("총알 충돌! 폭발!");
        GameObject obj = Instantiate(bombEffect);                           // 어떤것이와 충돌하든 폭발 이펙트 발생
        obj.transform.position = transform.position;                        // 충돌이 일어난 해당 위치로 이펙트 위치 변경
        Destroy(obj.gameObject, 1f);                                        // 1초 뒤에 이펙트 제거
        Destroy(this.gameObject);                                           // 본인 오브젝트 즉시 제거
    }
}
