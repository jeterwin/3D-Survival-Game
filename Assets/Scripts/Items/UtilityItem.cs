using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class UtilityItem : MonoBehaviour
{
    public UsableItem item;

    [SerializeField] private AudioClip swingSFX;
    [SerializeField] private AudioClip hitSFX;

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

    [SerializeField] private int enemyDamage = 0;
    public int SelfDamagerPerHit
    {
        get { return selfDamagePerHit; } 
    }

    [SerializeField] private Animator animator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Swing"))
        {
            SetBool();

            audioSource.PlayOneShot(swingSFX);
        }
    }
    private void SetBool()
    {
        animator.SetBool(triggerName, true);
    }
    private void AnimationEvent_ResetTrigger()
    {
        animator.SetBool(triggerName, false);
    }
    public abstract void AnimationEvent_EnableCollider();

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(hitSFX);
        itemDurability -= damage;
        float durabilityPercentage = itemDurability / maxItemDurability;
        item.DurabilityBar.fillAmount = durabilityPercentage;
        item.DurabilityBar.color = item.DurabilityGradient.Evaluate(durabilityPercentage);

        if(itemDurability <= 0)
        {
            InventorySystem.Instance.RemoveItem(transform.name, 1);
        }
    }
}
