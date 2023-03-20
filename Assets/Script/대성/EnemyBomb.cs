using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : EnemyBaseTest
{
    override protected void SettingInformation()
    {
        heart = 1;
        maxHeart = 1;
        enemySpeed = 4f;
        normalSpeed = 4f;
        chaseSpeed = 7f;
        detectRange = 7f;
        atackRange = 8f;
    }
    protected override void EnemyModeAtack()
    {
        //Debug.Log($"Time:{Time.time} / Interval:{intervalAtack} / Time-Inter:{Time.time - intervalAtackCurrent}");
        if (Time.time - intervalAtackCurrent > intervalAtack)
        {
            //Debug.Log("Atack");
            anim.SetTrigger("Atack");
            /*
             * 공격 이펙트 추가할 것
             */
            intervalAtackCurrent = Time.time;
            intervalAtackWaitCurrent = Time.time;
        }
        else
        {
            if (Time.time - intervalAtackWaitCurrent < intervalAtackWait)
            {
                // 1초동안 대기
            }
            else
            {
                transform.LookAt(player.transform.position);

            }

        }
    }
}
