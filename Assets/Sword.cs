using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] GameObject parentObject;
    [SerializeField] private int damageAmount;
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject == parentObject || collision.gameObject.tag == parentObject.tag) return;

        Health enemyHealth = collision.gameObject.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject == parentObject || other.gameObject.tag == parentObject.tag) return;

        Health enemyHealth = other.gameObject.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            collider.enabled = false;
        }
    }
}
