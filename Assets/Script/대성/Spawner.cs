using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    GameObject enemy;
    Vector3 spawnPoint;
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
        enemy = Instantiate(enemyPrefab, transform);

    }

    private void Start()
    {
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
