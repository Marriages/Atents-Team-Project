using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string nextSceneName;
    public GameObject testObject;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(nextSceneName);
        SceneManager.sceneLoaded += CheckScene;
        SceneManager.MoveGameObjectToScene(testObject, SceneManager.GetSceneByName(nextSceneName));
    }

    private void CheckScene(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log($"Test: {arg0}, Test: {arg1}");
    }
}
