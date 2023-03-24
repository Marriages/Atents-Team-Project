using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwardPosition : MonoBehaviour
{
    Transform trans1;
    Transform trans2;

    private void Awake()
    {
        Time.timeScale = 0.1f;
        trans1 = transform.GetChild(1);
        trans2 = transform.GetChild(2);
        
    }
    private void FixedUpdate()
    {
        Debug.Log($"칼끝 : {trans1.position} / 칼안쪽 : {trans2.position}");
    }
}
