using DeckPlayer.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeckPlayer.Managers
{
    public enum SceneType
    {
        menu = 0,
        game = 1
    }

    public class MenuManager : MonoBehaviour
    {
        public Overlay overlay;

        private void Start()
        {
            overlay.ShowOverlay(false, 0.25f);
        }

        public void DrawRandomCardsButton()
        {
            PlayerPrefs.SetInt(Constants.usingTestCase, 0);

            overlay.ShowOverlay(true, 0f, () =>
                SceneManager.LoadScene((int)SceneType.game));
        }

        public void UseSampleCaseButton()
        {
            PlayerPrefs.SetInt(Constants.usingTestCase, 1);

            overlay.ShowOverlay(true, 0f, () =>
                SceneManager.LoadScene((int)SceneType.game));
        }
    }
}
