using Platformer;
using UnityEngine;
using System;
using UnityEngine.AI; // For NavMesh following

public class EnemyAI : MonoBehaviour
{
    ////////////////ENEMY BEHAVIOUR////////////////////
    public Transform player;          // Player to follow
    public float followRange = 10f;   // How far enemy can detect the player
    public float attackRange = 2f;    // Distance to start attack
    public int damage = 10;           // Damage amount
    public float attackCooldown = 1f; // Time between attacks

    private NavMeshAgent agent;
    private float lastAttackTime;

    //////////////////ENEMY STATES//////////////////
    public enum Enemystate
    {
        Idle,
        Chasing,
        Attacking
    }
    public Enemystate currentState;

    // Animator for enemy animations
    private Animator animator;

    ///////////////////////////////////////////////////// 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Get the Animator component
        currentState = Enemystate.Idle;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Handle state transitions
        switch (currentState)
        {
            case Enemystate.Idle:
                IdleState(distance);
                break;

            case Enemystate.Chasing:
                ChasingState(distance);
                break;

            case Enemystate.Attacking:
                AttackingState(distance);
                break;
        }

        // Animator state handling based on FSM
        HandleAnimations();
    }

    void IdleState(float distance)
    {
        if (distance <= followRange)
        {
            currentState = Enemystate.Chasing;
        }
    }

    void ChasingState(float distance)
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (distance <= attackRange)
        {
            currentState = Enemystate.Attacking;
        }
        else if (distance > followRange)
        {
            currentState = Enemystate.Idle;
        }
    }

    void AttackingState(float distance)
    {
        agent.isStopped = true;
        Attack();

        if (distance > attackRange)
        {
            currentState = Enemystate.Chasing;
        }
    }

    void Attack()
    {
        Debug.Log("Enemy is trying to attack!");

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                Debug.Log("Dealing damage to player!");
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Player has no Health component!");
            }
            lastAttackTime = Time.time;
        }
    }

    void HandleAnimations()
    {
        // Set animator parameters based on FSM states
        switch (currentState)
        {
            case Enemystate.Idle:
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                break;

            case Enemystate.Chasing:
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsAttacking", false);
                break;

            case Enemystate.Attacking:
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", true);
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Debug ranges in Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
