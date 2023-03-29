using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : EnemyBase
{
    public GameObject explosionEffect;
    override protected void SettingInformation()
    {
        heart = 1;
        maxHeart = 1;
        enemySpeed = 4f;
        normalSpeed = 4f;
        chaseSpeed = 7f;
        detectRange = 7f;
    }
    protected override void EnemyModeChase()
    {
        if (agent.remainingDistance < arriveDistance)
        {
            //Debug.Log("플레이어에게 도착.");
            StopAllCoroutines();
            State = EnemyState.ATACK;
        }

    }
    override protected void StateAtack(EnemyState value)
    {
        //Debug.Log("Self Destruct Atack!!");
        anim.SetTrigger("SelfDestruct");
        agent.isStopped = true;
        StartCoroutine(OneSecondLaterBomb());
        _state = value;
    }
    IEnumerator OneSecondLaterBomb()
    {
        Debug.Log("2 second later...");
        yield return new WaitForSeconds(1.9f);
        GameObject obj = Instantiate(explosionEffect);
        obj.transform.position = transform.position+Vector3.up*2;
        enemyWeapon.enabled = true;
        yield return new WaitForSeconds(0.1f);
        State = EnemyState.GETHIT;
    }
    
    protected override void EnemyModeAtack()
    {
        //아무것도 실행하지 않음.
    }
    protected override void EnemyModeGetHit()
    {
        //아무것도 실행하지 않음.
    }
}
