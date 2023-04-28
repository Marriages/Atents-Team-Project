using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingCredit : MonoBehaviour
{
    public float moveSpeed = 50f;
    RectTransform endingCredit;
    GameObject pressAny;
    Image img;
    Color c;


    private void Awake()
    {
        endingCredit = transform.GetComponent<RectTransform>();
        pressAny = transform.parent.GetChild(2).gameObject;
        pressAny.SetActive(false);

        img = transform.parent.GetChild(0).GetComponent<Image>();
        c = img.color;
    }
    private void OnEnable()
    {
        c.a = 0;
        img.color = c;
        StartCoroutine(EndingCreditMove());
        StartCoroutine(EndingCreditImage());
    }
    IEnumerator EndingCreditImage()
    {
        while(img.color.a < 0.4f)
        {
            c.a += 0.5f * Time.deltaTime;
            img.color = c;
            yield return null;
        }
    }
    IEnumerator EndingCreditMove()
    {
        while(endingCredit.anchoredPosition.y < 3000f)
        {
            endingCredit.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        pressAny.SetActive(true);
        while(Input.anyKeyDown==false)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
