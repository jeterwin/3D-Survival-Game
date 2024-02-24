using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthAI : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private float currentHealth = 0;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    [SerializeField] private float maxHealth = 100f;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } } 

    [Header("Damage Status")]
    [SerializeField] private float coloredTimer = 0.1f;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 damagedColor;

    [Header("Death Stats")]
    [SerializeField] private Item[] itemsToGive;
    [SerializeField] private GameObject deathParticles;

    private List<Material> materials = new();

    private BasicAI AI;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            materials.Add(child.GetComponent<MeshRenderer>().materials[0]);
        }

        AI = GetComponent<BasicAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out WeaponItem weapon))
        {
            TakeDamage(weapon.EnemyDamage);
        }
    }

    private void Start()
    {
        Initialise();
    }
    public void TakeDamage(int damage)
    {
        AI.SetState(AIState.Fleeing);

        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            despawn();
        }
        StartCoroutine(tookDamage());
    }

    private IEnumerator tookDamage()
    {
        foreach(Material mat in materials)
        {
            mat.color = damagedColor;
        }

        yield return new WaitForSeconds(coloredTimer);

        foreach(Material mat in materials)
        {
            mat.color = normalColor;
        }
    }
    private void despawn()
    {
        foreach(Item item in itemsToGive)
        {
            Instantiate(item, transform.position, transform.rotation);
        }
        Instantiate(deathParticles, transform.position, deathParticles.transform.rotation);
        //Might create pooling system for this
        Destroy(gameObject);
    }

    private void Initialise()
    {
        currentHealth = maxHealth;
    }
}
