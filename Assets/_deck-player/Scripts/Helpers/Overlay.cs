using System;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace DeckPlayer.Helpers
{
    public class Overlay : MonoBehaviour
    {
        private Image overlay;

        [Header("Overlay config")]
        public float showDuration = 0.25f;
        public float hideDuration = 0.25f;

        private void Awake()
        {
            overlay = GetComponent<Image>();
        }

        /// <summary>
        /// Show/hide overlay method
        /// </summary>
        /// <param name="show"> show or hide </param>
        /// <param name="delay"> delay </param>
        /// <param name="OnComplete"> OnComplete is called after the execution </param>
        public void ShowOverlay(bool show, float delay, Action OnComplete = null)
        {
            Sequence overlaySeq = DOTween.Sequence();
            if (show)
            {
                overlaySeq.AppendInterval(delay);
                overlaySeq.Append(overlay.DOFade(1.0f, showDuration)).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            }
            else
            {
                overlaySeq.AppendInterval(delay);
                overlaySeq.Append(overlay.DOFade(0.0f, hideDuration)).OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            }
        }
    }
}
