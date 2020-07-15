using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class KnockableObject : MonoBehaviour, IKnockback
{
    public float knockBackSuffer = 25f;
    public bool roll = true;
    private Rigidbody rb;
    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = transform.forward;
    }

    private void Update()
    {
        if(roll)
        {
            rb.AddForce(direction * 3400 * Time.deltaTime);
        }
        
    }

    public void SufferAttackWithKnockback(GameObject attacker)
    {
        rb.AddForce(attacker.transform.forward * knockBackSuffer, ForceMode.Impulse);
    }
}
