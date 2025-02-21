using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DinosaurAI : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public CapsuleCollider capsuleCollider;
    public enum DinosaurState { Idle, Chase, Attack, Dead }
    public DinosaurState currentState = DinosaurState.Idle;
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
            Debug.LogWarning("Player not found");
        }

        navAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        lastAttackTime = -attackCooldown;

        // Load BloodScreenEffect from Resources if not assigned
        if (BloodScreenEffect == null)
        {
            BloodScreenEffect = Resources.Load<GameObject>("BloodScreenEffect");
            if (BloodScreenEffect == null)
            {
                Debug.LogWarning("BloodScreenEffect prefab not found in Resources.");
            }
        }
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
            case DinosaurState.Idle:
                if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
                {
                    currentState = DinosaurState.Chase;
                }
                break;

            case DinosaurState.Chase:
                navAgent.SetDestination(player.position);
                anim.SetBool("walking", false);
                anim.SetBool("Running", true);

                // Trigger roar animation once in chase mode
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Roar"))
                {
                    anim.SetTrigger("roar");
                }

                anim.SetBool("Attacking", false);

                if (Vector3.Distance(transform.position, player.position) <= attackDistance)
                {
                    anim.SetBool("Running", false);
                    currentState = DinosaurState.Attack;
                }
                break;

            case DinosaurState.Attack:
                navAgent.isStopped = true;
                navAgent.velocity = Vector3.zero;

                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(PerformMultipleAttacks(attackRepetitions));
                }

                if (Vector3.Distance(transform.position, player.position) > attackDistance)
                {
                    navAgent.isStopped = false;
                    currentState = DinosaurState.Chase;
                }
                break;

            case DinosaurState.Dead:
                navAgent.enabled = false;
                capsuleCollider.enabled = false;
                enabled = false;
                anim.SetBool("walking", false);
                anim.SetBool("Running", false);
                anim.SetBool("Attacking", false);
                anim.SetTrigger("died");
                break;
        }
    }

    private IEnumerator PerformMultipleAttacks(int repetitions)
    {
        isAttacking = true;

        for (int i = 0; i < repetitions; i++)
        {
            // Trigger the attack animation
            anim.SetTrigger("Attacking");

            // Show blood effect when the attack animation triggers
            InstantiateBloodScreenEffect();

            // Apply damage to player
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
                Debug.Log("Attacking Player");
            }

            // Wait for the duration of the animation or delay between attacks
            yield return new WaitForSeconds(attackDelay);

            // Hide blood effect after each attack animation completes
            DestroyBloodScreenEffect();

            // Additional delay between attacks
            yield return new WaitForSeconds(attackDelay);
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    private void InstantiateBloodScreenEffect()
    {
        if (instantiatedObject == null && BloodScreenEffect != null)
        {
            instantiatedObject = Instantiate(BloodScreenEffect);
        }
        if (instantiatedObject != null)
        {
            instantiatedObject.SetActive(true);
        }
    }

    private void DestroyBloodScreenEffect()
    {
        if (instantiatedObject != null)
        {
            instantiatedObject.SetActive(false);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentState == DinosaurState.Dead)
        {
            Debug.Log("Dinosaur is already dead. No damage taken.");
            return;
        }

        health -= damageAmount;

        if (health > 0 && health <= 10)
        {
            navAgent.enabled = false;
            anim.SetTrigger("dying");
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
        navAgent.enabled = false;
        capsuleCollider.enabled = false;
        anim.SetTrigger("died");
    }
}
