using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManagerSingleton;

    public static int beforeSceneIndex;

    TestPlayer player;

    const int Title = 0;
    const int Main = 1;
    const int Shop = 2;
    const int Sub=3;

    TitleToMain titleToMain;
    //bool findTitleToMain = false;
    SubToMain subToMain;
    //bool findSubToMain = false;
    ShopToMain shopToMain;
    //bool findShopToMain = false;
    MainToShop mainToShop;
    //bool findMainToShop = false;
    MainToSub mainToSub;
    //bool findMainToSub = false;

    

    private void Awake()
    {
        
        if( mapManagerSingleton == null )
        {
            mapManagerSingleton = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += SearchGateway;
        }
        else
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        
    }
    private void OnEnable()
    {
        //SceneManager.sceneLoaded += SearchGateway;
    }

    void SearchGateway(Scene scene,LoadSceneMode mode)
    {

        Debug.Log(beforeSceneIndex);
        Debug.Log(scene.name);

        if (player==null)
        {
            //Debug.Log("Player를 찾았습니다.");
            player=FindObjectOfType<TestPlayer>();
            if(player==null)
            {
                Debug.Log("Player 없음!");
            }
        }
        
        titleToMain = FindObjectOfType<TitleToMain>();
        if(titleToMain != null)
        {
            titleToMain.titleToMain += () =>
            {
                Debug.Log($"현재씬 : {SceneManager.GetActiveScene().name}");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                GameObject loadingSceneManager = FindObjectOfType<LoadingScene>(true).gameObject;
                loadingSceneManager.SetActive(true);
            };
        }

        mainToShop = FindObjectOfType<MainToShop>();
        if (mainToShop != null)
        {
            mainToShop.mainToShop += () =>
            {
                Debug.Log("Main->Shop");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(Shop);
            };
        }

        shopToMain = FindObjectOfType<ShopToMain>();
        if (shopToMain != null)
        {
            shopToMain.shopToMain += () =>
            {
                Debug.Log("Shop->Main");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(Main);
            };
        }

        subToMain = FindObjectOfType<SubToMain>();
        if (subToMain != null)
        {
            subToMain.subToMain += () =>
            {
                Debug.Log("Sub->main");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(Main);
            };
        }

        mainToSub = FindObjectOfType<MainToSub>();
        if (mainToSub != null)
        {
            mainToSub.mainToSub += () =>
            {
                Debug.Log("Main->Shop");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(Sub);
            };
        }
    }
}
