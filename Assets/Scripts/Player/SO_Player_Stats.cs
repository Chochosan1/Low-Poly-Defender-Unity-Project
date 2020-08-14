using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player stats", menuName = "Chochosan/Player/Player_Stats")]
public class SO_Player_Stats : ScriptableObject
{
    [Header("Base stats")]
    public float maxHealth;
 //   [HideInInspector]
    public float currentHealth;

    public float maxRage;
//    [HideInInspector]
    public float currentRage;

    [Header("Attack stats")]
    public float autoAttackDamage = 10f;
    public float attackRange = 5f;
    public float autoKnockbackPower = 10f;

    [Header("Movement")]
    [Tooltip("The maximum angle the player is allowed to walk and jump on. On a flat ground the angle is 90. Going up a hill is > 90, going downhill is < 90.")]
    public float maxAngle = 120f;
    public float walkSpeed = 2350f;
    public float sprintSpeed = 3350f;
    public float rotationSpeed = 180f;
    public float jumpForce = 50f;
    public float rollForwardForce = 50f;
    public float rollCooldown = 4f;
}
