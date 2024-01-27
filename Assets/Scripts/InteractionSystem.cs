using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayLength = 1f;

    [Header("Raycast UI")]
    [SerializeField] private TextMeshProUGUI displayedItemName;

    private void Update()
    {
        RaycastHit hit;
        //Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rayLength))
        {
            IInteractable interactableItem = hit.transform.GetComponent<IInteractable>();
            if(interactableItem != null)
            {
                displayedItemName.text = interactableItem.ItemName;
                if(Input.GetKeyDown(KeyCode.E))
                {
                    interactableItem.Interact();
                }
            }
            else // Didn't hit an object with the Item script attached
            {
                displayedItemName.text = "";
            }
        } // Didn't hit any objects
        else
        {
            displayedItemName.text = "";
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayLength);
    }
}