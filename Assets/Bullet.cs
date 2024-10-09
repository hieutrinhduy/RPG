using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject player;
    private Vector3 fireDir;
    [SerializeField] private float moveSpeed;

    private Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        if (player != null)
        {
            fireDir = player.transform.position - gameObject.transform.position;
            rb.velocity = new Vector3(fireDir.x, fireDir.y, fireDir.z).normalized * moveSpeed;
        }
        else
        {
            Destroy(gameObject);
        }

        // Automatically destroy the bullet after 3 seconds
        Destroy(gameObject, 8f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) return;
        Destroy(gameObject);
    }
}
