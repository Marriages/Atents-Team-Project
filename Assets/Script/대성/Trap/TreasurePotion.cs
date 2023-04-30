using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePotion : MonoBehaviour
{
    TreasureController treasure;
    public GameObject player;
    public float moveSpeed=0.1f;

    private void Awake()
    {
        treasure = transform.parent.GetComponent<TreasureController>();
        player = FindObjectOfType<Player>().gameObject;
    }

    private void PotionMove()
    {
        StartCoroutine(PotionMoveStart());
    }

    IEnumerator PotionMoveStart()
    {
        Vector3 dir;
        while(true)
        {
            dir = (player.transform.position-transform.position).normalized;
            dir.y = 0;
            transform.Translate( dir * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position , player.transform.position);

    }*/
}
