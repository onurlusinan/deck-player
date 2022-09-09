using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;

namespace DeckPlayer.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Manager Configuration")]
        public Overlay overlay;
        public RectTransform cardDeck;
        public RectTransform controlPanel;

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            overlay.ShowOverlay(false, () =>
            {
                cardDeck.DOAnchorPosY(0.0f, 0.2f);
                controlPanel.DOAnchorPosY(0.0f, 0.4f).OnComplete(() =>
                                controlPanel.GetComponent<CanvasGroup>().interactable = true
                );
            });
        }

        public void BackButton()
        {
            overlay.ShowOverlay(true, () => SceneManager.LoadScene((int)SceneType.menu));
        }
    }
}

