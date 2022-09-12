using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;

namespace DeckPlayer.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game Manager Configuration")]
        public Overlay overlay;
        public RectTransform cardDeck;
        public RectTransform controlPanel;

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
                controlPanel.DOAnchorPosY(100f, 0.5f).SetEase(Ease.OutExpo);
            else
                controlPanel.DOAnchorPosY(-480f, 0.4f);
        }

        public void BackButton()
        {
            overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.menu));
        }

        #region Sort-Buttons

        public void OneTwoThreeSortButton()
        {
            EnableInput(false);
        }
        public void TripleSevenSortButton()
        {
            EnableInput(false);
        }
        public void SmartSortButton()
        {
            EnableInput(false);
        }

        #endregion
    }
}



