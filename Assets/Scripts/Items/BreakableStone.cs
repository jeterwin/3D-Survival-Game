using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableStone : BreakableItem
{
    [SerializeField] private GameObject objectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out PickaxeItem pickaxe))
        {
            TakeDamage(pickaxe.StoneDamage);
            spawnObject(collision.contacts[0].point, pickaxe);

            pickaxe.TakeDamage(pickaxe.SelfDamagerPerHit);
        }
    }

    private void spawnObject(Vector3 position, PickaxeItem axe)
    {
        Instantiate(axe.ParticlesHitSFX, position, Quaternion.identity);
        Instantiate(objectPrefab, position, Quaternion.identity);
    }
}
