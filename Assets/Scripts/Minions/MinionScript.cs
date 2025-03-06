using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionScript : MonoBehaviour
{
    [Header("Movement")]
    public Transform targetPosition;

    [Header("Stats")]
    public int damage = 1;
    public int health = 5;
    public int maxHealth = 5;
    [Tooltip("How many attacks in one second")]
    public float attackSpeed = 0.2f;
    

    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void Death()
    {

    }


}
