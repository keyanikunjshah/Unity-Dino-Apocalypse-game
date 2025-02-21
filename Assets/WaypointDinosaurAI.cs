using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WaypointDinosaurAI : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public CapsuleCollider capsuleCollider;
    public enum DinosaurState { Walk, Chase, Attack, Dead }
    public DinosaurState currentState = DinosaurState.Walk;
    public Transform player;

    public float chaseDistance = 10f;
    public float attackDistance = 2f;
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;
    public int damage = 10;
    public int health = 100;
    public int attackRepetitions = 3;

    private bool isAttacking = false;
    private float lastAttackTime;
    public GameObject BloodScreenEffect;
    private GameObject instantiatedObject;

    [Header("Dinosaur Animation")]
    public Animator anim;

    void Start()
    {

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.Log("Player not found");
        }
        if (BloodScreenEffect == null)
        {
            BloodScreenEffect = Resources.Load<GameObject>("BloodScreenEffect");
            if (BloodScreenEffect == null)
            {
                Debug.LogWarning("BloodScreenEffect prefab not found in Resources.");
            }
        }

        navAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled || !navAgent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent is not on a NavMesh or is not active.");
            return;
        }

        switch (currentState)
        {
            case DinosaurState.Walk:
                Walk();
                if (IsPlayerInRange(chaseDistance))
                {
                    currentState = DinosaurState.Chase;
                }
                break;

            case DinosaurState.Chase:
                ChasePlayer();
                if (IsPlayerInRange(attackDistance))
                {
                    currentState = DinosaurState.Attack;
                }
                break;

            case DinosaurState.Attack:
                AttackPlayer();
                if (!IsPlayerInRange(attackDistance))
                {
                    currentState = DinosaurState.Chase;
                    navAgent.isStopped = false;
                }
                break;

            case DinosaurState.Dead:
                navAgent.enabled = false;
                capsuleCollider.enabled = false;
                anim.SetTrigger("died");
                enabled = false;
                break;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    private void Walk()
    {
        navAgent.speed = 0.3f;
        Vector3 randomPosition = RandomNavMeshPosition();
        navAgent.SetDestination(randomPosition);
    }

    private Vector3 RandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    private void ChasePlayer()
    {
        navAgent.speed = 3f;
        navAgent.SetDestination(player.position);

        // Trigger the roar animation during the chase
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Roar"))
        {
            anim.SetTrigger("roar");
        }

        // Ensure the dinosaur isn't stuck in "Attacking" animation while chasing
        anim.SetBool("Attacking", false);
    }

    private void AttackPlayer()
    {
        navAgent.isStopped = true;
        // No movement during attack
        navAgent.velocity = Vector3.zero;

        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(ExecuteRepeatedAttacks());
        }
    }

    private IEnumerator ExecuteRepeatedAttacks()
    {
        isAttacking = true;

        for (int i = 0; i < attackRepetitions; i++)
        {
            anim.SetTrigger("Attacking"); // Trigger attack animation
            Debug.Log("Attacking Player");

            InstantiateBloodScreenEffect();

            // Apply damage to player
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log("Before damage: Player Health = " + playerMovement.currentHealth);
                playerMovement.TakeDamage(damage);
                Debug.Log("After damage: Player Health = " + playerMovement.currentHealth);
            }
            else
            {
                Debug.LogWarning("PlayerMovement script is missing on player.");
            }

            yield return new WaitForSeconds(attackDelay);

            DestroyBloodScreenEffect();
            yield return new WaitForSeconds(attackDelay);
        }

        isAttacking = false;
        navAgent.isStopped = false;
    }

    private void InstantiateBloodScreenEffect()
    {
        if (instantiatedObject == null)
        {
            instantiatedObject = Instantiate(BloodScreenEffect);
        }
        instantiatedObject.SetActive(true);
    }

    private void DestroyBloodScreenEffect()
    {
        if (instantiatedObject != null)
        {
            instantiatedObject.SetActive(false);
        }
    }

    // Health management
    public void TakeDamage(int damageAmount)
    {
        if (currentState == DinosaurState.Dead)
        {
            Debug.Log("Dinosaur is already dead. No damage taken.");
            return;
        }

        health -= damageAmount;
        Debug.Log("Dinosaur Health: " + health);  // Debugging the health decrease

        if (health > 0 && health <= 10)
        {
            anim.SetTrigger("dying"); // Play dying animation if health is low
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        currentState = DinosaurState.Dead;
        anim.SetTrigger("died"); // Trigger the death animation
    }
}
