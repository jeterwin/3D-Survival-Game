using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsableItem : InventoryItem
{
    [Header("Durability Stats")]
    [SerializeField] private Image durabilityBar;
    [SerializeField] private Gradient durabilityGradient;
    public Image DurabilityBar
    {
        get { return durabilityBar; } 
    }
    public Gradient DurabilityGradient
    {
        get { return  durabilityGradient; }
    }
}
