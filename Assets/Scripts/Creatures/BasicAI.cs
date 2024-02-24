using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAI : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnimName;
    [SerializeField] private string movingAnimName;

    private int idleAnim;
    private int movingAnim;
    
    [Header("Fleeing")]
    [SerializeField] private float fleeingSpeed = 4f;
    [SerializeField] private float fleeingDistance = 35f;
    [SerializeField] private float maxFleeingTime = 5f;

    [Header("Wander")]
    [SerializeField] private float wanderDistance = 25f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float maxWalkTime = 6f;

    [Header("Idle")]
    [SerializeField] private float idleTime = 3f;

    private protected NavMeshAgent navMeshAgent;
    private protected AIState currentState = AIState.Idle;

    private void Start()
    {
        idleAnim = Animator.StringToHash(idleAnimName);
        movingAnim = Animator.StringToHash(movingAnimName);

        InitialiseAI();
    }

    private void InitialiseAI()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = walkSpeed;

        SetState(AIState.Idle);
        UpdateState();
    }

    public void UpdateState()
    {
        switch(currentState)
        {
            case AIState.Idle:
                handleIdleState();
                animator.Play(idleAnim);
                break;

            case AIState.Moving:
                handleMovingState();
                animator.Play(movingAnim);
                break;

            case AIState.Fleeing:
                handleFleeingState();
                animator.Play(movingAnim);
                break;

            case AIState.Breeding:
                handleBreedingState();
                animator.Play(movingAnim);
                break;
        }
    }

    private void handleBreedingState()
    {
        StartCoroutine(startBreeding());
    }
    private IEnumerator startBreeding()
    {
        navMeshAgent.speed = walkSpeed;

        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        SetState(AIState.Idle);
    }
    protected virtual void handleFleeingState()
    {
        //We don't want our idle / moving coroutines to mess up our fleeing state since we can be hit
        //while moving or while being idle
        StopAllCoroutines();

        StartCoroutine(startFleeing());
    }

    private IEnumerator startFleeing()
    {
        float startTime = Time.time;

        navMeshAgent.speed = fleeingSpeed;

        Vector3 randomDestionation = getRandomPoint(transform.position, fleeingDistance);
        navMeshAgent.SetDestination(randomDestionation);

        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if(Time.time - startTime >= maxFleeingTime)
            {
                navMeshAgent.ResetPath();
                SetState(AIState.Idle);
                yield break;
            }

            yield return null;
        }

        SetState(AIState.Idle);
    }

    protected virtual void handleMovingState()
    {
        StartCoroutine(waitToStartDestination());
    }

    private IEnumerator waitToStartDestination()
    {
        float startTime = Time.time;

        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            if(Time.time - startTime >= maxWalkTime)
            {
                navMeshAgent.ResetPath();
                SetState(AIState.Idle);
                yield break;
            }

            yield return null;
        }

        SetState(AIState.Idle);
    }
    public void SetState(AIState state)
    {
        if(state == currentState) { return; }

        currentState = state;
        UpdateState();
    }

    protected virtual void handleIdleState()
    {
        StartCoroutine(waitToMove());
    }

    private IEnumerator waitToMove()
    {
        navMeshAgent.speed = walkSpeed;

        yield return new WaitForSeconds(UnityEngine.Random.Range(idleTime / 2f, idleTime * 2f));

        Vector3 randomDestionation = getRandomPoint(transform.position, wanderDistance);
        navMeshAgent.SetDestination(randomDestionation);
        SetState(AIState.Moving);
    }

    protected Vector3 getRandomPoint(Vector3 origin, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navMeshHit;

        if(NavMesh.SamplePosition(randomDirection, out navMeshHit, distance, NavMesh.AllAreas))
        {
            return navMeshHit.position;
        }
        else
        {
            return getRandomPoint(origin, distance);
        }
    }
}

public enum AIState
{
    Idle,
    Moving,
    Fleeing,
    Breeding
}