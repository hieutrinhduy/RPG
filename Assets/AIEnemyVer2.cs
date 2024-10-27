using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIEnemyVer2 : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;
    private Animator animator;
    public float attackCooldown = 2f;
    private float timeSinceLastAttack = 0f;
    private bool isAttacking = false;
    [SerializeField] private Collider[] weaponColliders;
    [SerializeField] private Image HpBar;
    public bool isDead;
    public bool isStunning;
    [SerializeField] private float detectionRadius = 10f; // Detection radius around the enemy
    [SerializeField] private float attackDelay = 1f;

    [Header("Multi Attack Skills")]
    [SerializeField] private bool hasMultipleAttack;
    [SerializeField] private int numberAttackSkill;
    [SerializeField] private float[] attackSkillDelay;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isDead = false;
        isStunning = false;

        // Check if the agent is on a NavMesh at the start
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is not on the NavMesh. Ensure it starts on a NavMesh surface.");
        }
    }

    private void Update()
    {
        if (isDead || target == null)
        {
            return;
        }

        if (isStunning && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            return;
        }

        timeSinceLastAttack += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (isAttacking)
        {
            RotateTowardsTarget();
        }

        if (distanceToTarget <= detectionRadius && !isAttacking)
        {
            if (distanceToTarget > agent.stoppingDistance && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                animator.SetBool("Moving", agent.velocity.magnitude > 0.1f);
            }
            else if (timeSinceLastAttack >= attackCooldown)
            {
                StartCoroutine(StartAttack());
            }
        }
        else if (!isAttacking)
        {
            StopMovement();
        }
    }

    private float stunDuration = 0f;
    private float stunEndTime = 0f;

    public void StartStun(float time)
    {
        if (isStunning)
        {
            stunDuration += time;
            stunEndTime = Time.time + stunDuration;
        }
        else
        {
            stunDuration = time;
            stunEndTime = Time.time + stunDuration;
            StartCoroutine(StunRoutine());
        }
    }

    private IEnumerator StunRoutine()
    {
        isStunning = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("Stun", true);
        animator.SetBool("Moving", false);

        yield return new WaitUntil(() => Time.time >= stunEndTime);

        isStunning = false;
        agent.isStopped = false;
        animator.SetBool("Stun", false);
    }

    private IEnumerator StartAttack()
    {
        if (isStunning) yield break;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > agent.stoppingDistance)
        {
            isAttacking = false;
            yield break;
        }

        isAttacking = true;
        StopMovement();
        animator.SetBool("IsAttacking", true);

        RotateTowardsTarget();

        if (hasMultipleAttack)
        {
            int n = Random.Range(0, numberAttackSkill);
            string attackTrigger = n == 0 ? "Attack" : "Attack" + n;
            animator.SetTrigger(attackTrigger);
        }
        else
        {
            animator.SetTrigger("Attack");
        }

        timeSinceLastAttack = 0f;

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > agent.stoppingDistance)
        {
            EndAttack();
            yield break;
        }

        Invoke(nameof(EndAttack), attackCooldown);
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);

        if (target != null && agent != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= detectionRadius && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                animator.SetBool("Moving", true);
            }
            else
            {
                Debug.LogWarning("Agent cannot resume: Agent is either inactive, not on NavMesh, or target is out of range.");
            }
        }
    }

    public void EnableWeaponColliders()
    {
        foreach (Collider weapon in weaponColliders)
        {
            weapon.enabled = true;
        }
    }

    public void DisableWeaponColliders()
    {
        foreach (Collider weapon in weaponColliders)
        {
            weapon.enabled = false;
        }
    }

    private void StopMovement()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            animator.SetBool("Moving", false);
        }
    }

    private void RotateTowardsTarget()
    {
        if (target == null || isStunning) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
