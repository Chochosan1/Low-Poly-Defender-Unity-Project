using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to the desired projectile.
/// </summary>
public class Fireball_Controller : MonoBehaviour
{
    public SO_FireBall fireballStats;

    private void OnCollisionEnter(Collision collision)
    {
        ExplodeAndDealDamage();
    }

    private void ExplodeAndDealDamage()
    {
        GameObject hitVFX = Instantiate(fireballStats.hitParticle, transform.position, fireballStats.hitParticle.transform.rotation);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fireballStats.areaOfEffect, fireballStats.enemyLayer);
        foreach (Collider hitCol in hitColliders)
        {
            if (hitCol.gameObject.CompareTag("AI"))
            {
                hitCol.gameObject.GetComponent<EnemyAI_Controller>().TakeDamage(fireballStats.damage, 0, this.gameObject);
            }
        }
        Destroy(hitVFX, 1f);
        Destroy(this.gameObject);
    }
}
