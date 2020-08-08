using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class NPC_Controller : Enemy_Base
{
    public float greetAnimationCooldown = 5f;
    private float greetAnimationTimestamp;

    private NavMeshAgent agent;

    private void Start()
    { 
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = patrolSpeed;
        SetOrigin(gameObject.transform);
    }

    void Update()
    {
        WanderAround(agent);
    }

    //called during the HandleInteract event in TPMovement_Controller
    public void StopCurrentWanderPointAndRotateTowardsTarget(GameObject currentTarget)
    {
        currentPatrolPoint = transform.position;
        idleStayOnPatrolPoint = maxIdleStayOnPatrolPoint;

        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        transform.rotation = targetRotation;

        if(greetAnimationTimestamp <= Time.time)
        {
            anim.SetBool("is_Interacted", true);

            greetAnimationTimestamp = Time.time + greetAnimationCooldown;
        }            
    }
}
