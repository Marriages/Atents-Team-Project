using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenDoor : MonoBehaviour
{
    public float doorSpeed=0.5f;
    HiddenDoorOpenButton button;
    Transform openPosition;
    Transform closePosition;
    Light doorLight;
    bool activate = true;
    public float lightOnSpeed=0.5f;
    public float lightintensityMax = 2f;

    private void Awake()
    {
        button = transform.parent.GetChild(1).GetComponent<HiddenDoorOpenButton>();
        openPosition = transform.parent.GetChild(2).transform;
        closePosition = transform.parent.GetChild(3).transform;
        doorLight = transform.parent.GetChild(4).GetComponent<Light>();
        
    }
    private void OnEnable()
    {
        doorLight.intensity = 0.01f;
        button.buttonPress += OpenDoor;
    }
    void OpenDoor(bool isOpen)
    {
        StopAllCoroutines();
        if(isOpen==true)
            StartCoroutine(OpenDoorCoroutine());    
        else
            StartCoroutine(CloseDoorCoroutine());
    }
    IEnumerator OpenDoorCoroutine()
    {
        //Debug.Log("OpenDoor");
        while(transform.position.x < openPosition.position.x && activate==true)
        {
            if(doorLight.intensity<lightintensityMax)
                doorLight.intensity += lightOnSpeed * Time.deltaTime;
            transform.position = transform.position + Vector3.right * Time.deltaTime * doorSpeed;
            yield return null;
        }
        doorLight.intensity = 0f;
        activate = false;
        button.buttonPress -= OpenDoor;
        
    }

    IEnumerator CloseDoorCoroutine()
    {
        Debug.Log("CloseDoor");
        
        while (transform.position.x > closePosition.position.x && activate==true)
        {
            doorLight.intensity -= Time.deltaTime;
            transform.position = transform.position + Vector3.left * Time.deltaTime * doorSpeed;
            yield return null;
        }
        doorLight.intensity = 0f;
    }
}
