using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    public void ChangeScene(string sceneName)
    {
        ASyncLoader.Instance.IsChangeToNextScene = true;
        ASyncLoader.Instance.LoadLevel(sceneName);
        SceneManager.LoadScene(sceneName);
    }

}
