using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    [SerializeField] private GameObject itemInfoUI;

    public GameObject ItemInfoUI
    {
        get { return itemInfoUI; }
    }

    [SerializeField] private Transform inventoryGrid;

    [field: SerializeField] private List<GameObject> slotList = new List<GameObject>();
    public List<GameObject> SlotList
    {
        get { return slotList; }
    }

    [field: SerializeField] private List<MaterialStruct> itemList = new List<MaterialStruct>();
    public List<MaterialStruct> ItemList
    {
        get { return itemList; }
    }
 
    [SerializeField] private GameObject inventoryScreenUI;
    public GameObject InventoryScreenUI
    {
        get { return inventoryScreenUI; } 
    }

    private bool isOpen;
    public bool IsOpen 
    {
        get { return isOpen; }
    }

    public bool IsFull
    {
        get { return fullInventory(); }
    }
 
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    } 
 
    void Start()
    {
        isOpen = false;

        PopulateSlotList();
    }
    public void ChangeItemIndex(string materialName,  int index)
    {
        foreach(MaterialStruct material in ItemList)
        {
            if(material.MaterialName == materialName)
            {
                itemList[itemList.IndexOf(material)].MaterialIndex = index;
                break;
            }
        }
    }
    public void RemoveItem(string itemToRemove, int amountToRemove)
    {
        //These are the materials that we've fully depleted and we want to remove after the foreach
        List<MaterialStruct> eliminatedMaterials = new();

        foreach(MaterialStruct material in itemList)
        {
            if(material.MaterialName == itemToRemove)
            {
                material.MaterialAmount -= amountToRemove;

                if(material.MaterialAmount <= 0)
                {
                    eliminatedMaterials.Add(material);
                    Destroy(slotList[material.MaterialIndex].transform.GetChild(0).gameObject);
                    //In case you deleted an item, it was surely hovered over, so disable the 
                    //item info UI
                    itemInfoUI.SetActive(false);
                }
                else
                {
                    slotList[material.MaterialIndex].GetComponentInChildren<TextMeshProUGUI>().text =
                        itemList[itemList.IndexOf(material)].MaterialAmount.ToString() + "x";
                }
                break;
            }
        }
        if(eliminatedMaterials.Count == 0) { return; }

        foreach(MaterialStruct material in eliminatedMaterials)
        {
            ItemList.Remove(material);
        }
    }
    private void PopulateSlotList()
    {
        foreach(Transform child in inventoryGrid)
        {
            slotList.Add(child.gameObject);
        }
    }
    public void AddToInventory(MaterialStruct material)
    {
        //If the item is already in the inventory increment the amount of materials by 1
        foreach (MaterialStruct item in itemList)
        {
            if (item.MaterialName == material.MaterialName)
            {
                itemList[itemList.IndexOf(item)].MaterialAmount += 1;
                slotList[item.MaterialIndex].GetComponentInChildren<TextMeshProUGUI>().text =
                    itemList[itemList.IndexOf(item)].MaterialAmount + "x";
                return;
            }
        }
        //Else equip it to the next slot
        whatSlotToEquip = findNextFreeSlot();
        if (whatSlotToEquip == null) return;

        itemToAdd = Instantiate(Resources.Load<GameObject>(material.MaterialName),
            Vector3.zero, whatSlotToEquip.transform.rotation);
        //Replace the (Clone) name given to the GO at instantiation time now rather than replace it
        //for every goddamn time that is needed
        itemToAdd.name = itemToAdd.name.Replace("(Clone)", "");
        itemToAdd.transform.SetParent(whatSlotToEquip.transform, false);

        material.MaterialIndex = slotList.IndexOf(whatSlotToEquip);
        itemList.Add(material);
    }

    private GameObject findNextFreeSlot()
    {
        foreach(GameObject slot in slotList)
        {
            //If the game object has no child objects then it has no items equipped on the slot
            if(slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return null;
    }

    private bool fullInventory()
    {
        return itemList.Count + 1 > slotList.Count;
    }

    void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        inventoryScreenUI.SetActive(false);
        isOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ScriptManagers.Instance.equipSystem.enabled = true;
        ScriptManagers.Instance.craftingSystem.enabled = true;
    }

    private void OpenInventory()
    {
        inventoryScreenUI.SetActive(true);
        isOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ScriptManagers.Instance.equipSystem.enabled = false;
        ScriptManagers.Instance.craftingSystem.enabled = false;
    }
}
[Serializable]
public class MaterialStruct
{
    public string MaterialName;
    public int MaterialAmount;
    public int MaterialIndex;
}