using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGet : MonoBehaviour
{
    TestPlayer player=null;
    bool playerEnter = false;

    private void Awake()
    {
        player = FindObjectOfType<TestPlayer>();
    }
    private void OnEnable()
    {
        if(player.WeaponSetting==true)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") )
        {
            playerEnter = true;
            player.PlayerUseTry += CheckItemGet;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnter = false;
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
