using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sucessfulCraft;

    [SerializeField] private Craftable[] CraftableTools;
    [SerializeField] private Craftable[] CraftableWeapons;
    [SerializeField] private Craftable[] CraftableConstructions;

    [SerializeField] private GameObject craftingSystemUI;

    [SerializeField] private Transform ConstructionsGrid;
    [SerializeField] private Transform ToolsGrid;
    [SerializeField] private Transform WeaponsGrid;

    [SerializeField] private GameObject CraftSlotPrefab;
    [SerializeField] private GameObject MaterialNamePrefab;

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
        CraftableConstructions = Resources.LoadAll<Craftable>("CraftableConstructions");
        CraftableTools = Resources.LoadAll<Craftable>("CraftableTools");
        CraftableWeapons = Resources.LoadAll<Craftable>("CraftableWeapons");
        PopulateCraftableItems(ConstructionsGrid, CraftableConstructions);
        PopulateCraftableItems(ToolsGrid, CraftableTools);
        PopulateCraftableItems(WeaponsGrid, CraftableWeapons);
    }

    private void PopulateCraftableItems(Transform itemGrid, Craftable[] itemSet)
    {
        foreach (Craftable item in itemSet)
        {
            //Create the slot prefab for the craftable item
            GameObject GO = Instantiate(CraftSlotPrefab);
            //Get the transform for the prefab for modifications
            Transform GOTransform = GO.transform;
            //Set the object's parent to the crafting grid
            GOTransform.SetParent(itemGrid, false);
            //Find the image and text and set them to the item's name and image
            GOTransform.Find("Item").Find("ItemImagePrefab").GetComponent<Image>().sprite = item.ItemImage;

            GOTransform.Find("ItemNameText").GetComponent<TextMeshProUGUI>().text = item.material.MaterialName;
            //For all materials needed to craft the item, create a new text and assign it under the
            //materials grid
            Transform MaterialsGridTransform = GOTransform.Find("MaterialsGrid");
            foreach (NeededMaterials material in item.MaterialsNeeded)
            {
                GameObject textGO = Instantiate(MaterialNamePrefab);
                textGO.transform.SetParent(MaterialsGridTransform, false);
                textGO.GetComponent<TextMeshProUGUI>().text =
                    material.AmountOfMaterial + " " + material.Material.ToString();
            }

            GOTransform.Find("CraftButton").GetComponent<Button>().onClick.AddListener(delegate 
            { CraftItem(item); });
        }
    }

    private void CraftItem(Craftable itemBlueprint)
    {
        foreach(NeededMaterials material in itemBlueprint.MaterialsNeeded)
        {
            bool hasMaterial = false;
            foreach(MaterialStruct _item in InventorySystem.Instance.ItemList)
            {
                if(_item.MaterialName == material.Material.ToString())
                {
                    hasMaterial = true;
                    if(material.AmountOfMaterial > _item.MaterialAmount)
                    {
                        Debug.Log("Not enough materials!");
                        return;
                    }
                }
            }
            if(hasMaterial == false)
            {
                Debug.Log("Some material doesn't exist in the inventory!");
                return;
            }
        }

        foreach(NeededMaterials material in itemBlueprint.MaterialsNeeded)
        {
            InventorySystem.Instance.RemoveItem(material.Material.ToString(), material.AmountOfMaterial);
        }

        //audioSource.PlayOneShot(sucessfulCraft);
        InventorySystem.Instance.AddToInventory(itemBlueprint.material);
    }
    void Update()
    {
        if(PauseScript.Instance.IsPaused) { return; }

        if (Input.GetKeyDown(KeyCode.C) && IsOpen)
        {
            CloseCrafting();
        }
        else if(Input.GetKeyDown(KeyCode.C) && !IsOpen)
        {
            OpenCrafting();
        }
    }
    public void CloseCrafting()
    {
        craftingSystemUI.SetActive(false);
        isOpen = !isOpen;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ScriptManagers.Instance.inventorySystem.enabled = true;
    }

    private void OpenCrafting()
    {
        craftingSystemUI.SetActive(true);
        isOpen = !isOpen;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ScriptManagers.Instance.inventorySystem.enabled = false;
    }
}
