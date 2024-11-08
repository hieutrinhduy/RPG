using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndChapter1 : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text[] endText;
    [SerializeField] private Text toBeContinueText;
    [SerializeField] private Button homeButton;
    [SerializeField] private float fadeInDuration = 1f; // Thời gian mờ dần của mỗi endText
    [SerializeField] private Text homeBtnText;
    void Start()
    {
        // Khởi chạy Coroutine
        StartCoroutine(ShowEndSequence());

        homeButton.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Menu");
        });
    }

    private IEnumerator ShowEndSequence()
    {
        yield return new WaitForSeconds(1.5f);

        panel.SetActive(true);

        yield return new WaitForSeconds(.5f);

        foreach (var text in endText)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

            text.DOFade(1f, fadeInDuration);

            yield return new WaitForSeconds(fadeInDuration);
        }

        foreach (var text in endText)
        {
            text.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);

        toBeContinueText.color = new Color(toBeContinueText.color.r, toBeContinueText.color.g, toBeContinueText.color.b, 0);
        toBeContinueText.DOFade(1f, fadeInDuration);

        yield return new WaitForSeconds(fadeInDuration);

        yield return new WaitForSeconds(0.5f);
        homeButton.gameObject.SetActive(true);
        homeButton.image.color = new Color(homeButton.image.color.r, homeButton.image.color.g, homeButton.image.color.b, 0);
        homeButton.image.DOFade(1f, fadeInDuration);
        homeBtnText.color = new Color(homeBtnText.color.r, homeBtnText.color.g, homeBtnText.color.b, 0);
        homeBtnText.DOFade(1f, fadeInDuration);
    }
}
