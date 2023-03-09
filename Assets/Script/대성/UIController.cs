using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Contents")]
    //int canvasChildCount = 0;
    TextMeshProUGUI heartText;      // UI에 표시되는 ♥ 의 텍스트상자
    public int heartNum=3;          // 생명 수를 카운트
    TextMeshProUGUI coinText;       // UI에 표시되는 코인 개수의 텍스트 상자
    int coinNum;      

    [Header("무기,방패,포션 초기 활성화")]// 코인 개수를 카운트
    Image weaponImage;
    public bool weaponGet = false;
    Image shieldImage;
    public bool shieldGet = false;
    Image potionImage;
    public bool potionGet = false;
    


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
        weaponImage=transform.GetChild(4).GetComponent<Image>();
        shieldImage = transform.GetChild(5).GetComponent<Image>(); ;
        potionImage = transform.GetChild(6).GetComponent<Image>(); ;



    }
    private void Start()
    {
        heartNum = 3;
        heartText.text = null;
        for (int i = 0; i < heartNum; i++)
            heartText.text = heartText.text + "♥";
        coinNum = 0;
        

        player.HeartPlus += HeartPlusUpdate;
        player.HeartMinus += HeartMinusUpdate;
        player.CoinPlus += CoinPlusUpdate;
        player.CoinMinus += CoinMinusUpdate;
        player.PotionGet += PotionGetUpdate;
        player.PotionUse += PotionUseUpdate;
        player.WeaponGet += WeaponGetUpdate;
        player.ShieldGet += ShieldGetUpdate;
        player.PlayerDie += PlayerDieUpdate;
        //player.EnemyDetectPlayer += CoinMinusUpdate;

        //초기 셋팅. False로 설정되어있음.
        weaponImage.gameObject.SetActive(weaponGet);
        shieldImage.gameObject.SetActive(shieldGet);
        potionImage.gameObject.SetActive(potionGet);

    }



    private void HeartPlusUpdate(int x)
    {
        heartNum = heartNum + x;
        heartText.text = null;
        for (int i = 0; i < heartNum; i++)
            heartText.text = heartText.text + "♥";
    }
    private void HeartMinusUpdate(int x)
    {
        heartNum = heartNum - x;
        heartText.text = null;
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
    
    private void WeaponGetUpdate()
    {
        weaponGet = true;
        weaponImage.gameObject.SetActive(weaponGet);
    }
    private void ShieldGetUpdate()
    {
        shieldGet = true;
        shieldImage.gameObject.SetActive(shieldGet);
    }
    private void PotionGetUpdate()
    {
        potionGet = true;
        potionImage.gameObject.SetActive(potionGet);
    }
    private void PotionUseUpdate()
    {
        potionGet = false;
        potionImage.gameObject.SetActive(potionGet);
    }
    private void BattleModeUpdate()
    {
    }  
    private void PlayerDieUpdate()
    {
        heartText.text = "You Died";
    }
}