using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItemScrool : MonoBehaviour
{
    public int price = 10;
    int currentPlayerCoin = 0;
    TextMeshProUGUI priceText;
    GameObject canvas;
    Player player;
    static bool scroolGet=false;

    private void Awake()
    {
        canvas = transform.GetChild(0).gameObject;
        priceText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        priceText.text = $"{price} Coins";
    }
    private void Start()
    {
        if (scroolGet == true)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("플레이어 들어옴");
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            player.PlayerUseTry += TryBuyScroll;
            currentPlayerCoin = player.Coin;
            canvas.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("플레이어 나감");
        if (other.gameObject.CompareTag("Player"))
        {
            canvas.SetActive(false);
            currentPlayerCoin = 0;
            player.PlayerUseTry -= TryBuyScroll;
            player = null;
        }
    }

    void TryBuyScroll()
    {
        if (player.ScrollSetting == true)
        {
            Debug.LogWarning("이미 스크롤이 있습니다.");
        }
        else if (currentPlayerCoin >= price)
        {
            player.Coin -= price;
            player.ScrollSetting = true;

            player.PlayerUseTry -= TryBuyScroll;
            player = null;
            scroolGet = true;
            Destroy(this.gameObject);

        }
        else
        {
            Debug.LogWarning("돈이 부족합니다.");
        }
    }
}
