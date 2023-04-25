using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGet : MonoBehaviour
{
    TestPlayer player = null;
    bool playerEnter = false;

    private void Awake()
    {
        this.player = TestPlayer.player;
    }
    private void OnEnable()
    {
        if (player.ShieldSetting == true)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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
        if (playerEnter == true)
        {
            player.ShieldSetting = true;
            player.PlayerUseTry -= CheckItemGet;
            Destroy(this.gameObject);
        }
    }
}
