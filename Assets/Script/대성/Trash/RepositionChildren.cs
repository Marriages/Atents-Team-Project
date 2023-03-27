using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionChildren : MonoBehaviour
{

    public float responRange=10f;

    Transform[] child;
    public bool positionReset;
    private int childNum;

    private void Awake()
    {
        childNum = transform.childCount - 1;
        child = new Transform[childNum];        //scoutPosition을 제외하고 다른 scoutPosition 자식들의 Transform 정보 받아옴
        for(int i=0;i< childNum; i++)
        {
            child[i] = transform.GetChild(i+1);
        }
    }
    private void OnValidate()
    {
        for(int i=0;i<childNum;i++)
        {
            child[i].transform.position = new Vector3(Random.Range(-responRange, responRange), 0, Random.Range(-responRange, responRange));
        }
        
    }
}
