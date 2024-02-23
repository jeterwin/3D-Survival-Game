using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeItem : UtilityItem
{
    [SerializeField] private int stoneDamage = 0;
    public int StoneDamage
    {
        get { return stoneDamage; } 
    }
    public override void AnimationEvent_DisableCollider()
    {
        MeshCollider.enabled = false;
    }
    public override void AnimationEvent_EnableCollider()
    {
        MeshCollider.enabled = true;
    }
}
