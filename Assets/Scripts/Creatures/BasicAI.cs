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
    

    [Header("Wander")]
    [SerializeField] private float wanderDistance = 25f;
    [SerializeField] private float walkSpeed = 1f;
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

        currentState = AIState.Idle;
        UpdateState();
    }

    protected virtual void UpdateState()
    {
        switch(currentState)
        {
            case AIState.Idle:
                HandleIdleState();
                animator.Play(idleAnim);
                break;

            case AIState.Moving:
                HandleMovingState();
                animator.Play(movingAnim);
                break;
        }
    }

    protected virtual void HandleMovingState()
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
                setState(AIState.Idle);
                yield break;
            }

            yield return null;
        }

        setState(AIState.Idle);
    }
    protected void setState(AIState state)
    {
        if(state == currentState) { return; }

        currentState = state;
        UpdateState();
    }

    protected virtual void HandleIdleState()
    {
        StartCoroutine(waitToMove());
    }

    private IEnumerator waitToMove()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(idleTime / 2f, idleTime * 2f));

        Vector3 randomDestionation = getRandomPoint(transform.position, wanderDistance);
        navMeshAgent.SetDestination(randomDestionation);
        setState(AIState.Moving);
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
    Moving
}