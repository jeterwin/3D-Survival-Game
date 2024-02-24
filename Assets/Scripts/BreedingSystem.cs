using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingSystem : MonoBehaviour
{
    [SerializeField] private InteractionSystem interactionSystem;
    private Camera mainCam;
    private float rayLength;

    private void Awake()
    {
        mainCam = interactionSystem.Camera;
        rayLength = interactionSystem.RayLength;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if(!Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, rayLength)) 
            { return; }

            BreedableAnimal animal = hit.transform.GetComponent<BreedableAnimal>();
            if(animal == null || animal.IsFed) { return; }

            int currentItemIndex = InventorySystem.Instance.CurrentEquippedSlot;
            if(InventorySystem.Instance.SlotList[currentItemIndex].transform.childCount == 0) { return; }
            
            string equippedItemName = InventorySystem.Instance.SlotList[currentItemIndex].transform.GetChild(0).name;
            if(animal.FeedableItem.ItemName == equippedItemName)
            {
                InventorySystem.Instance.RemoveItem(equippedItemName, 1);
                animal.IsFed = true;
            }
        }
    }
}
