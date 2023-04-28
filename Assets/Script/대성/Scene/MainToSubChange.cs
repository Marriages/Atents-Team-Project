using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainToSubChange : MonoBehaviour
{
    Color pictureColor;
    Image picture;
    float alphaSpeed = 0.01f;
    AsyncOperation sceneoperation;
    bool alphaChangeEnd = false;

    private void Awake()
    {
        picture = transform.GetChild(0).GetComponent<Image>();
    }
    private void OnEnable()
    {
        pictureColor = picture.color;
        pictureColor.a = 0;
        picture.color = pictureColor;
        picture.gameObject.SetActive(false);
    }
    private void Start()
    {
        sceneoperation = SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);
        sceneoperation.allowSceneActivation = false;
        StartCoroutine(ChangePicrueAlpha());
    }
    IEnumerator ChangePicrueAlpha()
    {

        picture.gameObject.SetActive(true);


        while (pictureColor.a < 1)
        {
            //Debug.Log("증가중");
            pictureColor.a += alphaSpeed;
            picture.color = pictureColor;
            yield return null;
        }


        while (sceneoperation.progress < 0.9f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        alphaChangeEnd = true;

    }
    private void Update()
    {
        if (alphaChangeEnd == true)
        {
            //Debug.Log("씬전환");
            sceneoperation.allowSceneActivation = true;
        }


    }
}
