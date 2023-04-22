using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public string nextSceneName;


    public string[] smallTipContents = { "적의 공격을 예상하고 '막기'를 활용해보세요", 
                                  "식사는 하셨나요? 밥은 먹고 다녀야죠!",
                                  "아 우리정글 존나 못하네"};
    Slider loadingBar;
    float loadingBarvalue = 0.5f;

    Image[] randomImages;
    int imageCount=0;
    TextMeshProUGUI pressAnyKey;
    TextMeshProUGUI smallTiptext;

    bool loadingCompleteCheck = false;

    AsyncOperation operation;


    private void Awake()
    {
        //randomImages = new Image[]
        randomImages = transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
        imageCount = randomImages.Length;

        loadingBar = transform.GetChild(1).GetComponent<Slider>();

        pressAnyKey = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        smallTiptext = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        pressAnyKey.enabled = false;

        loadingBar.value = 0f;
        foreach(Image image in randomImages)
        {
            image.enabled = false;
        }
    }

    private void Start()
    {
        int randomValue = (int)Random.Range(0f, imageCount);
        randomImages[randomValue].enabled = true;

        randomValue = (int)Random.Range(0f, smallTipContents.Length);
        smallTiptext.text = smallTipContents[randomValue];


        operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false;

        StartCoroutine(LoadingStart());
        
    }
    IEnumerator LoadingStart()
    {

        yield return new WaitForSeconds(1f);

        while(loadingBar.value < 0.6f)
        {
            loadingBar.value += loadingBarvalue * Time.deltaTime;
            yield return null;
        }
        while(operation.isDone==true)
        {
            yield return null;
        }
        while(loadingBar.value<1f)
        {
            loadingBar.value += loadingBarvalue * 2f * Time.deltaTime;
            yield return null;
        }

        pressAnyKey.enabled = true;
        loadingCompleteCheck = true;

    }
    
    private void Update()
    {
        if(loadingCompleteCheck==true && Input.anyKeyDown == true)
        {
            operation.allowSceneActivation = true;
        }
    }
}
