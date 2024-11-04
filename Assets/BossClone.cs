using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossClone : MonoBehaviour
{
    public GameObject Player;
    private Animator animator;
    public float rotationSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(ShootFireBallRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsPlayer();
    }

    //Fire Ball
    IEnumerator ShootFireBallRoutine()
    {
        int n = Random.RandomRange(1, 5);
        yield return new WaitForSeconds(n);
        for (int i = 0; i < 10; i++)
        {
            ShootFireBall();
            Debug.Log("Shoot Time: " + i);
            yield return new WaitForSeconds(Random.RandomRange(2, 5));
        }
    }
    private void RotateTowardsPlayer()
    {
        if (Player == null) return;
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ShootFireBall()
    {
        animator.Play("Attack01");
    }
}
