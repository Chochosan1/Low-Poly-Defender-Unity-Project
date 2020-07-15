using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject splitArrowPrefab;
    public GameObject hitParticle;
    public GameObject splitArrowParticle;
    [Header("Values")]
    public float damage;
    public float shootForce = 300f;
    public float ragePerShot = 150f;
    [Tooltip("If true the arrow will split into three separate arrows after some time.")]
    public bool splitArrowPerkActivated = true;
    [Tooltip("If true the arrow's head will follow a natural curve and look towards the direction of the movement of the arrow.")]
    public bool followTrajectoryAngle = false;
    public float splitArrowAfter = 0.1f;

    [Tooltip("How far aside should each split arrow spawn based on the position of the original arrow.")]
    public float splitArrowOffset = 1f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (gameObject.name.Contains("SplitArrow")) //don't allow splitting if it's not an original arrow
        {
            splitArrowPerkActivated = false;
        }
        else
        {
            if(gameObject.name.Contains("ArrowStatic"))
            {
                shootForce = Random.Range(shootForce / 2, shootForce);
            }
            ShootStraightArrow(transform.forward); //add force only if it's an original arrow
        }

       
        if(splitArrowPerkActivated)
        {
            StartCoroutine(StartArrowSplitCounter());
        }     
    }

    private void Update()
    {
        if(followTrajectoryAngle && rb.velocity.sqrMagnitude >= 0.5f)
        {
            transform.forward = rb.velocity;
        }
    }

    public void ShootStraightArrow(Vector3 forwardDir)
    {
        transform.SetParent(null);
        rb.AddForce(shootForce * forwardDir, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        followTrajectoryAngle = false; //disable trajectory follow to avoid strange behaviour when no longer flying
        splitArrowPerkActivated = false;
        if(collision.transform.CompareTag("AI"))
        {
            collision.gameObject.GetComponent<EnemyAI_Controller>().TakeDamage(damage, 0, this.gameObject);
            HitTarget(0f, true);
            if(ragePerShot > 0)
            {
                TPMovement_Controller.instance.UpdateRageAndRageBar(ragePerShot);
            }         
            return;
        }
        else if(collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<TPMovement_Controller>().TakeDamage(damage, this.gameObject);
            HitTarget(0f, true);
            return;
        }
        HitTarget(5f, false);
    }

    private void HitTarget(float destroyAfter, bool playHitEffect)
    {
        if(playHitEffect)
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
        }
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        
        GetComponent<BoxCollider>().enabled = false;
        Destroy(this.gameObject, destroyAfter);
    }

    private IEnumerator StartArrowSplitCounter()
    {
        yield return new WaitForSeconds(splitArrowAfter);

        if(splitArrowPerkActivated)
        {
            //spawn at the position of the object but to the right of it
            Vector3 spawnPos = transform.right + transform.position;
            GameObject firstSplitArrow = Instantiate(splitArrowPrefab, spawnPos * splitArrowOffset, transform.rotation);

            //preserve the velocity of the original arrow
            firstSplitArrow.GetComponent<Rigidbody>().velocity = rb.velocity;

            spawnPos = -transform.right + transform.position;
            GameObject secondSplitArrow = Instantiate(splitArrowPrefab, spawnPos * splitArrowOffset, transform.rotation);

            //preserve the velocity of the original arrow
            secondSplitArrow.GetComponent<Rigidbody>().velocity = rb.velocity;

            Instantiate(splitArrowParticle, transform.position, Quaternion.identity);
        }   
    }
}
