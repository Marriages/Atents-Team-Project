using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGroundTest : MonoBehaviour
{
    public Material mat1;
    public Material mat2;
    int col=1;
    GameObject baseObject;
    MeshRenderer mr;
    public float scale=1;
    public int x;
    public int y;
    int cnt;


    private void Start()
    {
        for(int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {
                baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                baseObject.transform.position = new Vector3(i*scale , -0.5f, j*scale );
                baseObject.transform.localScale = new Vector3(scale, 1, scale);
                mr = baseObject.GetComponent<MeshRenderer>();

                if(col==1)
                    mr.material = mat1;
                else
                    mr.material = mat2;
                col *= -1;

                //mr.receiveShadows = false;

                cnt++;
            }
            if (x % 2 == 0)
                col *= -1;


        }

        Debug.Log($"{x} * {y} : 총 {cnt}개의 큐브 생성");

    }
}
