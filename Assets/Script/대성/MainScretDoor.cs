using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScretDoor : MonoBehaviour
{
    TestPlayer player;
    GameObject canvas;

    private void Awake()
    {
        canvas = transform.GetChild(0).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canvas.SetActive(true);
            player = other.GetComponent<TestPlayer>();
            if(player.ScrollSetting==true)
            {
                Destroy(this.gameObject);
                Destroy(this);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canvas.SetActive(false);
        }
    }
}
