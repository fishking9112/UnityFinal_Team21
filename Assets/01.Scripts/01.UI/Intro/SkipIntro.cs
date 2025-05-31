using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipIntro : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject skipUI;
    [SerializeField] private CanvasGroup fadeInImage;
    [SerializeField] private float fadeInDuration = 1f;

    private bool canSkip = false;

    private void Start()
    {
        fadeInImage.alpha = 1f;
        skipUI.gameObject.SetActive(false);

        Invoke(nameof(CheckAndShowSkipUI), 3f);
    }

    private void Update()
    {
        if (canSkip && (Input.GetKeyDown(KeyCode.Space) ))
        {
            SceneManager.LoadScene("AppScene");
        }
    }

    private void CheckAndShowSkipUI()
    {
        bool isFirstTime = PlayerPrefs.GetInt("HasWatchedIntro", 0) == 0;

        if (!isFirstTime)
        {
            ShowSkipUI();
        }
        else
        {
            PlayerPrefs.SetInt("HasWatchedIntro", 1);
            PlayerPrefs.Save();
        }
    }

    private void ShowSkipUI()
    {
        skipUI.gameObject.SetActive(true);
        canSkip = true;

        // Fade in (LeanTween 또는 DOTween 등 사용 가능)
        fadeInImage.alpha = 1f;
        fadeInImage.DOFade(0f, fadeInDuration);
    }
}
