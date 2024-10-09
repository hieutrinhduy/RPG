using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private AIEnemy AIEnemy;
    public float expForce, radius;
    public GameObject explodeEffect;
    public int damage;
    bool exploded;
    private void Start()
    {
        AIEnemy = GetComponent<AIEnemy>();
        exploded = false;
    }

    private void Update()
    {
        if (AIEnemy != null)
        {
            if (AIEnemy.isDead && !exploded)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                Instantiate(explodeEffect, transform.position, Quaternion.identity);
                foreach (Collider collider in colliders)
                {
                    if (collider.GetComponent<Rigidbody>() != null)
                    {
                        collider.GetComponent<Rigidbody>().AddExplosionForce(expForce, transform.position, radius);
                        collider.GetComponent<Health>().TakeDamage(damage);
                    }
                }
                exploded = true;
                //Destroy(gameObject);
            }
        }
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
