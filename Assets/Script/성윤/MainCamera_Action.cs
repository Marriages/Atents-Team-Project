using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject target;           // 오브젝트 타겟 설정
    Vector3 dir;
    private void Start()
    {
        dir = new Vector3(0, 10f, -5);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + dir,0.3f); ;
    }
}
