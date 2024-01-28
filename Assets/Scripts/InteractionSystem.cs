using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rayLength = 1f;

    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite defaultCrosshair;
    [SerializeField] private Sprite pickableCrosshair;

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
                crosshair.sprite = pickableCrosshair;
                if(Input.GetKeyDown(KeyCode.E))
                {
                    interactableItem.Interact();
                }
            }
            else // Didn't hit an object with the Item script attached
            {
                displayedItemName.text = "";
                crosshair.sprite = defaultCrosshair;
            }
        } // Didn't hit any objects
        else
        {
            displayedItemName.text = "";
            crosshair.sprite = defaultCrosshair;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayLength);
    }
}