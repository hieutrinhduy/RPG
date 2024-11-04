using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public static Fade Instance;
    private string _currentLevel;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private float _fadeTime = 1.5f;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _respawnPoint;

    private Image _image;
    private CanvasGroup _canvasGroup;
    private CinemachineVirtualCamera _virtualCam;

    private void Awake()
    {
        Instance = this;
        _image = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _virtualCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        _currentLevel = SceneManager.GetActiveScene().name;
        //FadeInAndOut();
    }

    public void FadeInAndOut()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeRoutine(1f));
        yield return new WaitForSeconds(0.3f);
        Respawn();
        StartCoroutine(FadeRoutine(0f));
        Debug.Log("Fade in");
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startValue = _canvasGroup.alpha;

        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetAlpha, elapsedTime / _fadeTime);
            //_image.color = new Color(_image.color.r, _image.color.b, _image.color.g, newAlpha);
            _canvasGroup.alpha = newAlpha;
            yield return null;
        }

        //_image.color = new Color(_image.color.r, _image.color.b, _image.color.g, targetAlpha);
        _canvasGroup.alpha = targetAlpha;
    }

    private void Respawn()
    {
        //Transform Player = Instantiate(_playerPrefab, _respawnPoint.position, Quaternion.identity).transform;
        //_virtualCam.Follow = Player;
        SceneManager.LoadScene(_currentLevel);
    }
}
