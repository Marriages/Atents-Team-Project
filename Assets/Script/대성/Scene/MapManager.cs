using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    Player player;

    const int Title = 0;
    const int Main = 1;
    const int Shop = 2;
    const int Sub=3;
    TitleToMain titleToMain;
    bool findTitleToMain = false;
    SubToMain subToMain;
    bool findSubToMain = false;
    ShopToMain shopToMain;
    bool findShopToMain = false;
    MainToShop mainToShop;
    bool findMainToShop = false;
    MainToSub mainToSub;
    bool findMainToSub = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += SearchGateway;
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        //SceneManager.sceneLoaded += SearchGateway;
    }

    void SearchGateway(Scene scene,LoadSceneMode mode)
    {
        //Debug.Log("Hi. I'm map Manager");

        
        if(player==null)
        {
            //Debug.Log("Player를 찾았습니다.");
            player=FindObjectOfType<Player>();
        }

        if(findTitleToMain==false)
        {
            titleToMain = FindObjectOfType<TitleToMain>();
            if(titleToMain != null)
            {
                findTitleToMain = true;
                titleToMain.titleToMain += () => SceneManager.LoadScene(Main);
            }
        }

        if(findMainToShop==false)
        {
            mainToShop = FindObjectOfType<MainToShop>();
            if (mainToShop != null)
            {
                findMainToShop = true;
                mainToShop.mainToShop += () => SceneManager.LoadScene(Shop);
            }
        }

        if (findShopToMain == false)
        { 
            shopToMain = FindObjectOfType<ShopToMain>();
            if (shopToMain != null)
            {
                findShopToMain = true;
                shopToMain.shopToMain += () => SceneManager.LoadScene(Main);
            }
        }

        if(findSubToMain==false)
        {
            subToMain = FindObjectOfType<SubToMain>();
            if (subToMain != null)
            {
                findSubToMain = true;
                subToMain.subToMain += () => SceneManager.LoadScene(Main);
            }
        }

        if(findMainToSub==false)
        {
            mainToSub = FindObjectOfType<MainToSub>();
            if (mainToSub != null)
            {
                findMainToSub = true;
                mainToSub.mainToSub += () => SceneManager.LoadScene(Sub);
            }
        }
    }


    

}
