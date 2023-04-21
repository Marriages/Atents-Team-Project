using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScretDoor : MonoBehaviour
{
    TestPlayer player;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player = other.GetComponent<TestPlayer>();
            if(player.ScrollSetting==true)
            {
                Destroy(this.gameObject);
                Destroy(this);
            }
        }
    }
}
