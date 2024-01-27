using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance;
    [SerializeField] private GameObject craftingSystemUI;

    public List<string> InventoryItemList = new List<string>();
    public Craftable[] CraftableItems;

    private bool isOpen = false;
    public bool IsOpen
    {
        get { return isOpen; } 
    }
    private void Awake()
    {
        if(Instance != null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        CraftableItems = Resources.LoadAll<Craftable>("CraftableItems");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && IsOpen)
        {
            CloseCrafting();
        }
        else if(Input.GetKeyDown(KeyCode.C) && !IsOpen)
        {
            OpenCrafting();
        }
    }
    private void craftAnyItem()
    {

    }
    public void CloseCrafting()
    {
        craftingSystemUI.SetActive(false);
        isOpen = !isOpen;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OpenCrafting()
    {
        craftingSystemUI.SetActive(true);
        isOpen = !isOpen;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
