using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizard  : EnemyBase
{
    public Transform firePosition;                  // 총알이 발사될 위치. Staff 자식의 FirePosition을 넣을 것
    public GameObject bullet;                       // 발사될 총알 프리팹

    public int _heart = 1;
    public int _maxHeart = 1;
    public float _normalSpeed = 2f;
    public float _chaseSpeed = 3f;
    public float _arriveDistance = 100f;


    public float _idleWaitTimeMax = 3f;              // Idle상태->정찰상태 로 가기전 대기시간
    public float _atackWaitTimeMax = 3f;             // 공격을 하기 전까지의 대기시간
    public float _atackStayTImeMax = 1f;             // 공격을 하는 시간
    public float _getHitWaitTimeMax = 1.5f;          // 피격 후 무적시간



    override protected void SettingInformation()
    {
        heart = _heart;
        maxHeart = _maxHeart;
        normalSpeed = _normalSpeed;
        chaseSpeed = _chaseSpeed;
        arriveDistance = _arriveDistance;

        idleWaitTimeMax = _idleWaitTimeMax;              // Idle상태->정찰상태 로 가기전 대기시간
        atackWaitTimeMax = _atackWaitTimeMax;             // 공격을 하기 전까지의 대기시간
        atackStayTImeMax = _atackStayTImeMax;               // 공격을 하는 시간
        getHitWaitTimeMax = _getHitWaitTimeMax;          // 피격 후 무적시간
    }

    override protected void EnemyModeAtack()
    {
        if (Time.time - atackStayTime > atackStayTImeMax)
        {
            State = EnemyState.ATACKWAIT;
        }
    }

    
    override protected void StateAtack(EnemyState value)
    {
        base.StateAtack(value);
        StartCoroutine(AtackDelayTime());           //총알 발사시간을 제어하기 위해 코루틴 실행
    }
    IEnumerator AtackDelayTime()
    {
        yield return new WaitForSeconds(0.4f);
        GameObject obj = Instantiate(bullet);
        obj.transform.position = firePosition.position;
        obj.GetComponent<EnemyBullet>().Target = player.transform;

        
    }
}
