using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] public GameObject parentObject;
    [SerializeField] private int damageAmount;
    [SerializeField] private int knockbackThurst;
    private Collider collider;
    public bool dontHaveTurnOffCollider;
    public bool canStun;
    [SerializeField] private float stunTime;
    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject == parentObject || collision.gameObject.tag == parentObject.tag) return;

        Health enemyHealth = collision.gameObject.GetComponent<Health>();
        KnockBack enemyKnockBack = collision.gameObject.GetComponent<KnockBack>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }
        if (enemyKnockBack != null && knockbackThurst != null)
        {
            Vector3 hitPosition = transform.position; // Vị trí va chạm chính là vị trí của thanh kiếm
            enemyKnockBack.GetKnockBack(hitPosition, knockbackThurst);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject == parentObject || other.gameObject.tag == parentObject.tag) return;

        Health enemyHealth = other.gameObject.GetComponent<Health>();
        KnockBack enemyKnockBack = other.gameObject.GetComponent<KnockBack>();
        AIEnemyVer2 enemyVer2 = other.gameObject.GetComponent<AIEnemyVer2>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            if (!dontHaveTurnOffCollider)
            {
                collider.enabled = false;
            }
        }
        if (canStun)
        {
            if(enemyVer2 != null)
            {
                enemyVer2.StartStun(stunTime);
                return;
            }
        }
        if (enemyKnockBack != null && knockbackThurst != null)
        {
            Vector3 hitPosition = transform.position; // Vị trí va chạm chính là vị trí của thanh kiếm
            enemyKnockBack.GetKnockBack(hitPosition, knockbackThurst);
        }
    }
}
