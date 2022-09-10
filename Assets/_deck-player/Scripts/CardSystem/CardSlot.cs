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
            cardRect.DOAnchorPos(Vector3.zero, 0.2f);
            cardRect.DOLocalRotateQuaternion(Quaternion.identity, 0.2f);
        }
        else
        {
            GameObject prevCardSlot = eventData.pointerEnter;
            RectTransform prevCardSlotRect = prevCardSlot.GetComponent<RectTransform>();

            GameObject card = eventData.pointerDrag;
            RectTransform cardRect = card.GetComponent<RectTransform>();

            cardRect.DOAnchorPos(prevCardSlotRect.position, 0.2f);
        }
    }
}
