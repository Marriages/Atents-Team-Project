using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject target;           // 오브젝트 타겟 설정

    
    private void Update()
    {
        transform.position = target.transform.position +
            new Vector3(0, 5.5f, -8);
    }
}
