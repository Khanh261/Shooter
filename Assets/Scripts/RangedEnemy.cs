using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject projectilePrefab;
    float attackRange;
    float projectileSpeed;

    protected override void Start()
    {
        base.Start();
        currentState = State.Chasing;
    }

    void Update()
    {
        if (hasTarget && target != null) // Add a null check for target
        {
            if (Time.time > nextAttackTime)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget <= attackRange)
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(AttackFromDistance());
                }
            }
        }
    }

    IEnumerator AttackFromDistance()
    {
        currentState = State.AttackingFromDistance;
        pathfinder.enabled = false;

        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dirToTarget);
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);

            if (projectilePrefab != null)
            {
                GameObject projectileInstance = Instantiate(
                    projectilePrefab,
                    transform.position,
                    transform.rotation
                );
                Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();
                if (projectileRigidbody != null)
                {
                    projectileRigidbody.velocity = dirToTarget * projectileSpeed;
                    projectileRigidbody.collisionDetectionMode =
                        CollisionDetectionMode.ContinuousDynamic;
                }
            }
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    public void SetCharacteristicsRangedEnemy(
        float moveSpeed,
        int hitToKillPlayer,
        float rangedEnemyHealth,
        float projectileSpeed,
        float attackRange
    )
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitToKillPlayer);
        }

        startingHealth = rangedEnemyHealth;
        this.projectileSpeed = projectileSpeed;
        this.attackRange = attackRange;
    }
}
