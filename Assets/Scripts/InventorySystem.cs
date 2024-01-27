using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    [SerializeField] private Transform inventoryGrid;

    [field: SerializeField] private List<GameObject> slotList = new List<GameObject>();
    [field: SerializeField] private List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
 
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

    private bool isFull;
    public bool IsFull
    {
        get { return fullInventory(); }
    }
 
 
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
        isFull = false;

        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach(Transform child in inventoryGrid)
        {
            slotList.Add(child.gameObject);

        }
    }
    public void AddToInventory(string itemName)
    {
        whatSlotToEquip = findNextFreeSlot();
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), 
            whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);
    }

    private GameObject findNextFreeSlot()
    {
        foreach(GameObject slot in slotList)
        {
            if(slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
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
    }

    private void OpenInventory()
    {
        inventoryScreenUI.SetActive(true);
        isOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}