using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum AIState { None, Idle, Patrol, MovingToTarget, Attack, LookingForTarget, Dead }

[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyAI_Controller : Enemy_Base, IEnemyAI
{
    public bool debugMode;

    [Header("Prefabs")]
    public GameObject deathParticle;
    public GameObject arrowToShootPrefab, arrowSpawnPoint, arrowVisual;

    [Header("AI")]
    public float pureEnemySenseRange = 12f;
    [Tooltip("If true, the enemy will attack all targets tagged as his enemies if they get close enough. If false, he will attack only if he takes damage.")]
    public bool currentlyAggressive = true;
    [Tooltip("If the enemy takes damage from a far distance (more than his sense range), he will go to the last known area source from which he took damage in order to find his enemy.")]
    public bool checkAreaIfDamageTakenFromAfar = true;
    public float rotationSpeed;
    public float lookDirectionYoffset = 0.3f;

    [Header("Attack")]
    public LayerMask enemyLayer;
    public float attackYoffset = 0.5f;
    public float attackRange = 4f;
    public float rotateTowardsSpeed = 10f;
    [Tooltip("Keep in mind that the attack max duration is also added to that cooldown so setting it to 0 will result in a minimal cooldown but not 0.")]
    public float attackCooldown = 1.5f;
    [Tooltip("Attack animation and state will not last longer than this value after going into that state.")]
    public float attack1MaxDuration = 0.75f;
    private float internalAttack1Cooldown;
    private float elapsedAttackDuration;
    private float internalAttack1Timestamp;

    [Header("Additional options")]
    public bool is_Bowman = false;
    [Tooltip("This type of bowmen will not move and will just shoot parabola type arrows. Best used for archers on walls.")]
    public bool is_StaticBowman = false;
    public bool is_StaticBowmanActivated = true;

    private GameObject currentTarget;
    private EnemyAI_Controller currentTargetAI;
    private NavMeshAgent agent;
    private AIState aiState;
    private Vector3 direction;
    private Vector3 lastKnownSpotOfTheEnemy; //if the enemy takes damage from far away, this is the position he is going towards in order to check for enemies
    private float originalStoppingDistance;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //  rb = GetComponent<Rigidbody>();
        SetAttackCooldowns();
        currentHealth = maxHealth;
        damageReductionValue = armor * 0.5f / 100;
        if (damageReductionValue > 0.5f)
        {
            damageReductionValue = 0.5f;
        }

        originalStoppingDistance = agent.stoppingDistance;
        SetOrigin(gameObject.transform);
        GoToNoneState();

        //only bowmen can be set as static bowmen
        if (is_StaticBowman)
        {
            is_Bowman = true;
        }
    }

    private void Update()
    {
        if (aiState == AIState.MovingToTarget)
        {
            if (currentTarget != null)
            {
                ((IEnemyAI)this).GoToTarget(currentTarget.transform.position);
            }
            else
            {
                GoToNoneState();
            }
            anim.SetBool("is_WalkForward", true);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", true);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            agent.stoppingDistance = originalStoppingDistance;
            agent.speed = chaseSpeed;
            currentPatrolPoint = Vector3.zero;
        }
        else if (aiState == AIState.Idle)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", true);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            agent.stoppingDistance = originalStoppingDistance;
            agent.speed = chaseSpeed;
            currentPatrolPoint = Vector3.zero;
        }
        else if (aiState == AIState.Attack)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            currentPatrolPoint = Vector3.zero;
            agent.stoppingDistance = originalStoppingDistance;
            elapsedAttackDuration += Time.deltaTime;

            LookAtTarget();

            if (elapsedAttackDuration >= attack1MaxDuration && anim.GetBool("is_Attack1"))
            {
                GoToNoneState();
            }
        }
        else if (aiState == AIState.Patrol)
        {
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0f;
            currentTarget = null;
            currentTargetAI = null;
        }
        else if (aiState == AIState.LookingForTarget)
        {
            agent.stoppingDistance = 1;
            float distance = Vector3.Distance(lastKnownSpotOfTheEnemy, transform.position);
            if (distance >= agent.stoppingDistance && currentTarget == null && lastKnownSpotOfTheEnemy != Vector3.zero)
            {
                ((IEnemyAI)this).GoToTarget(lastKnownSpotOfTheEnemy);
            }
            else
            {
                lastKnownSpotOfTheEnemy = Vector3.zero;
                GoToNoneState();
            }
            anim.SetBool("is_WalkForward", true);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", true);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            agent.speed = chaseSpeed;
        }
        else if (aiState == AIState.None)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            currentPatrolPoint = Vector3.zero;
            currentTarget = null;
            currentTargetAI = null;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else if (aiState == AIState.Dead)
        {
            anim.SetBool("is_WalkBackward", false);
            anim.SetBool("is_WalkForward", false);
            anim.SetBool("is_Idle", false);
            anim.SetBool("is_WalkLeft", false);
            anim.SetBool("is_WalkRight", false);
            anim.SetBool("is_SprintForward", false);
            anim.SetBool("is_SprintBackward", false);
            anim.SetBool("is_RollForward", false);
            anim.SetBool("is_Attack1", false);
            anim.SetBool("is_Attack2", false);
            anim.SetBool("is_Dead", true);
            currentPatrolPoint = Vector3.zero;
            is_Dead = true;
            currentTarget = null;
            currentTargetAI = null;
            agent.SetDestination(transform.position);
        }

        if (aiState != AIState.Dead && !is_Dead && !is_StaticBowman)
        {
            ChooseNewTarget();

            if (debugMode)
            {
                Debug.Log(aiState);
            }

            float distance = 10000;
            if (currentTarget != null)
            {
                distance = Vector3.Distance(currentTarget.transform.position, transform.position);
                SetTargetToNullIfTargetIsDead();
            }

            if (distance <= pureEnemySenseRange && distance > agent.stoppingDistance && aiState != AIState.Attack && currentlyAggressive) //if player is far but scented then go to him
            {
                aiState = AIState.MovingToTarget;
            }
            else if (distance <= agent.stoppingDistance && internalAttack1Timestamp <= Time.time && currentlyAggressive) //if player is within stop range and can attack go to attack state
            {
                aiState = AIState.Attack;
                Attack();
            }
            else
            {
                if (aiState != AIState.Attack && distance <= pureEnemySenseRange) //go to idle only if not currently in attack state in order not to interrupt the attack
                {
                    aiState = AIState.Idle;
                }
                else if (aiState != AIState.Attack && aiState != AIState.LookingForTarget) //if player leaves enemy sense range and is not attacking currently go to patrol
                {
                    aiState = AIState.Patrol;

                    WanderAround(agent);
                }
            }
        }
        else if (aiState != AIState.Dead && !is_Dead && is_StaticBowman && is_StaticBowmanActivated)
        {
            AttackBow2();
        }
    }


    public override void TakeDamage(float damage, float sufferKnockbackPower, GameObject attacker)
    {
        if (!is_Dead)
        {
            if (currentTarget == null && checkAreaIfDamageTakenFromAfar)
            {
                aiState = AIState.LookingForTarget;
                lastKnownSpotOfTheEnemy = Player_Location.instance.transform.position;
            }
            currentlyAggressive = true;
            damage -= damage * damageReductionValue;
            //   Debug.Log(damage);
            currentHealth -= damage;
            //if (sufferKnockbackPower > 0)
            //{
            //    rb.AddForce(attacker.transform.forward * sufferKnockbackPower, ForceMode.Impulse);
            //}

            if (currentHealth <= 0)
            {
                if (attacker.CompareTag("Player"))
                {
                    attacker.GetComponent<QuestComponent>().CheckQuestProgressKill(this);
                }

                Die();
            }
        }
    }

    private void Die()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        GetComponent<CapsuleCollider>().enabled = false;
        aiState = AIState.Dead;

        //force the state by cancelling everything beforehand
        anim.SetBool("is_WalkBackward", false);
        anim.SetBool("is_WalkForward", false);
        anim.SetBool("is_Idle", false);
        anim.SetBool("is_WalkLeft", false);
        anim.SetBool("is_WalkRight", false);
        anim.SetBool("is_SprintForward", false);
        anim.SetBool("is_SprintBackward", false);
        anim.SetBool("is_RollForward", false);
        anim.SetBool("is_Attack1", false);
        anim.SetBool("is_Attack2", false);
        anim.SetBool("is_Dead", true);
        currentPatrolPoint = Vector3.zero;
        is_Dead = true;
        currentTarget = null;
        currentTargetAI = null;
        agent.SetDestination(transform.position);
    }

    //rotate target especially archers to look at the target so that they can shoot it even if on different ground level
    private void LookAtTarget()
    {
        if(currentTarget != null)
        {
            if (is_Bowman)
            {
                Vector3 lookVector = currentTarget.transform.position - transform.position;
                lookVector.y += lookDirectionYoffset;
                Quaternion rot = Quaternion.LookRotation(lookVector);
                transform.rotation = rot;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
                targetRotation.x = transform.rotation.x;
                targetRotation.z = transform.rotation.z;
                transform.rotation = targetRotation;
            }
        }  
    }

    public void Attack1()
    {
        //    Debug.Log("ENEMY ATTACK");
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + new Vector3(0f, attackYoffset, 0f)), out hit, attackRange, enemyLayer))
        {
            //   Debug.Log(hit.transform.name);

            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<TPMovement_Controller>().TakeDamage(damage, this.gameObject);
            }
            else if (hit.transform.CompareTag("AI"))
            {
                EnemyAI_Controller ai = hit.transform.GetComponent<EnemyAI_Controller>();
                hit.transform.GetComponent<EnemyAI_Controller>().TakeDamage(damage, 0, this.gameObject);
            }
        }
    }

    public void ShootArrow1()
    {
        GameObject arrow = Instantiate(arrowToShootPrefab, arrowSpawnPoint.transform);
    }

    public void ShootArrow2()
    {
        GameObject arrow = Instantiate(arrowToShootPrefab, arrowSpawnPoint.transform);

        //spawn arrow just a bit in front of the hero to avoid self-collision
        arrow.transform.position = arrowSpawnPoint.transform.position + arrowSpawnPoint.transform.forward;
    }

    public void DisableArrowVisual()
    {
        arrowVisual.SetActive(false); //disable the arrow if this state is called in case that it is active
    }

    //call when needed to reset the current state; mostly used to exit the Attack state by calling it during an animation event or forcefully after X time spent in the state
    public void GoToNoneState()
    {
        aiState = AIState.None;
        ResetAllElapsedAttackDurations();
    }

    private void ChooseNewTarget()
    {
        if (currentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, pureEnemySenseRange, enemyLayer);
            if (hitColliders.Length > 0)
            {
                currentTarget = hitColliders[0].gameObject;
                if (currentTarget.CompareTag("AI"))
                {
                    currentTargetAI = currentTarget.GetComponent<EnemyAI_Controller>();
                }
            }
        }
    }

    private void SetTargetToNullIfTargetIsDead()
    {
        if (currentTargetAI != null && currentTargetAI.aiState == AIState.Dead)
        {
            //  Debug.Log("TARGETS RESET");
            currentTarget.layer = 0; //remove enemy from the enemyLayer so it does not get detected with raycasts
            currentTarget = null;
            currentTargetAI = null;
        }
    }

    private void SetAttackCooldowns()
    {
        internalAttack1Cooldown = attack1MaxDuration + attackCooldown;
    }

    private void ResetAllElapsedAttackDurations()
    {
        elapsedAttackDuration = 0f;
    }

    private void Attack()
    {
        internalAttack1Timestamp = Time.time + internalAttack1Cooldown;
        anim.SetBool("is_Attack1", true);

        if (is_Bowman)
        {
            arrowVisual.SetActive(true);
        }

        if (currentTarget != null)
        {          
            LookAtTarget();
        }
    }

    private void AttackBow2()
    {
        if (is_Bowman)
        {
            arrowVisual.SetActive(true);
            anim.SetBool("is_Attack2", true);
        }
    }


    private bool HasAnimatorFinishedPlayingCurrentStateAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0);
    }

    void IEnemyAI.GoToTarget(Vector3 targetPos)
    {
        agent.destination = targetPos;
    }
}
