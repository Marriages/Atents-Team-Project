using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("UI Contents")]
    //int canvasChildCount = 0;
    TextMeshProUGUI heartText;      // UI에 표시되는 ♥ 의 텍스트상자
    public int heartNum=3;          // 생명 수를 카운트
    TextMeshProUGUI coinText;       // UI에 표시되는 코인 개수의 텍스트 상자
    Image gameOverPanel;
     

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
    Player players;                  //-----------------------------------------------------------수정함. DsTestPlayer -> Player




    private void Awake()
    {
        players = Player.player;          //Start의 델리게이트 연결을 위하여 객체를 찾아둠.
                                    //-----------------------------------------------------------수정함. DsTestPlayer -> Player
        //canvasChildCount = transform.childCount;
        heartText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();      //heartText.TMP 찾기
        coinText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();       //coinText.TMP 찾기
        weaponImage=transform.GetChild(4).GetComponent<Image>();
        shieldImage = transform.GetChild(5).GetComponent<Image>();
        potionImage = transform.GetChild(6).GetComponent<Image>();
        gameOverPanel = transform.GetChild(8).GetComponent<Image>();
    }

    private void OnEnable()//-----------------------------------------------------------수정함. Start-> Enabled
    {
        if (players == null)
            players = FindObjectOfType<Player>();

        players.HeartChange += HeartUpdate;
        players.CoinChange += CoinUpdate;
        players.PotionChange += PotionUpdate;
        players.WeaponChange += WeaponGetUpdate;
        players.ShieldChange += ShieldGetUpdate;
        //player.EnemyDetectPlayer += CoinMinusUpdate;

        
    }
    private void OnDisable()//-----------------------------------------------------------추가함함. Delegate Disabled
    {
        players.HeartChange -= HeartUpdate;
        players.CoinChange -= CoinUpdate;
        players.PotionChange -= PotionUpdate;
        players.WeaponChange -= WeaponGetUpdate;
        players.ShieldChange -= ShieldGetUpdate;

    }
    private void Start()
    {
        gameOverPanel.gameObject.SetActive(false);

        heartText.text = null;
        for (int i = 0; i < players.Heart; i++)
            heartText.text = heartText.text + "♥";

        coinText.text = $"{players.Coin}";

        potionImage.gameObject.SetActive(players.PotionSetting);
        shieldImage.gameObject.SetActive(players.ShieldSetting);
        weaponImage.gameObject.SetActive(players.WeaponSetting);

    }



    private void HeartUpdate(int x)
    {
        if(x>0)
        {
            heartText.text = null;
            for (int i = 0; i < x; i++)
                heartText.text = heartText.text + "♥";
        }
        else
        {
            PlayerDieUpdate();
        }
    }
    private void CoinUpdate(int x)
    {
        coinText.text = $"{x}";
    }
    
    private void WeaponGetUpdate(bool weaponGet)
    {
        weaponImage.gameObject.SetActive(weaponGet);
    }
    private void ShieldGetUpdate(bool shieldGet)
    {
        shieldImage.gameObject.SetActive(shieldGet);
    }
    private void PotionUpdate(bool potionGet)
    {
        potionImage.gameObject.SetActive(potionGet);
    }
    private void BattleModeUpdate()
    {
    }  
    private void PlayerDieUpdate()
    {
        Debug.Log("Die Message");
        heartText.text = "";
        StartCoroutine(PrintDiePanel());
    }
    IEnumerator PrintDiePanel()
    {
        TextMeshProUGUI panelText = gameOverPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Color panelColor = gameOverPanel.color;
        Color txtColor = panelText.color;
        float a = 0.02f;
        panelColor.a = 0;
        txtColor.a = 0;
        gameOverPanel.color = panelColor;

        yield return new WaitForSeconds(2f);
        gameOverPanel.gameObject.SetActive(true);

        while (panelColor.a < 1f)
        {
            panelColor.a += a;
            txtColor.a += a;
            gameOverPanel.color = panelColor;
            panelText.color = txtColor;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        Debug.Log("재시작");
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        
    }
}