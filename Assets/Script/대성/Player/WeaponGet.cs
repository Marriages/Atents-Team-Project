using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGet : MonoBehaviour
{
    bool playerEnter = false;
    TestPlayer player = null;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") )
        {
            playerEnter = true;
            player = other.gameObject.GetComponent<TestPlayer>();
            player.PlayerUseTry += CheckItemGet;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnter = false;
            player = null;
        }
    }

    private void CheckItemGet()
    {
        if(playerEnter==true)
        {
            player.WeaponSetting = true;
            player.PlayerUseTry -= CheckItemGet;
            Destroy(this.gameObject);
        }
    }
}
