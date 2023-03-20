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
        atackRange = 8f;
    }
    protected override void EnemyModeAtack()
    {
    }
}
