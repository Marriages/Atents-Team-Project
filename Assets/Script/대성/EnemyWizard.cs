using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizard  : EnemyBase
{
    override protected void SettingInformation()
    {
        heart = 2;
        maxHeart = 2;
        enemySpeed = 2f;
        normalSpeed = 2f;
        chaseSpeed = 3f;
        detectRange = 6f;
        atackRange = 8f;
    }
    protected override void EnemyModeAtack()
    {
    }



        /*
        //Debug.Log($"Time:{Time.time} / Interval:{intervalAtack} / Time-Inter:{Time.time - intervalAtackCurrent}");
        if (Time.time - intervalAtackCurrent > intervalAtack)
        {
            //Debug.Log("Atack");
            anim.SetTrigger("Atack");
            
            intervalAtackCurrent = Time.time;
            intervalAtackWaitCurrent = Time.time;
        }
        else if (Time.time - intervalAtackWaitCurrent > intervalAtackWait)
        {
            transform.LookAt(player.transform.position);
        }
    }*/
}
