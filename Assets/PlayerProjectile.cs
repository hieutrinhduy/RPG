using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody rb;
    private Vector3 fireDir;
    private GameObject parent;
    Sword sword;
    public void Init(GameObject parent,Vector3 fireDir)
    {
        this.fireDir = fireDir;
        this.parent = parent;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sword = GetComponent<Sword>();
        sword.parentObject = parent;
        rb.velocity = new Vector3(fireDir.x, fireDir.y, fireDir.z).normalized * moveSpeed;
        Destroy(gameObject, 8f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) return;
        //Destroy(gameObject);
    }
}
