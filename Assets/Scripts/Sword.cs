using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Sword : MonoBehaviour
{
    [SerializeField] public GameObject parentObject;
    [SerializeField] private int damageAmount;
    [SerializeField] private int knockbackThurst;
    private Collider collider;
    public bool dontHaveTurnOffCollider;
    public bool canStun;
    public bool isEnemy;
    [SerializeField] private float stunTime;

    private CinemachineImpulseSource cinemachineImpulseSource;
    private void Start()
    {
        collider = GetComponent<Collider>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(isEnemy)
    //    {
    //        if (collision.gameObject.CompareTag("Enemy")) return;
    //    }
    //    if (collision.gameObject == parentObject || collision.gameObject.tag == parentObject.tag) return;
    //    Debug.Log(collision.gameObject.name);

    //    Health enemyHealth = collision.gameObject.GetComponent<Health>();
    //    KnockBack enemyKnockBack = collision.gameObject.GetComponent<KnockBack>();
    //    if (enemyHealth != null)
    //    {
    //        enemyHealth.TakeDamage(damageAmount);
    //    }
    //    if (enemyKnockBack != null && knockbackThurst != null)
    //    {
    //        Vector3 hitPosition = transform.position; // Vị trí va chạm chính là vị trí của thanh kiếm
    //        enemyKnockBack.GetKnockBack(hitPosition, knockbackThurst);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == parentObject || other.gameObject.tag == parentObject.tag) return;
        Debug.Log(other.gameObject.name);
        if (isEnemy)
        {
            if (other.gameObject.CompareTag("Enemy")) return;
        }

        Health enemyHealth = other.gameObject.GetComponent<Health>();
        KnockBack enemyKnockBack = other.gameObject.GetComponent<KnockBack>();
        AIEnemyVer2 enemyVer2 = other.gameObject.GetComponent<AIEnemyVer2>();
        PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
        if (enemyHealth != null)
        {
            if(cinemachineImpulseSource != null)
            {
                cinemachineImpulseSource.GenerateImpulse();
            }
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
                Debug.Log("Stun enemy");
                enemyVer2.StartStun(stunTime);
                return;
            }
            if (playerMovement != null)
            {
                Debug.Log("Stun player");
                playerMovement.StartStun(stunTime);
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
