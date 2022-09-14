using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;
using DeckPlayer.CardSystem;
using System.Collections.Generic;

namespace DeckPlayer.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game Manager Configuration")]
        public Overlay overlay;
        public RectTransform cardDeck;
        public RectTransform controlPanel;

        public bool deckInput = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            overlay.ShowOverlay(false, 0.5f, () =>
                {
                    StartCoroutine(CardManager.Instance.DrawRandomCards(() =>
                        EnableInput(true)
                    ));
                }
            );
        }

        public void EnableInput(bool enable)
        {
            controlPanel.GetComponent<CanvasGroup>().interactable = enable;

            if (enable)
                controlPanel.DOAnchorPosY(100f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() =>
                            deckInput = enable
                );
            else
                controlPanel.DOAnchorPosY(-480f, 0.3f).OnComplete(() =>
                            deckInput = enable
                );
        }

        public void BackButton()
        {
            overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.menu));
        }
        public void RestartButton()
        {
            overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.game));
        }

        #region Sort-Buttons

        public void OneTwoThreeSortButton()
        {
            EnableInput(false);

            StartCoroutine(
                DeckManager.Instance.SortCardsFromList(
                    CardManager.Instance.OneTwoThreeSort()
                    )
                );
        }
        public void TripleSevenSortButton()
        {
            EnableInput(false);

            StartCoroutine(
                DeckManager.Instance.SortCardsFromList(
                    CardManager.Instance.TripleSevenSort()
                    )
                );
        }
        public void SmartSortButton()
        {
            EnableInput(false);

            //StartCoroutine(
            //    DeckManager.Instance.SortCardsFromList(
            //        CardManager.Instance.SmartSort()
            //        )
            //    );
        }

        #endregion
    }
}



