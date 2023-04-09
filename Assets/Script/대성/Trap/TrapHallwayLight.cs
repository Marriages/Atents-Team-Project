using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapHallwayLight : MonoBehaviour
{
    FireTrapDetector fireTrapDetector;
    GameObject[] lightObject;
    new Light[] light;
    public float intensityMax = 2f;
    public float lightOnSpeed = 2f;
    HiddenDoorDetector hiddenDoorDetector;
    private void Awake()
    {
        hiddenDoorDetector = FindObjectOfType<HiddenDoorDetector>();
        fireTrapDetector = FindObjectOfType<FireTrapDetector>();
        lightObject = new GameObject[transform.childCount];
        light = new Light[lightObject.Length];
        //Debug.Log($"자식개수 : {lightObject.Length}");

        for(int i=0;i<lightObject.Length;i++)
        {
            lightObject[i] = transform.GetChild(i).GetChild(0).gameObject;
            light[i] = lightObject[i].transform.GetChild(0).GetComponent<Light>();

            lightObject[i].SetActive(false);
            light[i].intensity = 0;
        }
    }
    private void OnEnable()
    {
        fireTrapDetector.playerDetect += OnFirePassZoneLight;
        hiddenDoorDetector.hiddenDoorPass += OnRockPassZoneLight;
        // 34번에 대한 델리게이트 없음.
    }

    //1,2번 불 켜야함.
    void OnFirePassZoneLight(bool lightOn = true)
    {
        lightObject[0].SetActive(true);
        StartCoroutine(OnFireZoneLighting(0));
        lightObject[1].SetActive(true);
        StartCoroutine(OnFireZoneLighting(1));
    }
    IEnumerator OnFireZoneLighting(int x)
    {
        while (light[x].intensity < intensityMax)
        { 
            light[x].intensity += lightOnSpeed * Time.deltaTime;

            yield return null;
        }
    }
    void OnRockPassZoneLight()
    {
        lightObject[2].SetActive(true);
        StartCoroutine(OnRockZoneLighting(2));
        lightObject[3].SetActive(true);
        StartCoroutine(OnRockZoneLighting(3));
    }
    IEnumerator OnRockZoneLighting(int x)
    {
        while (light[x].intensity < intensityMax)
        {
            light[x].intensity += lightOnSpeed * Time.deltaTime;

            yield return null;
        }
    }

    /*
    FireTrapDetector detector;
    new Light light;
    public float intensityMax = 2f;
    public float lightOnSpeed = 2f;
    private void Awake()
    {
        detector = FindObjectOfType<FireTrapDetector>();
        light = transform.GetChild(0).GetChild(0).GetComponent<Light>();

    }
    private void OnEnable()
    {
        detector.playerDetect += OnLight;
        light.intensity = 0f;
        transform.gameObject.SetActive(false);
    }
    void OnLight(bool lightOn=true)
    {
        Debug.Log("불좀 켜줄래?");
        transform.gameObject.SetActive(true);
        StartCoroutine(OnLighting());
    }

    IEnumerator OnLighting()
    {
        while(light.intensity < intensityMax)
        {
            light.intensity += Time.deltaTime * lightOnSpeed;
            yield return null;
        }
    }*/

}
