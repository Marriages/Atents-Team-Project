using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapHallwayLight : MonoBehaviour
{
    FireTrapDetector detector;
    GameObject[] lightObject;
    Light[] light;
    public float intensityMax = 2f;
    public float lightOnSpeed = 2f;

    private void Awake()
    {
        detector = FindObjectOfType<FireTrapDetector>();
        lightObject = new GameObject[transform.childCount];
        light = new Light[lightObject.Length];
        Debug.Log($"자식개수 : {lightObject.Length}");

        for(int i=0;i<lightObject.Length;i++)
        {
            lightObject[i] = transform.GetChild(i).gameObject;
            light[i]=transform.GetChild(i).GetChild(0).GetComponent<Light>();

            lightObject[i].SetActive(false);
        }
    }
    private void OnEnable()
    {
        detector.playerDetect += OnLight;
        // 34번에 대한 델리게이트 없음.
    }

    //1,2번 불 켜야함.
    void OnLight(bool lightOn = true)
    {
        StartCoroutine(OnLighting(lightObject[0]));
        StartCoroutine(OnLighting(lightObject[1]));
    }
    IEnumerator OnLighting(GameObject obj)
    {
        yield return null;
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
