using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : EnemyBaseTest
{
    /*
    [Header("Enemy Information")]
    public new int maxHeart = 4;
    public new float enemySpeed = 6f;
    public new float normalSpeed = 6f;
    public new float chaseSpeed = 9f;
    public new float detectRange = 5f;
    public new float atackRange = 7f;
    */

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
