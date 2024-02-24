using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreedableAnimal : MonoBehaviour
{
    public bool CanBeBred = true;
    public bool IsFed 
    { 
        get { return isFed; }
        set 
        { 
            isFed = value; 
            if(isFed == true)
            {
                onValueChanged();
            }
        } 
    }

    private void onValueChanged()
    {
        StartCoroutine(mateSearch());
    }

    [SerializeField] private bool isFed = false;
    public Item FeedableItem { get { return feedableItem; } }
    public AnimalType AnimalType
    {
        get { return animalType; } 
    }

    [SerializeField] private AnimalType animalType;

    [SerializeField] private float timeBetweenBreedings = 180f;
    [SerializeField] private float timeBetweenPairSearch = 1f; //This is the time between each search
    //for a compatible mate

    [SerializeField] private float breedingRange = 25f;
    [SerializeField] private float animalDistanceRange = 1f; //This is how close the animals must be to eachother

    [SerializeField] private Item feedableItem;

    [SerializeField] private GameObject animalChildPrefab;
    [SerializeField] private GameObject loveParticles;

    private BasicAI animalAI;
    private NavMeshAgent navMeshAgent;

    private bool foundTwoCompatibles = false;
    private BreedableAnimal compatibleAnimal;
    private BasicAI compAnimalAI;
    private NavMeshAgent compAnimalNavMeshAgent;
    private void Start()
    {
        animalAI = GetComponent<BasicAI>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, breedingRange);
    }

    public IEnumerator BreedingTimer()
    {
        yield return new WaitForSeconds(timeBetweenBreedings);

        CanBeBred = true;
    }
    private IEnumerator mateSearch()
    {
        yield return new WaitForSeconds(timeBetweenPairSearch);

        if(!CanBeBred)
        {
            StartCoroutine(mateSearch());
            yield break;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, breedingRange);
        foreach(Collider collider in colliders)
        {
            if(collider.TryGetComponent(out BreedableAnimal animal))
            {
                if(animal.AnimalType != AnimalType || animal == this) { yield break; }
                
                //We have two animals of the same type that are both fed
                if(IsFed && animal.IsFed)
                {
                    compatibleAnimal = animal;
                    foundTwoCompatibles = true;

                    animalAI.StopAllCoroutines();
                    navMeshAgent.destination = compatibleAnimal.transform.position;
                    animalAI.SetState(AIState.Breeding);

                    compAnimalAI = compatibleAnimal.GetComponent<BasicAI>();
                    compAnimalNavMeshAgent = compAnimalAI.GetComponent<NavMeshAgent>();
                    
                    compAnimalAI.StopAllCoroutines();
                    compAnimalNavMeshAgent.destination = transform.position;
                    compAnimalAI.SetState(AIState.Breeding);
                }
            }
        }
    }
    private void Update()
    {
        if(foundTwoCompatibles)
        {
            navMeshAgent.destination = compatibleAnimal.transform.position;
            compAnimalNavMeshAgent.destination = transform.position;

            if(Vector3.Distance(navMeshAgent.destination, compAnimalNavMeshAgent.destination) < 0.4f)
            {
                Instantiate(animalChildPrefab, transform.position, Quaternion.identity);
                Instantiate(loveParticles, transform.position, loveParticles.transform.rotation);

                resetVariables();
            }
        }
    }

    private void resetVariables()
    {
        foundTwoCompatibles = false;

        animalAI.StopAllCoroutines();
        compAnimalAI.StopAllCoroutines();

        animalAI.SetState(AIState.Idle);
        compAnimalAI.SetState(AIState.Idle);

        IsFed = CanBeBred = false;
        compatibleAnimal.IsFed = compatibleAnimal.CanBeBred = false;

        StartCoroutine(BreedingTimer());
        compatibleAnimal.StartCoroutine(compatibleAnimal.BreedingTimer());

        compatibleAnimal = null;
        compAnimalAI = null;
        compAnimalNavMeshAgent = null;
    }
}
public enum AnimalType
{
    Chicken,
    Dog,
    Cat
}
