using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySward : EnemyBase
{
    // 몬스터마다 다르게 값을 부여하기 위하여 별도의 public 변수들을 SettingInformation에 대입할 용도로 선언함.
    public int _heart=3;
    public int _maxHeart = 3;
    public float _normalSpeed = 2f;
    public float _chaseSpeed = 4f;
    public float _arriveDistance = 10f;


    float _idleWaitTimeMax = 3f;              // Idle상태->정찰상태 로 가기전 대기시간
    float _atackWaitTimeMax = 1f;             // 공격을 하기 전까지의 대기시간
    float _atackStayTImeMax = 1f;             // 공격을 하는 시간
    float _getHitWaitTimeMax = 1f;          // 피격 후 무적시간


    override protected void SettingInformation()
    {
        heart=_heart;
        maxHeart = _maxHeart;
        normalSpeed = _normalSpeed;
        chaseSpeed = _chaseSpeed;
        arriveDistance = _arriveDistance;

        idleWaitTimeMax = _idleWaitTimeMax;              // Idle상태->정찰상태 로 가기전 대기시간
        atackWaitTimeMax = _atackWaitTimeMax;             // 공격을 하기 전까지의 대기시간
        atackStayTImeMax = _atackStayTImeMax;               // 공격을 하는 시간
        getHitWaitTimeMax = _getHitWaitTimeMax;          // 피격 후 무적시간
    }

    protected override void StateAtack(EnemyState value)
    {
        base.StateAtack(value);
        
        if (Random.Range(0f, 1f) > 0.5f)                            // 0.5의 확률로 Atack1 또는 Atack2 실행.
            anim.SetTrigger("Atack1");
        else
            anim.SetTrigger("Atack2");
    }
}
