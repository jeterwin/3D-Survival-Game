using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : UtilityItem
{
    [SerializeField] private BoxCollider boxCollider;
    public override void AnimationEvent_DisableCollider()
    {
        boxCollider.enabled = false;
    }
    public override void AnimationEvent_EnableCollider()
    {
        boxCollider.enabled = true;
    }
}
