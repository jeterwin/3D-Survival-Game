using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppableTree : BreakableItem
{
    [SerializeField] private GameObject objectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out AxeItem axe))
        {
            TakeDamage(axe.TreeDamage);
            if(ObjectHealth % 25 == 0)
            {
                spawnObject(collision.contacts[0].point);
            }

            axe.TakeDamage(axe.SelfDamagerPerHit);
        }
    }

    private void spawnObject(Vector3 position)
    {
        Instantiate(objectPrefab, position, Quaternion.identity);
    }
}
