using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIEnemy : MonoBehaviour
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
    [SerializeField] private float detectionRadius = 10f; // Detection radius around the enemy
    [SerializeField] private float attackDelay = 10f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isDead = false;
    }

    private void Update()
    {
        if (isDead || target == null) 
        {
            
            return;
        } 

        timeSinceLastAttack += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= detectionRadius && !isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("Moving", isMoving);

            if (agent.remainingDistance <= agent.stoppingDistance && timeSinceLastAttack >= attackCooldown)
            {
                StartCoroutine(StartAttack());
            }
        }
        else if (!isAttacking)
        {
            StopMovement();
        }
    }

    private IEnumerator StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        animator.SetBool("Moving", false); // Ensure movement is stopped when attacking
        animator.SetBool("IsAttacking", true);
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);
        EnableWeaponColliders();
        timeSinceLastAttack = 0f;
        Invoke(nameof(EndAttack), attackCooldown);
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        DisableWeaponColliders();
    }

    private void EnableWeaponColliders()
    {
        foreach (Collider weapon in weaponColliders)
        {
            weapon.enabled = true;
        }
    }

    private void DisableWeaponColliders()
    {
        foreach (Collider weapon in weaponColliders)
        {
            weapon.enabled = false;
        }
    }

    private void StopMovement()
    {
        agent.isStopped = true;
        animator.SetBool("Moving", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
