using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    [SerializeField] private GameObject equippedItemGO;

    [SerializeField] private Image[] circleImages;
    [SerializeField] private TextMeshProUGUI[] circleTexts;
    //Will change the colors of the number of the equipped item to the respective colors
    [SerializeField] private Color32 textEquippedColor;
    [SerializeField] private Color32 textUnequippedColor;
    //Will change the colors of the circle of the equipped item to the respective colors
    [SerializeField] private Color32 circleEquippedColor;
    [SerializeField] private Color32 circleUnequippedColor;

    private int currentEquippedSlot = 0;
    private int lastEquippedSlot = 0;

    private void Update()
    {
        //Preferred this over n if statements
        lastEquippedSlot = currentEquippedSlot;

        for(int i = 0; i < circleImages.Length + 1; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                //If Alpha1 (1) + i is at least (1) or at maximum (5) then we indeed pressed a button
                //for our inventory slots
                if(KeyCode.Alpha1 + i <= KeyCode.Alpha5 && KeyCode.Alpha1 + i >= KeyCode.Alpha1)
                {
                    currentEquippedSlot = i;
                }
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(currentEquippedSlot >= circleImages.Length - 1)
                currentEquippedSlot = 0;
            else
                currentEquippedSlot += 1;
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(currentEquippedSlot <= 0)
                currentEquippedSlot = circleImages.Length - 1;
            else
                currentEquippedSlot -= 1;
        }

        if(lastEquippedSlot != currentEquippedSlot)
        {
            changeEquippedSlot(currentEquippedSlot);
        }
    }
    private void changeEquippedSlot(int slot)
    {
        currentEquippedSlot = slot;

        //Set the current slot colors accordingly
        circleTexts[currentEquippedSlot].color = textEquippedColor;
        circleImages[currentEquippedSlot].color = circleEquippedColor;

        //And set the last equipped slot colors accordingly (instead of running a for loop)
        circleTexts[lastEquippedSlot].color = circleUnequippedColor;
        circleTexts[lastEquippedSlot].color = textUnequippedColor;
    }

}
