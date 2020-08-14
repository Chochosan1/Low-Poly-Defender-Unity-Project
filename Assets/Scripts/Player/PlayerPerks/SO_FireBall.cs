using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used to create a SO that holds all stats for the projectile.
/// </summary>
[CreateAssetMenu(fileName = "Magic wand fireball", menuName = "Chochosan/Companions/Magic wand")]
public class SO_FireBall : ScriptableObject
{
    public float damage;
    public float areaOfEffect;
    public LayerMask enemyLayer;
    public float knockBackPower;

    public GameObject muzzleParticle;
    public GameObject hitParticle;
}
