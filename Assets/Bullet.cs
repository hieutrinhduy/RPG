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
        if(player != null)
        {
            fireDir = player.transform.position - gameObject.transform.position;
            rb.velocity = new Vector3(fireDir.x, fireDir.y, fireDir.z).normalized * moveSpeed;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Destroy(gameObject);
    //}
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
