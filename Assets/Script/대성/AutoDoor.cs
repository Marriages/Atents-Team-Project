using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    Animator anim;
    GameObject hinge;

    private void Awake()
    {

        hinge = transform.parent.transform.GetChild(1).gameObject;
        anim=hinge.gameObject.GetComponent<Animator>();
    }
    private void OnEnable()
    {
        hinge.transform.rotation = Quaternion.identity;   /// 회전 값 초기화.
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            anim.SetBool("IsOpen", true);

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            anim.SetBool("IsOpen", false);

    }
}
