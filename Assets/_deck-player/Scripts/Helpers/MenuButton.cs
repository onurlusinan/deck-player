using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;
using DeckPlayer.Audio;

/// <summary>
/// Attach this Monobehaviour to any rect to get a clicking animation + sound
/// </summary>
public class MenuButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Button Config")]
    public bool animated = true;
    public bool sound = true;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (animated)
            rect.DOScale(1f, 0.1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (animated)
            rect.DOScale(0.9f, 0.1f);

        if (sound && SoundManager.Instance)
            SoundManager.Instance.Play(Sounds.buttonClick);
    }
}
