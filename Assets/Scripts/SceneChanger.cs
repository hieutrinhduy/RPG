using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    private RectTransform skipButtonRect;
    private Vector3 initialPosition;
    private bool isOffScreen = false;

    private void Start()
    {
        skipButtonRect = skipButton.GetComponent<RectTransform>();
        initialPosition = skipButtonRect.anchoredPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            if (!isOffScreen)
            {
                skipButtonRect.DOAnchorPosY(Screen.height, 0.2f).SetEase(Ease.InOutQuad);
            }
            else
            {
                skipButtonRect.DOAnchorPosY(initialPosition.y, 0.2f).SetEase(Ease.InOutQuad);
            }
            isOffScreen = !isOffScreen;
        }
    }

    public void ChangeScene(string sceneName)
    {
        ASyncLoader.Instance.IsChangeToNextScene = true;
        ASyncLoader.Instance.LoadLevel(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
