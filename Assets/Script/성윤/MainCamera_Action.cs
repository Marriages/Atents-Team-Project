using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject Target;           // 오브젝트 타겟 설정

    public float offsetX = 0.0f;
    public float offsetY = 5.0f;
    public float offsetZ = -10.0f;

    public float CameraSpeed = 2.0f;    // 카메라 움직이는 속도
    Vector3 TargetPos;                  

    private void FixedUpdate()
    {
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ);

        transform.position = Vector3.Lerp(transform.position,
            TargetPos, Time.fixedDeltaTime * CameraSpeed);
        //이것은 테스트 코드입니다.
        
    }
}
