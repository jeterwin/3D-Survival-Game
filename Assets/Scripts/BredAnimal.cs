using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BredAnimal : MonoBehaviour
{
    [Header("Breading Stats")]
    [SerializeField] private float minBreedingTime = 60f; //In seconds
    [SerializeField] private float maxBreedingTime = 120f; //In seconds
    [SerializeField] private float maxHPAfterGrowing = 100f;
    [SerializeField] private Vector3 bredScale = new(1, 1, 1);

    private HealthAI healthScript;


    private void Start()
    {
        healthScript = GetComponent<HealthAI>();

        StartCoroutine(startGrowing());
    }

    private IEnumerator startGrowing()
    {
        float randomTime = UnityEngine.Random.Range(minBreedingTime, maxBreedingTime);
        //Wait for a random time for the animal to grow up
        yield return new WaitForSeconds(randomTime);

        healthScript.CurrentHealth = maxHPAfterGrowing;
        healthScript.MaxHealth = maxHPAfterGrowing;
        //Then make it scale turn bigger and de-activate the script
        transform.localScale = bredScale;
        enabled = false;
    }
}
