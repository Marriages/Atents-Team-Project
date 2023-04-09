using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RodkSpawner : MonoBehaviour
{
    public float rockRespawnTimeMax = 3f;
    //private float rockRespawnTime;
    public GameObject rock;
    HiddenDoorDetector detector;

    private void Awake()
    {
        detector = FindObjectOfType<HiddenDoorDetector>();
    }
    private void OnEnable()
    {
        detector.hiddenDoorPass += RockTrapStart;
    }


    private void RockTrapStart()
    {
        StartCoroutine(GenerateRock());
    }
    IEnumerator GenerateRock()
    {
        while(true)
        {
            yield return new WaitForSeconds(rockRespawnTimeMax);
            GameObject obj = Instantiate(rock);
            if (Random.Range(0f, 1f) > 0.5f)
                obj.transform.position = transform.position;
            else
                obj.transform.position = transform.position + Vector3.forward * 1.5f;

        }
        
    }
}
