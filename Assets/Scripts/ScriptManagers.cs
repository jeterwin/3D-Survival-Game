using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManagers : MonoBehaviour
{
    public static ScriptManagers Instance;
    public InventorySystem inventorySystem;
    public CraftingSystem craftingSystem;
    public AudioSource itemAudioSource;

    private void Awake()
    {
        Instance = this;
    }
}
