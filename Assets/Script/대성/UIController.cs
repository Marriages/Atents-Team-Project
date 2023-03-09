using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("UI Contents")]
    //int canvasChildCount = 0;
    TextMeshProUGUI heartText;      // UI에 표시되는 ♥ 의 텍스트상자
    public int heartNum=3;          // 생명 수를 카운트
    TextMeshProUGUI coinText;       // UI에 표시되는 코인 개수의 텍스트 상자
    int coinNum;                    // 코인 개수를 카운트


    //이미지는 어떻게 받지
    //칼 이미지 + 부울
    //방패 이미지 + 부울
    //포션 이미지 + 부울


    [Header("Component")]
    DsTestPlayer player;




    private void Awake()
    {
        player = FindObjectOfType<DsTestPlayer>();          //Start의 델리게이트 연결을 위하여 객체를 찾아둠.
        

        //canvasChildCount = transform.childCount;
        heartText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();      //heartText.TMP 찾기
        coinText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();       //coinText.TMP 찾기
        

    }
    private void Start()
    {
        heartText.text = null;
        for (int i = 0; i < heartNum; i++)
            heartText.text = heartText.text + "♥";
        coinNum = 0;

        player.HeartPlus = HeartPlusUpdate;
        player.HeartPlus = HeartMinusUpdate;
        player.CoinPlus += CoinPlusUpdate;
        player.CoinMinus += CoinMinusUpdate;
        player.PotionGet += PotionGetUpdate;
        player.PotionUse += PotionUseUpdate;
        player.WeaponGet += WeaponGetUpdate;
        player.ShieldGet += ShieldGetUpdate;
        //player.EnemyDetectPlayer += CoinMinusUpdate;

    }
    private void Update()
    {
        //coinText.text = coinContext;
    }


    private void HeartPlusUpdate(int x)
    {
        heartNum += x;
        for (int i = 0; i < heartNum; i++)
            heartText.text = heartText.text + "♥";
    }
    private void HeartMinusUpdate(int x)
    {
        heartNum -= x;
        for (int i = 0; i < heartNum; i++)
            heartText.text = heartText.text + "♥";
    }
    private void CoinPlusUpdate(int x)
    {
        
        coinNum += x;
        coinText.text = (coinNum).ToString();
    }
    private void CoinMinusUpdate(int x)
    {
        coinNum -= x;
        coinText.text = (coinNum).ToString();
    }
    private void PotionGetUpdate()
    {
    }
    private void PotionUseUpdate()
    {
    }
    private void WeaponGetUpdate()
    {
    }
    private void ShieldGetUpdate()
    {
    }
    private void BattleModeUpdate()
    {
    }  
}