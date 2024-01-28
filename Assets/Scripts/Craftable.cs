using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Craftable Item")]
public class Craftable : ScriptableObject
{
    public Material material;
    
    public List<NeededMaterials> MaterialsNeeded = new List<NeededMaterials>();

    public Sprite ItemImage;
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
    Wood
}