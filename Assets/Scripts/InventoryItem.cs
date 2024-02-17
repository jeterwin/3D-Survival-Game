using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public abstract class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- If the item can be disposed of --- //
    [SerializeField] private bool isTrashable;
 
    // --- Item Info UI --- //
    private GameObject itemInfoUI;

    [Header("Item UI")]
    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunctionality;
    [Space(5)]
        
    [Header("Item UI Info")]
    [SerializeField] private string thisName;
    [SerializeField] private string thisDescription;
    [SerializeField] private string thisFunctionality;
    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUI;
        Transform itemInfoUITransform = itemInfoUI.transform;

        itemInfoUI_itemName = itemInfoUITransform.Find("itemName").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemDescription = itemInfoUITransform.Find("itemDescription").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemFunctionality = itemInfoUITransform.Find("itemFunctionality").GetComponent<TextMeshProUGUI>();
    }
    
    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunctionality.text = thisFunctionality;
    }
 
    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }
}