using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // --- Is this item trashable --- //
    [SerializeField] private bool isTrashable;
 
    // --- Item Info UI --- //
    private GameObject itemInfoUI;
 
    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunctionality;
 
    [SerializeField] private string thisName;
    [SerializeField] private string thisDescription;
    [SerializeField] private string thisFunctionality;
 
    // --- Consumption --- //
    [SerializeField] private bool isConsumable;
    [SerializeField] private int hydrationAmount = 0;
    [SerializeField] private int hungerAmount = 0;
    [SerializeField] private int healthAmount = 0;
 
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Input.GetKeyDown(KeyCode.Mouse1) && isConsumable)
        {
            //After using the item, reduce it's quantity by 0 and give the necessary stats
            GameObject clickedObject = eventData.pointerPressRaycast.gameObject;
            string itemName = clickedObject.transform.parent.name.Replace("(Clone)", "");
            InventorySystem.Instance.RemoveItem(itemName, 1);
            HealthSystem.Instance.Regen(hungerAmount, healthAmount, hydrationAmount);
            //InventorySystem.Instance.RemoveItem()
        }
    }
}