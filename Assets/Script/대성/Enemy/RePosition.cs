using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePosition : MonoBehaviour
{
    public float responRange = 20f;
    public bool positionReset;
    public bool positionRock = true;
    float x1;
    float x2;
    float z1;
    float z2;


    private void OnValidate()
    {
        if(positionRock==false)
        {
            transform.position = transform.parent.position;
            //Debug.Log($"Parent :{transform.parent.position}");
            x1 = transform.position.x - responRange * 0.5f;
            x2 = transform.position.x + responRange * 0.5f;
            z1 = transform.position.z - responRange * 0.5f;
            z2 = transform.position.z + responRange * 0.5f;
            //Debug.Log($"x1:{x1} / x2:{x2} / z1:{z1} / z2:{z2}");
            
            transform.position = new Vector3(Random.Range(x1, x2), 0,Random.Range(z1,z2));
        }
            
    }
}
