using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsumableItem : InventoryItem, IPointerDownHandler
{
    [Header("Consumable Item Stats")]
    [SerializeField] private int hydrationAmount = 0;
    [SerializeField] private int hungerAmount = 0;
    [SerializeField] private int healthAmount = 0;
    [SerializeField] private int warmnessAmount = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //After using the item, reduce it's quantity by 0 and give the necessary stats
            GameObject clickedObject = eventData.pointerPressRaycast.gameObject;

            string itemName = clickedObject.transform.parent.name;
            InventorySystem.Instance.RemoveItem(itemName, 1);
            HealthSystem.Instance.Regen(hungerAmount, healthAmount, hydrationAmount, warmnessAmount);
        }
    }
}
