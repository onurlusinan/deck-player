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

    public void ShowOverlay(bool show, float delay, Action OnComplete = null)
    {
        Sequence overlaySeq = DOTween.Sequence();
        if (show)
        {
            overlaySeq.AppendInterval(delay);
            overlaySeq.Append(overlay.DOFade(1.0f, 0.5f)).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
        else
        {
            overlaySeq.AppendInterval(delay);
            overlaySeq.Append(overlay.DOFade(0.0f, 0.5f)).OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        }
    }
}
