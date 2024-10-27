using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Bomb : MonoBehaviour
{
    private Health health;
    public float expForce, radius;
    public GameObject explodeEffect;
    public int damage;
    bool exploded;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private void Start()
    {
        health = GetComponent<Health>();
        exploded = false;
        cinemachineImpulseSource= GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {

    }
 
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        //foreach (Collider collider in colliders)
        //{
        //    if (collider.GetComponent<Rigidbody>() != null)
        //    {
        //        collider.GetComponent<Rigidbody>().AddExplosionForce(expForce, transform.position, radius);
        //        collider.GetComponent<Health>().TakeDamage(damage);
        //    }
        //}
        exploded = true;
        cinemachineImpulseSource.GenerateImpulse();
        Debug.Log("Explode");
        //Destroy(gameObject);
    }

    // Vẽ Gizmo để thấy bán kính vụ nổ
    private void OnDrawGizmosSelected()
    {
        // Chọn màu cho Gizmo
        Gizmos.color = Color.red;
        // Vẽ một hình cầu biểu thị bán kính của vụ nổ
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
