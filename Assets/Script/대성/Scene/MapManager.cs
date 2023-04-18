using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    //처음 로딩씬에서 생성될 예정.
    Player player;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SearchGateway;
    }

    void SearchGateway(Scene scene,LoadSceneMode mode)
    {

    }
    

}
