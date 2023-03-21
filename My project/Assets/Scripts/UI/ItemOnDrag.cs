using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform orginalParent;
    CanvasGroup canvasGroup;

    int currentSlotId;
    int targetSlotId;

    void Start()
    {
        this.orginalParent = this.transform.parent;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Avoid ±»ÕÚµ²
        transform.SetParent(orginalParent.parent);
        // Move
        transform.position = eventData.position;

        currentSlotId = this.orginalParent.GetComponent<Slot>().slotId;

        this.canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject != null)
        {
            // Swap with other Item
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Item"))
            {
                this.targetSlotId = eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<Slot>().slotId;

                BagManager.Instance.SwapItem(currentSlotId, targetSlotId);
            }
            // Swap with empty slot
            else if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Slot"))
            {
                this.targetSlotId = eventData.pointerCurrentRaycast.gameObject.GetComponent<Slot>().slotId;

                BagManager.Instance.SwapItem(currentSlotId, targetSlotId);
            }
            else
            {
                this.transform.SetParent(this.orginalParent);
                RectTransform rt = this.transform.GetComponent<RectTransform>();
                rt.anchorMax = Vector2.one;
                rt.anchorMin = Vector2.zero;
                // max: -Right -Top
                rt.offsetMax = new Vector2(-20, -20);
                // min: Left Bottom
                rt.offsetMin = new Vector2(20, 20);
            }
        }
        else
        {
            this.transform.SetParent(this.orginalParent);
            RectTransform rt = this.transform.GetComponent<RectTransform>();
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            rt.offsetMax = new Vector2(-20, -20);
            rt.offsetMin = new Vector2(20, 20);

        }
        this.canvasGroup.blocksRaycasts=true;
    }
}
