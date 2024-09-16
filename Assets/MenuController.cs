using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button optionButton;

    private void Start()
    {
        startButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("CutScene",LoadSceneMode.Single);
        });
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
    }
}
