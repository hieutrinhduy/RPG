using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelChange : MonoBehaviour
{

    public string LevelName;
    public bool IsChangeToNextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && LevelName != null)
        {
            ASyncLoader.Instance.IsChangeToNextScene = IsChangeToNextScene;
            ASyncLoader.Instance.LoadLevel(LevelName);
            SceneManager.LoadScene(LevelName);
            //Debug.Log("LoadScene");
        }
    }
}
