using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeItem : UtilityItem
{
    [SerializeField] private int treeDamage = 0;
    public int TreeDamage
    {
        get { return  treeDamage; } 
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
