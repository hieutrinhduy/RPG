using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] private GameObject teleportPanel;
    [SerializeField] private Image teleporProcessAmount;
    [SerializeField] private float teleportTime = 3f;
    private float teleportedTime;
    private bool isTeleporting;


    [Header("For Teleport")]
    public string LevelName;
    public bool IsChangeToNextScene;

    void Start()
    {
        teleportedTime = 0;
        isTeleporting = false;
        teleportPanel.SetActive(false);
    }

    void Update()
    {
        if (isTeleporting)
        {
            teleportedTime += Time.deltaTime;
            teleporProcessAmount.fillAmount = teleportedTime / teleportTime;

            if (teleportedTime >= teleportTime)
            {
                Teleport();
                Debug.Log("Teleport successful!");
                teleportedTime = 0;
                isTeleporting = false;
                teleportPanel.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            teleportPanel.SetActive(true);
            isTeleporting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetTeleport();
        }
    }

    private void ResetTeleport()
    {
        teleportPanel.SetActive(false);
        teleportedTime = 0;
        teleporProcessAmount.fillAmount = 0;
        isTeleporting = false;
    }

    private void Teleport()
    {
        if(LevelName != null)
        {
            ASyncLoader.Instance.IsChangeToNextScene = IsChangeToNextScene;
            ASyncLoader.Instance.LoadLevel(LevelName);
            SceneManager.LoadScene(LevelName);
        }
    }
}
