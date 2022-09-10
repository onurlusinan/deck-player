using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

public class CardSlot : MonoBehaviour, IDropHandler
{
    private RectTransform slotRect;

    private void Awake()
    {
        slotRect = GetComponent<RectTransform>();        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            GameObject card = eventData.pointerDrag;
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.SetParent(slotRect, false);
            cardRect.anchoredPosition = Vector3.zero;
            cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);
        }
    }
}
