using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : EnemyBase
{
    override protected void SettingInformation()
    {
        heart = 1;
        maxHeart = 1;
        enemySpeed = 4f;
        normalSpeed = 4f;
        chaseSpeed = 7f;
        detectRange = 7f;
    }
    protected override void EnemyModeAtack()
    {
        if (player != null)
        {
            transform.LookAt(player.transform);
            //너무 멀어졌으면 다시 추적
            if ((player.transform.position - transform.position).sqrMagnitude > 25.0f)
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
}
