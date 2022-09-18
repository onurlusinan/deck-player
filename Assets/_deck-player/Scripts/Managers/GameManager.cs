using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;
using System.Collections.Generic;
using DeckPlayer.Helpers;

namespace DeckPlayer.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game Manager Configuration")]
        public Overlay overlay;
        public RectTransform cardDeck;
        public RectTransform controlPanel;

        internal bool deckInput = false;

        [Header("Testing Config")]
        public List<CardData> testInputCardDatas;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(Constants.usingTestCase))
                StartGame();
            else
            {
                if (PlayerPrefs.GetInt(Constants.usingTestCase) == 0)
                    StartGame();
                else
                    TestGame();
            }
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

        private void TestGame()
        {
            overlay.ShowOverlay(false, 0.5f, () =>
                {
                    StartCoroutine(CardManager.Instance.CreateTestCards(() =>
                        EnableInput(true)
                    ));
                }
            );
        }

        public void EnableInput(bool enable)
        {
            controlPanel.GetComponent<CanvasGroup>().interactable = enable;

            if (enable)
                controlPanel.DOAnchorPosY(50f, 0.3f).SetEase(Ease.OutExpo).OnComplete(() =>
                            deckInput = enable
                );
            else
                controlPanel.DOAnchorPosY(-480f, 0.3f).OnComplete(() =>
                            deckInput = enable
                );
        }
        #region UI-Buttons
        public void BackButton()
        {
            overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.menu));
        }

        public void RestartButton()
        {
            overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.game));
        }

        public void RandomThemeButton()
        {
            CardManager.Instance.ChangeCardsTheme();
        }
        #endregion

        #region Sort-Buttons

        public void OneTwoThreeSortButton()
        {
            EnableInput(false);
            
            StartCoroutine(
                DeckManager.Instance.SortCardsFromList(
                    CardManager.Instance.oneTwoThreeSortResult
                    )
                );
        }

        public void TripleSevenSortButton()
        {
            EnableInput(false);

            StartCoroutine(
                DeckManager.Instance.SortCardsFromList(
                    CardManager.Instance.tripleSevenSortResult
                    )
                );
        }

        public void SmartSortButton()
        {
            EnableInput(false);

            StartCoroutine(
                DeckManager.Instance.SortCardsFromList(
                    CardManager.Instance.smartSortResult
                    )
                );
        }

        #endregion
    }
}



