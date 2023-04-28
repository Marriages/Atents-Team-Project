using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubChange : MonoBehaviour
{
    Color pictureColor;
    Image picture;
    float alphaSpeed = 0.01f;

    private void Awake()
    {
        picture = transform.GetChild(0).GetComponent<Image>();
    }
    private void OnEnable()
    {
        pictureColor = picture.color;
        pictureColor.a = 1;
        picture.color = pictureColor;
        picture.gameObject.SetActive(true);
    }
    private void Start()
    {
        StartCoroutine(ChangePicrueAlpha());
    }
    IEnumerator ChangePicrueAlpha()
    {

        picture.gameObject.SetActive(true);
        while (pictureColor.a > 0f)
        {
            //Debug.Log("증가중");
            pictureColor.a -= alphaSpeed;
            picture.color = pictureColor;
            yield return null;
        }

    }
}
