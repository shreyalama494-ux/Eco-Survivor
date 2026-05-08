using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private DayNightCycle dayNight;
    private Animator anim;
    private Rigidbody rb;

    [Header("Combat Settings")]
    public Transform player;
    public float attackRange = 6.0f;     // Increased to ensure hits land
    public float attackCooldown = 4.0f; 
    public float pounceForce = 15f;      // Slightly lowered to prevent flying too far
    
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
        
        // Automatically find the player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        dayNight = Object.FindFirstObjectByType<DayNightCycle>();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.isKinematic = true; 
        }
    }

    void Update()
    {
        if (dayNight == null || player == null || isAttacking) return;

        // Check if it is night time
        bool isNight = dayNight.sun.intensity < 0.15f;

        if (isNight)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false; 
                agent.SetDestination(player.position);
            }

            float distance = Vector3.Distance(transform.position, player.position);
            
            // Check if player is in range and cooldown is over
            if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(TripleAttackRoutine());
            }

            // Handle walking animation
            if (anim != null)
            {
                float movementVisual = agent.velocity.magnitude / agent.speed;
                anim.SetFloat("isRunning", movementVisual);
            }
        }
        else
        {
            // Stop moving during the day
            if (agent.isOnNavMesh) agent.isStopped = true;
            if (anim != null) anim.SetFloat("isRunning", 0.0f);
        }
    }

    void FacePlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    IEnumerator TripleAttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // 1. Stop NavMesh and animations
        if (agent.isOnNavMesh) agent.isStopped = true; 
        if (anim != null) anim.SetFloat("isRunning", 0.0f);

        FacePlayer();
        
        // 2. The Pounce (Small jump forward)
        if (rb != null)
        {
            rb.isKinematic = false; 
            Vector3 jumpDirection = (player.position - transform.position).normalized;
            rb.AddForce(jumpDirection * pounceForce + Vector3.up * 3f, ForceMode.Impulse);
        }

        // 3. The 3-Hit Combo
        for (int i = 0; i < 3; i++)
        {
            FacePlayer();
            
            // Trigger Attack Animation
            if (anim != null) anim.SetTrigger("Attack");

            // WAIT for the animation strike point (roughly 0.5 seconds into the animation)
            yield return new WaitForSeconds(0.5f);

            // DAMAGE LOGIC
            if (player != null)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(1.5f); // Deals 1.5 damage per hit
                    Debug.Log("Tiger HIT the player! HP decreased.");
                }
                else
                {
                    Debug.LogError("No PlayerHealth script found on the object in the Tiger's Player slot!");
                }
            }

            // WAIT for animation to finish before next bite
            yield return new WaitForSeconds(0.7f);
        }

        // 4. Reset for next chase
        if (rb != null) rb.isKinematic = true; 
        isAttacking = false;
        if (agent.isOnNavMesh) agent.isStopped = false; 
    }
}