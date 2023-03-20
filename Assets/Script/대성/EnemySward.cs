using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySward : EnemyBase
{
    override protected void SettingInformation()
    {
        heart = 4;
        maxHeart = 4;
        enemySpeed = 5f;
        normalSpeed = 5f;
        chaseSpeed = 7f;
        detectRange = 5f;
        atackRange = 8f;
    }

    protected override void EnemyModeAtack()
    {


    }
}
