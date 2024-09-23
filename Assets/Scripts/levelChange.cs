using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelChange : MonoBehaviour
{

    public string LevelName;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && LevelName != null)
        {
            SceneManager.LoadScene(LevelName);
            Debug.Log("LoadScene");
        }
    }
}
