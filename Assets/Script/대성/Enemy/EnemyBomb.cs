using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ★★★★★★★★★★수정 및 개편사항★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
// 코루틴이 아닌 Time을 이용해서 자폭할 수 있도록 수정할 것
// HP가 1이 아닌 정예 자폭병. 폭발범위가 상당하며, 피격당해도 슈퍼아머! 라는 컨셉의 몬스터 추가는 어떨까

public class EnemyBomb : EnemyBase
{
    public GameObject explosionEffect;              // 자폭 효과 프리팹

    public int _heart = 1;
    public int _maxHeart = 1;
    public float _normalSpeed = 3f;
    public float _chaseSpeed = 5f;
    public float _arriveDistance = 1.5f;


    public float _idleWaitTimeMax = 3f;              // Idle상태->정찰상태 로 가기전 대기시간
    public float _atackWaitTimeMax = 2f;             // 공격을 하기 전까지의 대기시간
    public float _atackStayTImeMax = 1f;               // 공격을 하는 시간
    public float _getHitWaitTimeMax = 1.5f;          // 피격 후 무적시간

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
    protected override void StateAtackWait(EnemyState value)
    {
        _state = value;
    }
    override protected void StateAtack(EnemyState value)
    {
        // EnemyBase의 내용을 상속받지 않고, 본인만의 공격 패턴을 가짐.
        if (debugOnOff)
            Debug.Log("Self Destruct Atack!!");

        anim.SetTrigger("SelfDestruct");                        // 자폭 애니메이션 실행
        agent.isStopped = true;                                 // agent 정지
        StartCoroutine(OneSecondLaterBomb());                   // 정해진 시간 이후 자폭할 수있게 코루틴 실행
        _state = value;
    }
    IEnumerator OneSecondLaterBomb( )
    {
        if (debugOnOff)
            Debug.Log("2 second later...");

        yield return new WaitForSeconds(1.9f);
        GameObject obj = Instantiate(explosionEffect);                      // 폭팔 프리팹 생성 ( 충돌 없음 )
        obj.transform.position = transform.position+Vector3.up*2;           // 폭팔 프리팹 위치 조정
        enemyWeapon.enabled = true;                                         // 본인의 Bomb 콜리더를 활성화 시켜 해당 범위 안에서 Enter 발생시 공격효과
        yield return new WaitForSeconds(0.1f);                              // 확실히 공격이 들어갈 수 있게 잠깐의 대기시간
        State = EnemyState.GETHIT;
        // 해당 몬스터의 HP는 1이므로 피격 후 사망.
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
