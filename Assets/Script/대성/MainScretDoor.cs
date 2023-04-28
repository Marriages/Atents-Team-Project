using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainScretDoor : MonoBehaviour
{
    TestPlayer player;
    GameObject canvas;

    private void Awake()
    {
        player = TestPlayer.player;
        canvas = transform.GetChild(0).gameObject;
    }
    private void OnEnable()
    {
        if (player.ScrollSetting == true)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canvas.SetActive(true);
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
