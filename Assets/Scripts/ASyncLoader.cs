using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ASyncLoader : MonoBehaviour
{
    public static ASyncLoader Instance;
    [Header("Menu Screen")]
    [SerializeField] private GameObject loadingScreen;
    public bool IsChangeToNextScene;

    private void Awake()
    {
        Instance = this;
        IsChangeToNextScene = true;
        DontDestroyOnLoad(gameObject);
    }
    public void LoadLevel(string LevelToLoad)
    {
        Instantiate(loadingScreen);
        StartCoroutine(LoadLevelASync(LevelToLoad));
    }

    IEnumerator LoadLevelASync(string LevelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(LevelToLoad);
        yield return null;
    }
}
