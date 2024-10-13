using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;
    public GameObject Player;
    private void Start()
    {
        if (ASyncLoader.Instance.IsChangeToNextScene)
        {
            Player.transform.position = SpawnPoint1.position;
        }
        else
        {
            Player.transform.position = SpawnPoint2.position;
        }
    }
}
