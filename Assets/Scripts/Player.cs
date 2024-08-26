using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Movement data")]
    public float moveSpeed;
    public float rotationSpeed;

    public float verticalInput;
    public float horizontalInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {

        rb.velocity = new Vector3(0,0, moveSpeed * verticalInput);
        transform.Rotate(0, horizontalInput * rotationSpeed, 0);
    }
}
