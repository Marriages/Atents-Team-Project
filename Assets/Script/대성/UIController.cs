using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("UI Contents")]
    int canvasChildCount = 0;
    TextMeshProUGUI heartText;
    int heartNum;
    TextMeshProUGUI coinText;
    int coinNum;
    string coinContext;
    //이미지는 어떻게 받지
    //칼 이미지 + 부울
    //방패 이미지 + 부울
    //포션 이미지 + 부울


    [Header("Component")]
    DsTestPlayer player;




    private void Awake()
    {
        player = FindObjectOfType<DsTestPlayer>();
        

        canvasChildCount = transform.childCount;
        heartText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        coinText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        

    }
    private void Start()
    {
        heartNum = 3;
        coinNum = 0;
        player.CoinPlus += CoinPlusUpdate;

    }
    private void Update()
    {
        //coinText.text = coinContext;
    }


    private void CoinPlusUpdate(int x)
    {
        Debug.Log("Test CoinPlusUpdate");
        coinNum = coinNum + x;
        coinText.text = (coinNum).ToString();
    }
}

//private Action<int> HeartMinus;
//private Action<int> CoinPlus;
//private Action<int> CoinMinus;
//private Action PotionGet;
//private Action PotionUse;
//private Action WeaponGet;
//private Action ShieldGet;