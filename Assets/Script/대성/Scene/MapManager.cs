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
    SubChange subChange;




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

        //Debug.Log(beforeSceneIndex);
        //Debug.Log(scene.name);

        if (player==null)
        {
            //Debug.Log("Player를 찾았습니다.");
            this.player = TestPlayer.player;
            if (player==null)
            {
                Debug.Log("Player 없음!");
            }
        }
        
        titleToMain = FindObjectOfType<TitleToMain>();
        if(titleToMain != null)
        {
            titleToMain.titleToMain += () =>
            {
                //Debug.Log($"현재씬 : {SceneManager.GetActiveScene().name}");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                GameObject loadingSceneManager = FindObjectOfType<LoadingScene>(true).gameObject;
                loadingSceneManager.SetActive(true);
            };
        }

        mainToShop = FindObjectOfType<MainToShop>();
        if (mainToShop != null)
        {
            if(beforeSceneIndex==2)     //상점에서 넘어온 경우에만 실행할 수 있도록 조건 걸음.
            {
                GameObject mainChange = FindObjectOfType<MainChange>(true).gameObject;
                mainChange.SetActive(true);
            }

            mainToShop.mainToShop += () =>
            {
                //Debug.Log("Main->Shop");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                GameObject mainShopChange = FindObjectOfType<MainShopChange>(true).gameObject;
                mainShopChange.SetActive(true);
            };
        }

        shopToMain = FindObjectOfType<ShopToMain>();
        if (shopToMain != null)
        {
            GameObject shopChange = FindObjectOfType<ShopChange>(true).gameObject;
            shopChange.SetActive(true);

            shopToMain.shopToMain += () =>
            {
                Debug.Log("Shop->Main");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                GameObject shopMainChange = FindObjectOfType<ShopMainChange>(true).gameObject;
                shopMainChange.SetActive(true);
            };
        }

        mainToSub = FindObjectOfType<MainToSub>();
        if (mainToSub != null)
        {
            mainToSub.mainToSub += () =>
            {
                Debug.Log("Main->Sub");
                beforeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                GameObject mainToSubChange = FindObjectOfType<MainToSubChange>(true).gameObject;
                mainToSubChange.SetActive(true);
            };
        }

        // sub씬인 경우 재생
        subChange = FindObjectOfType<SubChange>(true);
        //Debug.Log(subChange);
        if (subChange != null)
        {
            Debug.Log("Sub Effect On");
            GameObject subChange = FindObjectOfType<SubChange>(true).gameObject;
            subChange.SetActive(true);
        }
    }
}
