using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainShopChange : MonoBehaviour
{
    Color color;
    Image picture;
    float alphaSpeed = 1f;

    private void Awake()
    {
        picture = transform.GetChild(0).GetComponent<Image>();
        picture.gameObject.SetActive(false);

        color = picture.GetComponent<Image>().color;
        color.a = 0;

    }
    private void OnEnable()
    {
        
        
        picture.SetActive(true);
        StartCoroutine(ChangePicrueAlpha());
    }
    IEnumerator ChangePicrueAlpha()
    {
        while(true)
        {

            yield return null;
        }

    }
}
