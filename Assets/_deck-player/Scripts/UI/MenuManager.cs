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
    public Overlay overlay;

    private void Start()
    {
        StartCoroutine(InitializeMenu());
    }

    private IEnumerator InitializeMenu()
    {
        yield return new WaitForSeconds(0.25f);
        overlay.ShowOverlay(false, 0f);
    }


    public void StartButton()
    {
        overlay.ShowOverlay(true, 0f, () => SceneManager.LoadScene((int)SceneType.game));
    }
}
