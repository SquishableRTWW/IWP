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

            // If item is going into inventory==============================================================================================================================
            if (slotType == "Item" && eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType != "Item")
            {
                Debug.Log("Shit");
                Manager.Instance.playerItemList.Add(eventData.pointerDrag);
                eventData.pointerDrag.GetComponent<WeaponBehaviour>().isInInventory = true;
                foreach (GameObject weapon in PrepPhaseManager.Instance.characterSelected.weaponsEquipped)
                {
                    if (weapon.GetComponent<WeaponBehaviour>().GetWeaponName() == eventData.pointerDrag.GetComponent<WeaponBehaviour>().GetWeaponName())
                    {
                        PrepPhaseManager.Instance.characterSelected.weaponsEquipped.Remove(weapon);
                    }
                }
            }
            if (slotType == "Item" && eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType != "Item")
            {
                Manager.Instance.playerItemList.Add(eventData.pointerDrag);
                eventData.pointerDrag.GetComponent<EquipmentBehaviour>().isInInventory = true;
                foreach (GameObject equipment in PrepPhaseManager.Instance.characterSelected.equipmentList)
                {
                    if (equipment.GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName == eventData.pointerDrag.GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName)
                    {
                        PrepPhaseManager.Instance.characterSelected.equipmentList.Remove(equipment);
                    }
                }
            }
            //==================================================================================================================================================================
            // If item is going into weapon or equipment slot
            if (eventData.pointerDrag.GetComponent<WeaponBehaviour>() != null && slotType == "Weapon")
            {
                Debug.Log("Poop");
                // WEAPON
                eventData.pointerDrag.GetComponent<WeaponBehaviour>().isInInventory = false;
                PrepPhaseManager.Instance.characterSelected.weaponsEquipped.Add(eventData.pointerDrag);
                // Remove a copy of it from the inventory
                foreach (GameObject item in Manager.Instance.playerItemList)
                {
                    if (item.name == eventData.pointerDrag.name)
                    {
                        Manager.Instance.playerItemList.Remove(item);
                    }
                }
            }
            if (eventData.pointerDrag.GetComponent<EquipmentBehaviour>() != null && slotType == "Equipment")
            {
                // EQUIPMENT
                eventData.pointerDrag.GetComponent<EquipmentBehaviour>().isInInventory = false;
                PrepPhaseManager.Instance.characterSelected.equipmentList.Add(eventData.pointerDrag);
                foreach (GameObject item in Manager.Instance.playerItemList)
                {
                    if (item.name == eventData.pointerDrag.name)
                    {
                        Manager.Instance.playerItemList.Remove(item);
                    }
                }
            }

            eventData.pointerDrag.GetComponent<DragDrop>().prevSlot = gameObject;
        }
    }
}
