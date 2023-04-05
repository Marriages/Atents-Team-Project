using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(2f, Vector3.up);
    }
}
