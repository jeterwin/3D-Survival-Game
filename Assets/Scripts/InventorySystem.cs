using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class InventorySystem : MonoBehaviour
{
    #region Variables
    public static InventorySystem Instance { get; set; }

    [SerializeField] private List<GameObject> equippableItems;

    [SerializeField] private GameObject equippedItemGO;

    [SerializeField] private Transform itemHolder;


    [SerializeField] private Image[] circleImages;

    [SerializeField] private TextMeshProUGUI[] circleTexts;

    //Will change the colors of the number of the equipped item to the respective colors
    [SerializeField] private Color32 textEquippedColor;

    [SerializeField] private Color32 textUnequippedColor;
    //Will change the colors of the circle of the equipped item to the respective colors
    [SerializeField] private Color32 circleEquippedColor;

    [SerializeField] private Color32 circleUnequippedColor;

    private int currentEquippedSlot = 0;
    private int lastEquippedSlot = 0;

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

    #endregion
    void Start()
    {
        isOpen = false;

        PopulateSlotList();
    }
    void Update()
    {
        if(PauseScript.Instance.IsPaused) { return; }
        //Preferred this over n if statements
        lastEquippedSlot = currentEquippedSlot;

        for (int i = 0; i < circleImages.Length + 1; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                //If Alpha1 (1) + i is at least (1) or at maximum (5) then we indeed pressed a button
                //for our inventory slots
                if (KeyCode.Alpha1 + i <= KeyCode.Alpha5 && KeyCode.Alpha1 + i >= KeyCode.Alpha1)
                {
                    currentEquippedSlot = i;
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (currentEquippedSlot >= circleImages.Length - 1)
                currentEquippedSlot = 0;
            else
                currentEquippedSlot += 1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (currentEquippedSlot <= 0)
                currentEquippedSlot = circleImages.Length - 1;
            else
                currentEquippedSlot -= 1;
        }

        if (lastEquippedSlot != currentEquippedSlot)
        {
            changeEquippedSlot(currentEquippedSlot);
        }

        HandleInventory();
    }
    private void changeEquippedSlot(int slot)
    {
        //if(SlotList[slot].transform.childCount == 0) { return; }

        //Set the current slot colors accordingly
        circleTexts[currentEquippedSlot].color = textEquippedColor;
        circleImages[currentEquippedSlot].color = circleEquippedColor;

        //And set the last equipped slot colors accordingly (instead of running a for loop)
        circleTexts[lastEquippedSlot].color = circleUnequippedColor;
        circleTexts[lastEquippedSlot].color = textUnequippedColor;

        //If we had a previously equipped item the disable it
        if(equippedItemGO != null)
            equippedItemGO.SetActive(false);

        equippedItemGO = equippableItems[slot];

        //If we actually HAVE an item now, then enable it
        if(equippedItemGO != null)
            equippedItemGO.SetActive(true);
    }
    public void ChangeItemIndex(string materialName,  int index)
    {
        foreach(MaterialStruct material in ItemList)
        {
            if(material.MaterialName == materialName)
            {
                int oldIndex = itemList[itemList.IndexOf(material)].MaterialIndex;
                itemList[itemList.IndexOf(material)].MaterialIndex = index;

                equippableItems[index] = equippableItems[oldIndex];
                equippableItems[oldIndex] = null;

                if(oldIndex == currentEquippedSlot)
                {
                    equippedItemGO.SetActive(false);
                }
                if(index == currentEquippedSlot)
                {
                    equippedItemGO = equippableItems[index];
                    equippedItemGO.SetActive(true);
                }
                break;
            }
        }
    }
    public void InstantiateItem(int slot)
    {
        //Somehow make pooling out of all of them instead of instantiate one by one
        GameObject newItem = Instantiate(Resources.Load<GameObject>("HoldableItems/"
            + SlotList[slot].transform.GetChild(0).name.Replace("1", string.Empty)));

        UtilityItem utilityItem = newItem.GetComponent<UtilityItem>();
        if(utilityItem != null)
            utilityItem.inventoryItem = slotList[slot].GetComponentInChildren<InventoryItem>();
        //De-activate the item on instantiation, replace the annoying text name, chil it to the
        //itemHolder transform which is on the player's camera, add the item to the
        //equippable items list for easier, set the later to Items so only the secondary camera
        //can see the item (because motion blur)
        //If our current slot is the same as the picked up item, then enable it
        if(currentEquippedSlot == slot)
        {
            equippedItemGO = newItem;
        }
        newItem.SetActive(currentEquippedSlot == slot);

        newItem.name = newItem.name.Replace("(Clone)", string.Empty);

        checkForDuplicates(newItem);

        newItem.transform.SetParent(itemHolder, false);
        equippableItems[slot] = newItem;
        //newItem.layer = LayerMask.NameToLayer("Items");
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
            if(material.MaterialIndex <= 4)
            {
                UninstantiateItem(material.MaterialIndex);
            }
            ItemList.Remove(material);
        }
    }
    public void UninstantiateItem(int slot)
    {
        Destroy(equippableItems[slot]);
        equippableItems[slot] = null;
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
        MaterialStruct materialCopy = new();
        materialCopy.MaterialIndex = material.MaterialIndex;
        materialCopy.MaterialName = material.MaterialName;
        materialCopy.MaterialAmount = material.MaterialAmount;
        materialCopy.IsStackable = material.IsStackable;

        if (materialCopy.IsStackable)
        {
            //If the item is already in the inventory increment the amount of materials by 1
            foreach (MaterialStruct item in itemList)
            {
                if (item.MaterialName == materialCopy.MaterialName)
                {
                    itemList[itemList.IndexOf(item)].MaterialAmount += 1;
                    slotList[item.MaterialIndex].GetComponentInChildren<TextMeshProUGUI>().text =
                        itemList[itemList.IndexOf(item)].MaterialAmount + "x";
                    return;
                }
            }
        }
        //Else equip it to the next slot
        whatSlotToEquip = findNextFreeSlot();
        if (whatSlotToEquip == null) return;

        itemToAdd = Instantiate(Resources.Load<GameObject>(materialCopy.MaterialName),
            Vector3.zero, whatSlotToEquip.transform.rotation);
        //Replace the (Clone) name given to the GO at instantiation time now rather than replace it
        //for every goddamn time that is needed
        itemToAdd.name = itemToAdd.name.Replace("(Clone)", string.Empty);

        if(checkForDuplicates(itemToAdd))
        {
            materialCopy.MaterialName += "1";
        }

        itemToAdd.transform.SetParent(whatSlotToEquip.transform, false);

        int itemSlotIndex = slotList.IndexOf(whatSlotToEquip);
        materialCopy.MaterialIndex = itemSlotIndex;
        //No. of quick slots starting from 0
        if (itemSlotIndex <= 4)
        {
            InstantiateItem(itemSlotIndex);
        }
        itemList.Add(materialCopy);
    }

    private bool checkForDuplicates(GameObject itemToAdd)
    {
        //Check if an item already with the same name already exists, for example
        //Stone Axe, Stone Axe -> Stone Axe, Stone Axe1
        foreach (Transform child in itemHolder.transform)
        {
            if (child.name == itemToAdd.name)
            {
                itemToAdd.name += "1";
                return true;
            }
        }
        return false;
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

    private void HandleInventory()
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
        ScriptManagers.Instance.craftingSystem.enabled = true;
    }

    private void OpenInventory()
    {
        inventoryScreenUI.SetActive(true);
        isOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ScriptManagers.Instance.craftingSystem.enabled = false;
    }
}
[Serializable]
public class MaterialStruct
{
    public string MaterialName;
    public int MaterialAmount;
    public int MaterialIndex;
    public bool IsStackable;
}