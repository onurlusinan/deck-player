using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

public enum SceneType
{ 
    menu = 0,
    game = 1
}

public class MenuManager : MonoBehaviour
{
    [Header("Menu Config")]
    public Image overlay;

    private void Start()
    {
        StartCoroutine(InitializeMenu());
    }

    private IEnumerator InitializeMenu()
    {
        yield return new WaitForSeconds(0.25f);
        ShowOverlay(false);
    }

    public void ShowOverlay(bool show, Action OnComplete = null)
    { 
        if(show)
        {
            overlay.gameObject.SetActive(true);
            overlay.DOFade(1.0f, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        else
        {
            overlay.DOFade(0.0f, 0.2f).OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                OnComplete?.Invoke();
            });
        }
    }

    public void StartButton()
    {
        ShowOverlay(true, () => SceneManager.LoadScene((int)SceneType.game));
    }
}
