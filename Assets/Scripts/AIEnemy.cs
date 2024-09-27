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

    [Header("Multi Attack Skills")]
    [SerializeField] private bool hasMultipleAttack;
    [SerializeField] private int numberAttackSkill;
    [SerializeField] private float[] attackSkillDelay;

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
            //StopMovement();
        }
    }

    private IEnumerator StartAttack()
    {
        // Kiểm tra khoảng cách trước khi bắt đầu tấn công
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > detectionRadius)
        {
            // Nếu target đã ra khỏi tầm đánh, quay lại đuổi theo
            isAttacking = false;
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetBool("Moving", true);
            yield break; // Kết thúc tấn công
        }

        // Bắt đầu tấn công
        isAttacking = true;
        agent.isStopped = true;
        animator.SetBool("Moving", false); // Ensure movement is stopped when attacking
        animator.SetBool("IsAttacking", true);

        if (hasMultipleAttack)
        {
            int n = Random.Range(0, numberAttackSkill);
            string attackTrigger = n == 0 ? "Attack" : "Attack" + n;
            animator.SetTrigger(attackTrigger);
            yield return new WaitForSeconds(attackSkillDelay[n]);
        }
        else
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDelay);
        }

        EnableWeaponColliders();
        timeSinceLastAttack = 0f;

        // Kiểm tra khoảng cách sau lần tấn công đầu tiên
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > detectionRadius)
        {
            EndAttack();
            yield break; // Nếu target ra khỏi tầm đánh, dừng tấn công
        }

        // Kết thúc đợt tấn công
        Invoke(nameof(EndAttack), attackCooldown);
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        DisableWeaponColliders();

        // Kiểm tra khoảng cách sau khi kết thúc tấn công
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Nếu target đã ra khỏi tầm đánh, enemy tiếp tục đuổi theo
        if (distanceToTarget > detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetBool("Moving", true);
        }
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
