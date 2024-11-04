using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Boss : MonoBehaviour
{
    public GameObject Player;
    public Transform LockPlayerPos;
    public Transform[] ClonePos;
    public GameObject ClonePrefab;
    private Shoot shoot;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject HpBarPanel;
    public enum BossState
    {
        idle,
        fireAttack,
        cloneSkill,
        meteoritoRain,
        defeated,
        teleporting
    }
    private BossState state;
    private Animator animator;
    private Health health;
    public float rotationSpeed = 2f;  // Speed at which the boss rotates towards the Player

    // Start is called before the first frame update
    void Start()
    {
        shoot = gameObject.GetComponent<Shoot>();
        animator = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<Health>();
        animator.Play("Idle");
        state = BossState.idle;
        StartCoroutine(cloneSkill());
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsPlayer();
    }

    private void RotateTowardsPlayer()
    {
        if (Player == null) return;
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    //Fire Ball
    IEnumerator ShootFireBallRoutine()
    {
        for (int i = 0; i < 10; i++)
        {
            ShootFireBall();
            yield return new WaitForSeconds(1f);
        }
    }

    private void ShootFireBall()
    {
        animator.Play("Attack01");
    }

    //Active Sheild
    private void ActiveShield()
    {
        health.enabled = false;
    }


    //CloneSkill
    private IEnumerator cloneSkill()
    {
        Player.transform.position = LockPlayerPos.position;
        ChangeCameraPos();
        for (int i= 0; i < ClonePos.Length; i++)
        {
            GameObject newClone = Instantiate(ClonePrefab, ClonePos[i].position, Quaternion.identity);
            newClone.GetComponent<BossClone>().Player = this.Player;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(100f);
        ResetCameraPos();
    }
    
    Vector2 DefaultCameraPos;
    float DefaultCameraDistance;
    float DefaultCameraRotationX;
    private void ChangeCameraPos()
    {
        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        HpBarPanel.gameObject.SetActive(false);
        if (framingTransposer != null)
        {
            //DefaultCameraPos = new Vector2(framingTransposer.m_ScreenX, framingTransposer.m_ScreenY);
            DefaultCameraDistance = framingTransposer.m_CameraDistance;
            DefaultCameraRotationX = transform.rotation.eulerAngles.x;
            framingTransposer.m_CameraDistance = 19.3f;
            transform.rotation = Quaternion.Euler(61.99f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
    private void ResetCameraPos()
    {
        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        HpBarPanel.gameObject.SetActive(true);
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = DefaultCameraDistance;
            transform.rotation = Quaternion.Euler(DefaultCameraRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
