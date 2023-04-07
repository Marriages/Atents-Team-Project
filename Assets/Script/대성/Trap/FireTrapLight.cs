using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrapLight : MonoBehaviour
{
    FireTrapDetector detector;
    private new Light light;
    public float intensityMax = 5;
    public float lightChangeSpeed = 3f;

    private void Awake()
    {
        detector = transform.parent.parent.GetChild(2).GetComponent<FireTrapDetector>();
        light=transform.GetComponent<Light>();
    }

    private void OnEnable()
    {
        detector.playerDetect += LightOnOff;
        light.enabled = false;
        light.intensity = 0f;
    }
    private void OnDisable()
    {
        detector.playerDetect -= LightOnOff;
    }

    private void LightOnOff(bool isDetect)
    {
        StopAllCoroutines();
        StartCoroutine(LightChange(isDetect));
    }
    IEnumerator LightChange(bool isDetect)
    {
        if( isDetect==true )
        {
            light.enabled = true;
            while(light.intensity< intensityMax)
            {
                light.intensity += Time.deltaTime * lightChangeSpeed;
                yield return null;
            }
        }
        else
        {
            while (light.intensity > 0)
            {
                light.intensity -= Time.deltaTime * lightChangeSpeed;
                yield return null;
            }
            light.enabled = false;
        }
    }
}
