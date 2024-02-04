using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableItem : MonoBehaviour
{
    [SerializeField] private int objectHealth = 100;

    public int ObjectHealth
    {
        get { return objectHealth; }
    }
    public void TakeDamage(int damage)
    {
        objectHealth -= damage;
        if (objectHealth <= 0)
        {
            despawnObject();
        }
    }

    private void despawnObject()
    {
        Destroy(gameObject);
    }
}
