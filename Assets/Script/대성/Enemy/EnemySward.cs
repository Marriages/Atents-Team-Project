using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySward : EnemyBase
{
    override protected void SettingInformation()
    {
        heart=2;
        maxHeart = 2;
        enemySpeed = 1f;
        normalSpeed = 1f;
        chaseSpeed = 2f;
        detectRange = 5f;
        atackRange = 8f;
    }

    protected override void EnemyModeAtackWait()
    {
        if(player!=null)
        {
            transform.LookAt(player.transform);
            //너무 멀어졌으면 다시 추적
            if ((player.transform.position - transform.position).sqrMagnitude > 4.0f)
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
    protected override void EnemyModeAtack()
    {
        if(Time.time-atackStayTime > atackStayTImeMax)
        {
            State=EnemyState.ATACKWAIT;
        }
    }
}
