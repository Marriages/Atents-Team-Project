using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    public float moveSpeed = 50f;
    RectTransform endingCredit;
    GameObject pressAny;

    private void Awake()
    {
        endingCredit = transform.GetComponent<RectTransform>();
        pressAny = transform.parent.GetChild(2).gameObject;
        pressAny.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine(EndingCreditMove());
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
