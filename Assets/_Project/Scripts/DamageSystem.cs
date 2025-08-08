using Platformer;
using UnityEngine;
using UnityEngine.AI; // For NavMesh following

public class EnemyAI : MonoBehaviour
{
    public Transform player;          // Player to follow
    public float followRange = 10f;   // How far enemy can detect the player
    public float attackRange = 2f;    // Distance to start attack
    public int damage = 10;           // Damage amount
    public float attackCooldown = 1f; // Time between attacks

    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Detection & Follow
        if (distance <= followRange && distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (distance <= attackRange)
        {
            agent.isStopped = true;
            Attack();
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


    void OnDrawGizmosSelected()
    {
        // Debug ranges in Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
