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
            // If there is an item inside, swap them:
            if (itemInside != null)
            {
                GameObject replacementItem = Instantiate(itemInside, gameObject.transform.position, Quaternion.identity);
                replacementItem.GetComponent<RectTransform>().anchoredPosition = eventData.pointerDrag.GetComponent<DragDrop>().ogItemPos;
                itemInside = null;
            }

            // Snap the item's position
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            // Set its slot boolean to true
            if (slotType == "Item" || slotType == eventData.pointerDrag.GetComponent<DragDrop>().itemType)
            {
                eventData.pointerDrag.GetComponent<DragDrop>().isInSlot = true;
            }

            // If item is going into inventory==============================================================================================================================
            if (slotType == "Item" && eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType != "Item" && eventData.pointerDrag.GetComponent<WeaponBehaviour>())
            {
                Debug.Log("Shit");
                Manager.Instance.playerItemList.Add(eventData.pointerDrag);
                eventData.pointerDrag.GetComponent<WeaponBehaviour>().isInInventory = true;
                for (int i = 0; i < PrepPhaseManager.Instance.characterSelected.weaponsEquipped.Count; i++)
                {
                    if (PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i].GetComponent<WeaponBehaviour>().GetWeaponName() == eventData.pointerDrag.GetComponent<WeaponBehaviour>().GetWeaponName())
                    {
                        PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] = null;
                        break;
                    }
                }
            }
            if (slotType == "Item" && eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType != "Item" && eventData.pointerDrag.GetComponent<EquipmentBehaviour>())
            {
                Manager.Instance.playerItemList.Add(eventData.pointerDrag);
                eventData.pointerDrag.GetComponent<EquipmentBehaviour>().isInInventory = true;
                for (int i = 0; i < PrepPhaseManager.Instance.characterSelected.equipmentList.Count; i++)
                {
                    if (PrepPhaseManager.Instance.characterSelected.equipmentList[i].GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName == eventData.pointerDrag.GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName)
                    {
                        PrepPhaseManager.Instance.characterSelected.equipmentList[i] = null;
                        break;
                    }
                }
            }
            //==================================================================================================================================================================
            // If item is going into weapon or equipment slot
            if (eventData.pointerDrag.GetComponent<WeaponBehaviour>() != null && slotType == "Weapon")
            {
                // WEAPON
                eventData.pointerDrag.GetComponent<WeaponBehaviour>().isInInventory = false;
                for (int i = 0; i < PrepPhaseManager.Instance.characterSelected.weaponsEquipped.Count; i++)
                {
                    if (PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] == null)
                    {
                        GameObject weaponToEquip = new GameObject();
                        weaponToEquip.AddComponent<WeaponBehaviour>();
                        weaponToEquip.GetComponent<WeaponBehaviour>().weaponScriptable = eventData.pointerDrag.GetComponent<WeaponBehaviour>().weaponScriptable;

                        PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] = weaponToEquip;
                    }
                }
                // Remove a copy of it from the inventory
                foreach (GameObject item in Manager.Instance.playerItemList)
                {
                    if (item != null && (item.name == eventData.pointerDrag.name))
                    {
                        Manager.Instance.playerItemList.Remove(item);
                        break;
                    }
                }
            }
            if (eventData.pointerDrag.GetComponent<EquipmentBehaviour>() != null && slotType == "Equipment")
            {
                // EQUIPMENT
                eventData.pointerDrag.GetComponent<EquipmentBehaviour>().isInInventory = false;
                for (int i = 0; i < PrepPhaseManager.Instance.characterSelected.equipmentList.Count; i++)
                {
                    if (PrepPhaseManager.Instance.characterSelected.equipmentList[i] == null)
                    {
                        PrepPhaseManager.Instance.characterSelected.equipmentList[i] = eventData.pointerDrag;
                    }
                }
                foreach (GameObject item in Manager.Instance.playerItemList)
                {
                    if (item != null && (item.name == eventData.pointerDrag.name))
                    {
                        Manager.Instance.playerItemList.Remove(item);
                        break;
                    }
                }
            }

            eventData.pointerDrag.GetComponent<DragDrop>().prevSlot = gameObject;
            itemInside = eventData.pointerDrag;
        }
    }
}
