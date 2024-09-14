using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    public Transform target;
    public Camera mainCamera; // Reference to the camera

    private void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        if (mainCamera != null)
        {
            target = mainCamera.transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Rotate the health bar to face the camera
            transform.LookAt(target);

            // Optionally, adjust rotation to prevent the health bar from being flipped
            Vector3 dirToCamera = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(dirToCamera);

            // Ensure the health bar doesn't flip upside down
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
