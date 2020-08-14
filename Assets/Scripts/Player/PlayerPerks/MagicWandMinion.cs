using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Wand controller, finds a target and shoots a projectile towards it. Projectile controller is a separate script. Projectile stats are a ScriptableObject.
/// </summary>
public class MagicWandMinion : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    private Rigidbody rb;

    [SerializeField]
    private float castCooldown;
    private float castTimestamp;

    [SerializeField]
    private float force;
    private GameObject currentTarget;
    private EnemyAI_Controller currentTargetAI;

    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private float enemyDetectionArea;

    private void Start()
    {
     //   rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ChooseNewTarget();
        if (Time.time >= castTimestamp && currentTarget != null)
        {
            SetTargetToNullIfTargetIsDead();       
            castTimestamp = Time.time + castCooldown;
            CastSpell();
        }
    }


    private void CastSpell()
    {
        if(currentTarget != null)
        {
            Vector3 offsetVector = new Vector3(0, 0f, 0);
            GameObject projectileCopy = Instantiate(projectilePrefab, transform.position + offsetVector, projectilePrefab.transform.rotation);
            projectileCopy.GetComponent<Rigidbody>().AddForce((currentTarget.transform.position - transform.position) * force, ForceMode.Impulse);
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
            //  Debug.Log("TARGETS RESET");
            currentTarget.layer = 0; //remove enemy from the enemyLayer so it does not get detected with raycasts
            currentTarget = null;
            currentTargetAI = null;
        }
    }
}
