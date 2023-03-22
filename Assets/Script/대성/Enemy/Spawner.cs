using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    GameObject enemy;
    EnemyBase enemyBase;
    Vector3 spawnPoint;
    


    private void Awake()
    {
        spawnPoint = transform.GetChild(0).position;
        enemy = Instantiate(enemyPrefab, transform);
        enemyBase = enemy.GetComponent<EnemyBase>();
        enemyBase.IAmDied += RespawnEnemy;

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
        yield return new WaitForSeconds(5f);
        enemy.transform.position = spawnPoint;
        enemy.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
   
}
