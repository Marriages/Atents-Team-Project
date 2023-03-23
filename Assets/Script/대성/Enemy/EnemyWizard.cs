using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizard  : EnemyBase
{
    public Transform firePosition;
    public GameObject bullet;
    public Action<GameObject> makeBullet;
    override protected void SettingInformation()
    {
        heart = 2;
        maxHeart = 2;
        enemySpeed = 2f;
        normalSpeed = 2f;
        chaseSpeed = 3f;
        detectRange = 7f;
        arriveDistance = 10f;
    }

    protected override void EnemyModeAtackWait()
    {
        if (player != null)
        {
            transform.LookAt(player.transform);
            //너무 멀어졌으면 다시 추적
            if ((player.transform.position - transform.position).sqrMagnitude > 100.0f)
            {
                //Debug.LogWarning("거리가 너무 멀어짐. 추적 다시 시작");
                State = EnemyState.CHASE;
            }
            else if (Time.time - atackWaitTime > atackWaitTimeMax)
            {
                //Debug.LogWarning("대기시간 종료. AtackWait로 이동");
                State = EnemyState.ATACK;
            }
        }
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
        Debug.LogWarning("Atack!!!!!! and Wait..");
        anim.SetTrigger("Atack1");
        GameObject obj = Instantiate(bullet);
        obj.transform.position = firePosition.position;
        obj.GetComponent<EnemyBullet>().Target = player.transform;

        atackStayTime = Time.time;
        _state = value;
    }
}
