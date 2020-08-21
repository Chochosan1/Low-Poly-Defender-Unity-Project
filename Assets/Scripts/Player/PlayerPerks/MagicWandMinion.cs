using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Wand controller, finds a target and shoots a projectile towards it. Projectile controller is a separate script. Projectile stats are a ScriptableObject.
/// </summary>
public class MagicWandMinion : MonoBehaviour, IInteractable
{
    public static MagicWandMinion Instance;

    [SerializeField]
    private GameObject projectilePrefab;

    //pooling
    [SerializeField]
    [Tooltip("The size of the object pool. Make sure it is big enough so that it can support even a fast attack rate. The pool will get filled during runtime, when this limit is reached items from the pool will be reused.")]
    private int projectilePoolSize = 15;
    [HideInInspector]
    public List<GameObject> fireballPool;
    [HideInInspector]
    public bool stillSpawning = true;
    private int currentPoolItem = 0; //used to iterate through the pool of items

    [SerializeField]
    private float ragePerShotCost;
    [SerializeField]
    private float castCooldown;
    [SerializeField]
    Vector3 offsetVector;
    private float castTimestamp;

    [SerializeField]
    private bool wandLooted = false;

    [SerializeField]
    private float force = 3f;
    private GameObject currentTarget;
    private EnemyAI_Controller currentTargetAI;

    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private float enemyDetectionArea = 10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void Update()
    {
        if (wandLooted)
        {
            ChooseNewTarget();
            if (Time.time >= castTimestamp && currentTarget != null)
            {
                SetTargetToNullIfTargetIsDead();
                castTimestamp = Time.time + castCooldown;
                CastSpell();
            }
        }
    }

    private void CastSpell()
    {
        if (currentTarget != null && TPMovement_Controller.instance.CheckIfEnoughRageForSpell(ragePerShotCost, false))
        {
            TPMovement_Controller.instance.UpdateRageAndRageBar(-ragePerShotCost);
            if (stillSpawning) //if the pool is still not full
            {
                GameObject projectileCopy = Instantiate(projectilePrefab, transform.position + offsetVector, projectilePrefab.transform.rotation);
                projectileCopy.GetComponent<Rigidbody>().AddForce((currentTarget.transform.position - transform.position) * force, ForceMode.Impulse);
            }
            else //when full start using items from the pool
            {
                // Debug.Log("I COME FROM THE POOL");
                Rigidbody tempRb = fireballPool[currentPoolItem].GetComponent<Rigidbody>();
                tempRb.velocity = new Vector3(0, 0, 0);
                fireballPool[currentPoolItem].transform.position = transform.position + offsetVector;
                fireballPool[currentPoolItem].SetActive(true);
                tempRb.AddForce((currentTarget.transform.position - transform.position) * force, ForceMode.Impulse);
                currentPoolItem++;

                if (currentPoolItem >= fireballPool.Count)
                {
                    currentPoolItem = 0;
                    //   Debug.Log("MAX OBJECT REACHED, STARTING FROM THE START");
                }
            }
        }
    }

    private void ChooseNewTarget()
    {
        if (currentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionArea, enemyLayer);
            if (hitColliders.Length > 0)
            {
                int randomEnemyIndex = Random.Range(0, hitColliders.Length);
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
            currentTarget.layer = 0; //remove enemy from the enemyLayer so it does not get detected with raycasts
            currentTarget = null;
            currentTargetAI = null;
        }
    }

    //add objects to the pool during runtime while instantiating objects, when a certain limit is reached then disable instantiating
    //and start using the pool
    public void AddBallToPool(GameObject fireball)
    {
        fireballPool.Add(fireball);

        if (fireballPool.Count >= projectilePoolSize)
        {
            stillSpawning = false;
            //   Debug.Log("DISABLING INSTANTIATION!");
        }
    }

    public void LootWand()
    {
        wandLooted = true;
        this.GetComponentInParent<Follow_Target>().EnableFollow();
        this.gameObject.tag = "Untagged";
        this.gameObject.layer = 0;

        Player_Inventory.instance.UpdateRewardPanelText("Wand companion acquired", 0);
    }

    public void Interact()
    {
        LootWand();
       
    }
}
