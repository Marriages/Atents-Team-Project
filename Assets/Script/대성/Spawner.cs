using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    GameObject enemy;
    Vector3 spawnPoint;
    Vector3[] scoutPoint;
    private bool isAlive;
    public bool IsAlive
    {
        get => isAlive;
        set
        {
            isAlive = value;
            RespawnEnemy();
        }
    }
    


    private void Awake()
    {
        spawnPoint = transform.GetChild(0).position;
        scoutPoint = new Vector3[transform.childCount];
        for(int i=1;i<transform.childCount;i++)
            scoutPoint[i] = transform.GetChild(i).position;

    }

    private void Start()
    {
        enemy = Instantiate(enemyPrefab,transform);
        enemy.transform.position = spawnPoint;
    }

    private void RespawnEnemy()
    {
        StartCoroutine(WaitAndRespawn());

    }
    IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(3f);
        enemy.transform.position = spawnPoint;
        enemy.SetActive(true);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
   
}
