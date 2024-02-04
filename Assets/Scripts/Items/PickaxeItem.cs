using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeItem : UtilityItem
{
    [SerializeField] private int stoneDamage = 0;
    public override void AnimationEvent_EnableCollider()
    {
        MeshCollider.enabled = true;
    }
    private void AnimationEvent_DisableCollider()
    {
        MeshCollider.enabled = false;
    }
}
