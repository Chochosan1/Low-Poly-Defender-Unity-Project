using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Base : MonoBehaviour
{
    [Header("Enemy_Base AI")]
    public float maxHealth;
    public float armor;
    public float damage;
    public float wanderZone;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 3.5f;
    public float minIdleStayOnPatrolPoint = 3f, maxIdleStayOnPatrolPoint = 10f;
    [SerializeField]
    private bool isUsingWaypoints = false;
    [SerializeField]
    private List<Transform> waypoints;

    protected float idleStayOnPatrolPoint;
    protected float elapsedIdleStayOnPatrolPoint;

    protected float damageReductionValue = 0f;
    protected float currentHealth;
    protected bool is_Dead = false;
    protected Animator anim;
    protected Rigidbody rb;
    protected Vector3 currentPatrolPoint, origin;

    public virtual void TakeDamage(float damage, float sufferKnockbackPower, GameObject attacker)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
    }

    //a random REACHABLE point on the navmesh; other alternatives of picking a point might glitch the AI
    protected virtual Vector3 RandomPointInRange(Vector3 origin, float wanderZone)
    {
        Vector3 currentPatrolPoint = new Vector3(0, 0, 0);
        //   Vector3 randomPoint = origin + Random.insideUnitSphere * wanderZone;
        // return new Vector3(randomPoint.x, transform.position.y, randomPoint.z);
        if (!isUsingWaypoints)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderZone;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderZone, NavMesh.AllAreas);
            currentPatrolPoint = hit.position;
        }
        else
        {
            currentPatrolPoint = waypoints[Random.Range(0, waypoints.Count - 1)].position;
        }


        return currentPatrolPoint;
    }

    protected virtual void WanderAround(NavMeshAgent agent)
    {
        if (currentPatrolPoint == Vector3.zero) //pick a new point
        {
            currentPatrolPoint = RandomPointInRange(origin, wanderZone);
            idleStayOnPatrolPoint = Random.Range(minIdleStayOnPatrolPoint, maxIdleStayOnPatrolPoint);
        }
        else //go to the current point and control the animations
        {
            agent.SetDestination(currentPatrolPoint);
            anim.SetBool("is_WalkForward", true);
            anim.SetBool("is_Idle", false);
            if (Vector3.Distance(currentPatrolPoint, transform.position) < 1f)
            {
                anim.SetBool("is_WalkForward", false);
                anim.SetBool("is_Idle", true);
                elapsedIdleStayOnPatrolPoint += Time.deltaTime;
                if (elapsedIdleStayOnPatrolPoint >= idleStayOnPatrolPoint)
                {
                    idleStayOnPatrolPoint = Random.Range(minIdleStayOnPatrolPoint, maxIdleStayOnPatrolPoint);
                    currentPatrolPoint = RandomPointInRange(origin, wanderZone);
                    elapsedIdleStayOnPatrolPoint = 0f;
                }
            }
        }
    }

    protected void SetOrigin(Transform objectTransform)
    {
        origin = objectTransform.position;
    }
}
