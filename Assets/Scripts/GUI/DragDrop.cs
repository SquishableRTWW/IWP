using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public Vector2 ogItemPos;
    public bool isInSlot;
    public string itemType;
    public GameObject prevSlot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        isInSlot = true;
    }

    // At the start of the drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Check if it was in a slot
        if (isInSlot)
        {

        }

        //Debug.Log("Dragging started");
        // Note the original position of the item
        ogItemPos = GetComponent<RectTransform>().anchoredPosition;

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        isInSlot = false;
    }

    // During drag
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging.......");
        rectTransform.anchoredPosition += eventData.delta;
    }

    // After drag ends
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("Stopped drag");

        // Check if it landed in an item slot
        // If not revert it back to where it was
        if (!isInSlot)
        {
            rectTransform.anchoredPosition = ogItemPos;
            isInSlot = true;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Selected item");
    }
}
