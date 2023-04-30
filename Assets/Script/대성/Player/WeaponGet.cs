using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGet : MonoBehaviour
{
    Player player=null;
    bool playerEnter = false;
    GameObject pressF;

    private void Awake()
    {
        this.player = Player.player;
        pressF = transform.GetChild(2).gameObject;
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
            pressF.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnter = false;
            pressF.SetActive(false);
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
