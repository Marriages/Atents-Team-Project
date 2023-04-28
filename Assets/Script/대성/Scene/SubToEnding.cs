using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SubToEnding : MonoBehaviour
{
    TreasureController controller;
    float alphaSpeed=0.5f;
    Image i;
    Color c;

    private void Awake()
    {
        controller = FindObjectOfType<TreasureController>();
        i = GetComponent<Image>();
        c = i.color;
    }
    private void OnEnable()
    {
        controller.EndingCreditStart += endingCreditPanel;
        c.a = 0f;
        i.color= c;
    }

    void endingCreditPanel()
    {
        StartCoroutine(EndingCreditPanel());
    }
    IEnumerator EndingCreditPanel()
    {
        yield return new WaitForSeconds(1f);
        while(i.color.a<1f)
        {
            c.a += alphaSpeed*Time.deltaTime;
            i.color = c;
            yield return null;
        }
        SceneManager.LoadScene(4);
        
    }
}
