using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleToMain : MonoBehaviour
{
    Button button;
    public Action titleToMain;
    private void Awake()
    {
        button= GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(ChangeTitleToMain);
    }
    void ChangeTitleToMain()
    {
        titleToMain?.Invoke();
    }

}
