using System;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class Overlay : MonoBehaviour
{
    private Image overlay;

    private void Awake()
    {
        overlay = GetComponent<Image>();
    }

    public void ShowOverlay(bool show, Action OnComplete = null)
    {
        if (show)
        {
            overlay.DOFade(1.0f, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        else
        {
            overlay.DOFade(0.0f, 0.2f).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
    }
}
