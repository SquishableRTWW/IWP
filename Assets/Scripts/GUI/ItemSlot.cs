using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string slotType;
    public GameObject itemInside;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped in");
        // NOTE: Pointer Drag refers to the gameObject
        if (eventData.pointerDrag != null)
        {
            // Snap the item's position
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            // Set its slot boolean to true
            eventData.pointerDrag.GetComponent<DragDrop>().isInSlot = true;
            //eventData.pointerDrag.transform.SetParent(gameObject.transform);
        }
    }
}
