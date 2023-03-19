using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizard  : EnemyBaseTest
{
    /*
    [Header("Enemy Information")]
    new int maxHeart = 2;
    new float enemySpeed = 4f;
    new float normalSpeed = 3f;
    new float chaseSpeed = 5f;
    new float detectRange = 5f;
    new float atackRange = 6f;
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



        //else if(Time.time - intervalAtackWaitCurrent > intervalAtackWait)
        //{

        //}
        //StartCoroutine(waitAtack);
        /*
        if (Time < 0)
        {
            anim.SetTrigger("Atack");
            Time 초기화
        }
        else
        {
            Time--;
            look
        }*/

        //이중 시간 카운트(쿨타임)을 이용해서 구현할 것
    }
}
