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
    public override void AnimationEvent_EnableCollider()
    {
        MeshCollider.enabled = true;
        //Vector3 colliderSize = Vector3.one * 0.3f;
/*        Collider[] colliderArray = Physics.OverlapBox(hitArea.transform.position, hitAreaCollider.size);
        foreach(Collider collider in colliderArray)
        {
            if(collider.TryGetComponent(out ChoppableTree tree))
            {
                tree.TakeDamage(treeDamage);
                TakeDamage(SelfDamagerPerHit);
            }
        }*/
/*        if(InteractionSystem.Instance.ChoppableTree != null)
        {
            InteractionSystem.Instance.ChoppableTree.TakeDamage(treeDamage);
            TakeDamage(SelfDamagerPerHit);
        }*/
    }
    private void AnimationEvent_DisableCollider()
    {
        MeshCollider.enabled = false;
    }
}
