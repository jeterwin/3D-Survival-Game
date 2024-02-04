using System;
using UnityEngine;

public abstract class UtilityItem : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;
    public MeshCollider MeshCollider
    {
        get { return meshCollider; } 
    }

    [SerializeField] private int itemDurability = 100;

    public int ItemDurability
    {
        get { return  itemDurability; } 
    }

    [SerializeField] private int selfDamagePerHit = 0;

    [SerializeField] private int enemyDamage = 0;
    public int SelfDamagerPerHit
    {
        get { return selfDamagePerHit; } 
    }

    [SerializeField] private Animator animator;
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            AnimationEvent_SetTrigger();
        }
    }
    private void AnimationEvent_SetTrigger()
    {
        animator.SetTrigger("Used");
    }
    private void AnimationEvent_ResetTrigger()
    {
        animator.ResetTrigger("Used");
    }
    public abstract void AnimationEvent_EnableCollider();

    public void TakeDamage(int damage)
    {
        itemDurability -= damage;
        if(itemDurability <= 0)
        {
            destroyItem();
        }
    }

    private void destroyItem()
    {
        InventorySystem.Instance.RemoveItem(transform.name, 1);
    }
}
