using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heart : MonoBehaviour
{
    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(2f, Vector3.up);
    }
}
