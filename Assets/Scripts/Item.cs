using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Item : IInteractable
{
    [SerializeField] private MaterialStruct material;
    //[SerializeField] private string itemName = "";
    public override string ItemName 
    {
        get { return material.MaterialName; }
    }

    public override void Interact()
    {
        //Pickup logic
        if(InventorySystem.Instance.IsFull)  { return; }  

        InventorySystem.Instance.AddToInventory(material);
        Destroy(gameObject);
    }
}