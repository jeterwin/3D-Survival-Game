using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Craftable Item")]
public class Craftable : ScriptableObject
{
    public MaterialStruct material;
    
    public List<NeededMaterials> MaterialsNeeded = new List<NeededMaterials>();

    public Sprite ItemImage;

    public bool RequiresCraftingTable = false;
}

[Serializable]
public class NeededMaterials
{
    public CraftingMaterials Material;
    public int AmountOfMaterial;
}

public enum CraftingMaterials
{
    Stone,
    Wood,
    Feather,
    String
}