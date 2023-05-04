using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItemPotion : MonoBehaviour
{
    public int price = 10;
    int currentPlayerCoin=0;
    TextMeshProUGUI priceText;
    GameObject canvas;
    Player player;

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
        Debug.Log("플레이어 들어옴");
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            player.PlayerUseTry += TryBuyPotion;
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
            player.PlayerUseTry -= TryBuyPotion;
            player = null;
        }
    }

    void TryBuyPotion()
    {
        if( player.PotionSetting==true)
        {
            Debug.LogWarning("이미 포션이 있습니다.");
        }
        else if(currentPlayerCoin >= price)
        {
            player.Coin -= price;
            player.PotionSetting = true;

            player.PlayerUseTry -= TryBuyPotion;
            player = null;
            Destroy(this.gameObject);

        }
        else
        {
            Debug.LogWarning("돈이 부족합니다.");
        }
    }
}
