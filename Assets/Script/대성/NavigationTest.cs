using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{
    public GameObject target;
    public Rigidbody rigid;
    public NavMeshAgent mavAgent;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mavAgent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate()
    {
        mavAgent.SetDestination(target.transform.position);
        mavAgent.speed = 5f;
    }
}
