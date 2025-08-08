using Platformer;
using UnityEngine;
using System;
using UnityEngine.AI; 

public class EnemyAI : MonoBehaviour
{
    ////////////////ENEMY BEHAVIOUR////////////////////
    public Transform player;         
    public float followRange = 10f;   
    public float attackRange = 2f;   
    public int damage = 10;           
    public float attackCooldown = 1f; 

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

    
    private Animator animator;

    ///////////////////////////////////////////////////// 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = Enemystate.Idle;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        /////STATE TRANSITIONA/////////////////////////////////////////////////////////////
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
        ///////ANIMATION PARAMETERZZZZ/////////////////////////////////
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
        ////////EASIER VIEWING OF RANGEZ////////////////////////////////
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
