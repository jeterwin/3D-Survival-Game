using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractable : MonoBehaviour
{
    public abstract string ItemName { get; }
    public abstract void Interact();
}
