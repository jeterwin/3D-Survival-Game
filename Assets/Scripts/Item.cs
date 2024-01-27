using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : IInteractable
{
    [SerializeField] private string itemName = "";
    public override string ItemName 
    {
        get { return itemName; }
    }

    public override void Interact()
    {
        //Pickup logic
        if(InventorySystem.Instance.IsFull)  { return; }  

        InventorySystem.Instance.AddToInventory(ItemName);
        Destroy(gameObject);
    }
}