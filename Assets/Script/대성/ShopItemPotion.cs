using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItemPotion : MonoBehaviour
{
    public int price = 10;
    TextMeshProUGUI priceText;
    GameObject canvas;

    private void Awake()
    {
        canvas = transform.GetChild(0).gameObject;
        priceText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        priceText.text = $"{price} Coins";
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
        if (other.gameObject.CompareTag("Player"))
        {
            canvas.SetActive(false);
        }
    }
}
