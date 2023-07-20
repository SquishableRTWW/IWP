using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string slotType;
    public GameObject itemInside;

    public void Start()
    {
        if (slotType == "Weapon" || slotType == "Equipment")
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
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
                if (eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType == "Weapon" || eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType == "Equipment")
                {
                    eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else if (slotType == "Item" && eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType != "Item" && eventData.pointerDrag.GetComponent<EquipmentBehaviour>())
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
                if (eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType == "Weapon" || eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.GetComponent<ItemSlot>().slotType == "Equipment")
                {
                    eventData.pointerDrag.GetComponent<DragDrop>().prevSlot.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            //==================================================================================================================================================================
            // If item is going into weapon or equipment slot
            else if (eventData.pointerDrag.GetComponent<DragDrop>().prevSlot != this)
            {
                if (eventData.pointerDrag.GetComponent<WeaponBehaviour>() != null && slotType == "Weapon")
                {
                    // WEAPON
                    eventData.pointerDrag.GetComponent<WeaponBehaviour>().isInInventory = false;
                    for (int i = 0; i < PrepPhaseManager.Instance.characterSelected.weaponsEquipped.Count; i++)
                    {
                        if (PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] == null)
                        {
                            //GameObject weaponToEquip = new GameObject();
                            //weaponToEquip.AddComponent<WeaponBehaviour>();
                            //weaponToEquip.GetComponent<WeaponBehaviour>().weaponScriptable = eventData.pointerDrag.GetComponent<WeaponBehaviour>().weaponScriptable;
                            //PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] = weaponToEquip;

                            foreach (GameObject item in PrepPhaseManager.Instance.itemsInGame)
                            {
                                if (item.GetComponent<WeaponBehaviour>() != null && item.GetComponent<WeaponBehaviour>().GetWeaponName() == eventData.pointerDrag.GetComponent<WeaponBehaviour>().GetWeaponName())
                                {
                                    GameObject itemPrefab = item;
                                    PrepPhaseManager.Instance.characterSelected.weaponsEquipped[i] = itemPrefab;
                                    break;
                                }
                            }
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
                    if (slotType == "Weapon" || slotType == "Equipment")
                    {
                        gameObject.transform.GetChild(0).gameObject.SetActive(false);
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
                            foreach (GameObject item in PrepPhaseManager.Instance.itemsInGame)
                            {
                                if (item.GetComponent<EquipmentBehaviour>() != null && item.GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName == eventData.pointerDrag.GetComponent<EquipmentBehaviour>().equipmentScriptable.equipmentName)
                                {
                                    GameObject itemPrefab = item;
                                    PrepPhaseManager.Instance.characterSelected.equipmentList[i] = itemPrefab;
                                    break;
                                }
                            }
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
                    if (slotType == "Weapon" || slotType == "Equipment")
                    {
                        gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }

            eventData.pointerDrag.GetComponent<DragDrop>().prevSlot = gameObject;
            itemInside = eventData.pointerDrag;
        }
    }
}
