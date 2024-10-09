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
    }

    private void Update()
    {
        if (isDead || target == null)
        {
            return;
        }

        if(isStunning)
        {
            agent.isStopped = true;
            return;
        }

        timeSinceLastAttack += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Xoay người về phía mục tiêu khi đang tấn công
        if (isAttacking)
        {
            RotateTowardsTarget();
        }

        // Kiểm tra xem target có nằm trong vùng detection hay không
        if (distanceToTarget <= detectionRadius && !isAttacking)
        {
            // Đuổi theo mục tiêu nếu ngoài vùng tấn công
            if (distanceToTarget > agent.stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                animator.SetBool("Moving", agent.velocity.magnitude > 0.1f);
            }
            // Bắt đầu tấn công nếu target trong vùng tấn công
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
        // Nếu đang bị stun, cộng dồn thời gian
        if (isStunning)
        {
            stunDuration += time;
            stunEndTime = Time.time + stunDuration; // Tính toán thời gian kết thúc mới
        }
        else
        {
            stunDuration = time; // Thiết lập thời gian stun
            stunEndTime = Time.time + stunDuration; // Tính toán thời gian kết thúc
            StartCoroutine(StunRoutine());
        }
    }

    private IEnumerator StunRoutine()
    {
        isStunning = true;
        agent.isStopped = true; // Dừng NavMeshAgent khi bị stun
        agent.velocity = Vector3.zero; // Đảm bảo NavMeshAgent không di chuyển
        animator.SetBool("Stun", true);
        animator.SetBool("Moving", false);

        // Chờ đến khi thời gian stun kết thúc
        yield return new WaitUntil(() => Time.time >= stunEndTime);

        isStunning = false;
        agent.isStopped = false; // Cho phép NavMeshAgent hoạt động lại sau khi stun kết thúc
        animator.SetBool("Stun", false);
    }


    private IEnumerator StunRoutine(float stunTime)
    {
        isStunning = true;
        agent.isStopped = true; // Dừng NavMeshAgent khi bị stun
        agent.velocity = Vector3.zero; // Đảm bảo NavMeshAgent không di chuyển
        animator.SetBool("Stun", true);
        animator.SetBool("Moving", false);

        yield return new WaitForSeconds(stunTime);

        isStunning = false;
        agent.isStopped = false; // Cho phép NavMeshAgent hoạt động lại sau khi stun kết thúc
        animator.SetBool("Stun", false);
    }


    private IEnumerator StartAttack()
    {
        if (isStunning) yield break; // Nếu đang bị stun thì không tấn công

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Nếu target ra khỏi vùng tấn công trước khi bắt đầu tấn công
        if (distanceToTarget > agent.stoppingDistance)
        {
            isAttacking = false;
            yield break;
        }

        // Bắt đầu tấn công
        isAttacking = true;
        StopMovement();
        animator.SetBool("IsAttacking", true);

        // Xoay người về phía mục tiêu trong khi tấn công
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

        // Kiểm tra khoảng cách sau lần tấn công đầu tiên
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > agent.stoppingDistance)
        {
            EndAttack();
            yield break; // Nếu target ra khỏi tầm đánh, dừng tấn công
        }

        // Kết thúc tấn công sau một khoảng thời gian
        Invoke(nameof(EndAttack), attackCooldown);
    }


    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        // Tiếp tục đuổi theo mục tiêu nếu còn trong vùng detection
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator.SetBool("Moving", true);
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
        agent.isStopped = true;
        animator.SetBool("Moving", false);
    }

    private void RotateTowardsTarget()
    {
        if (target == null || isStunning) return; 

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Giữ nguyên độ cao
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Quay từ từ về phía mục tiêu
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
