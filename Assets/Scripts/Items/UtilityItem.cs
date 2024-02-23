using System;
using UnityEngine;

public abstract class UtilityItem : MonoBehaviour
{
    public UsableItem item;

    [SerializeField] private AudioClip swingSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip destroyedItemSFX;

    [SerializeField] private GameObject particlesHitSFX;

    public GameObject ParticlesHitSFX
    {
        get { return particlesHitSFX; } 
    } 

    [SerializeField] private MeshCollider meshCollider;

    private const string triggerName = "Used";

    private AudioSource audioSource;
    public MeshCollider MeshCollider
    {
        get { return meshCollider; } 
    }

    [SerializeField] private float maxItemDurability = 100f;

    [SerializeField] private int itemDurability = 100;

    public int ItemDurability
    {
        get { return  itemDurability; } 
    }

    [SerializeField] private int selfDamagePerHit = 0;
    public int SelfDamagerPerHit
    {
        get { return selfDamagePerHit; } 
    }

    [SerializeField] private int enemyDamage = 0;

    public int EnemyDamage
    {
        get { return enemyDamage; }
    }

    [SerializeField] private Animator animator;

    private void Start()
    {
        audioSource = ScriptManagers.Instance.itemAudioSource;
    }

    private void Update()
    {
        if(InventorySystem.Instance.IsOpen || CraftingSystem.Instance.IsOpen) { return; }

        if(Input.GetKeyDown(KeyCode.Mouse0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
        {
            SetBool();

            audioSource.PlayOneShot(swingSFX);
        }
    }
    private void SetBool()
    {
        animator.SetBool(triggerName, true);
        InventorySystem.Instance.IsUsingWeapon = true;
    }
    public void AnimationEvent_ResetTrigger()
    {
        animator.SetBool(triggerName, false);
        InventorySystem.Instance.IsUsingWeapon = false;
    }
    public abstract void AnimationEvent_EnableCollider();
    public abstract void AnimationEvent_DisableCollider();
    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(hitSFX);
        itemDurability -= damage;
        float durabilityPercentage = itemDurability / maxItemDurability;
        item.DurabilityBar.fillAmount = durabilityPercentage;
        item.DurabilityBar.color = item.DurabilityGradient.Evaluate(durabilityPercentage);

        if(itemDurability <= 0)
        {
            audioSource.PlayOneShot(destroyedItemSFX);
            InventorySystem.Instance.RemoveItem(transform.name, 1);
            InventorySystem.Instance.IsUsingWeapon = false;
        }
    }
}
