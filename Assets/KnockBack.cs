using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnockBack : MonoBehaviour
{
    private Vector3 _hitDirection;
    private Rigidbody _rb;
    private bool isKnockback = false;  // Cờ kiểm soát knockback
    private float knockbackDuration = 0.5f;  // Thời gian bị knockback

    private NavMeshAgent NavMeshAgent;
    private float oldSpeed;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void GetKnockBack(Vector3 hitPosition, int knockBackThrust)
    {
        if (isKnockback) return;  // Nếu đang bị knockback, không thể knockback lần nữa

        // Bật cờ knockback
        isKnockback = true;

        // Tính hướng của cú đánh, từ vị trí bị đánh đến vị trí va chạm
        _hitDirection = (transform.position - hitPosition).normalized;

        if (_rb != null)
        {
            if(NavMeshAgent != null)
            {
                oldSpeed = NavMeshAgent.speed;
                NavMeshAgent.speed = 0;
            }
            // Thêm lực knockback với chế độ lực tức thời
            _rb.isKinematic = false;
            _rb.AddForce(_hitDirection * knockBackThrust, ForceMode.Impulse);
            Debug.Log("HitDir: " + _hitDirection);
            Debug.Log("KnockBack");

            // Bắt đầu coroutine để reset velocity sau khi knockback kết thúc
            StartCoroutine(ResetKnockback());
        }
    }

    private IEnumerator ResetKnockback()
    {
        // Chờ trong thời gian knockbackDuration
        yield return new WaitForSeconds(knockbackDuration);

        // Reset velocity sau thời gian knockback
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
        }
        if (NavMeshAgent != null)
        {
            NavMeshAgent.speed = oldSpeed;
        }
        // Tắt cờ knockback để cho phép knockback lần nữa
        isKnockback = false;
    }
}
