using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    public bool IsRedMode;
    public GameObject Player;
    public Transform LockPlayerPos;
    public Transform[] ClonePos;
    public GameObject ClonePrefab;
    private Shoot shoot;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject HpBarPanel;
    public GameObject LockFx;
    public GameObject ShieldFx;
    public GameObject MeteorPrefab;
    public GameObject IceAttackPrefab;
    public Transform[] IceAttackSpawnPos;
    public GameObject IceSkillHitBox;

    private List<GameObject> cloneList = new List<GameObject>();
    public enum BossState
    {
        idle,
        fireAttack,
        cloneSkill,
        meteoritoRain,
        defeated,
        teleporting,
        iceAttack
    }

    private void OnDestroy()
    {

    }

    private BossState state = BossState.idle;
    private Animator animator;
    private Health health;
    public float rotationSpeed = 2f;  // Speed at which the boss rotates towards the Player

    // Start is called before the first frame update
    void Start()
    {
        shoot = gameObject.GetComponent<Shoot>();
        animator = gameObject.GetComponent<Animator>();
        health = gameObject.GetComponent<Health>();
        StartCoroutine(BossAction());
    }

    private IEnumerator BossAction()
    {
        while (health.currentHealth > 0)
        {
            if (IsRedMode)
            {
                yield return new WaitForSeconds(0.1f);

                SetBossState(BossState.iceAttack);
                yield return StartCoroutine(IceAttackRoutine());

                SetBossState(BossState.meteoritoRain);
                yield return StartCoroutine(MeteorRoutine());

                // Fire Attack
                SetBossState(BossState.fireAttack);
                yield return StartCoroutine(ShootFireBallRoutine());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(3f);

                // Clone Skill
                SetBossState(BossState.cloneSkill);
                yield return StartCoroutine(CloneSkill());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(5f);

                SetBossState(BossState.iceAttack);
                yield return StartCoroutine(IceAttackRoutine());

                SetBossState(BossState.teleporting);
                yield return StartCoroutine(TeleportRoutine());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(5f);

                SetBossState(BossState.meteoritoRain);
                yield return StartCoroutine(MeteorRoutine());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);

                // Fire Attack
                SetBossState(BossState.fireAttack);
                yield return StartCoroutine(ShootFireBallRoutine());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(3f);

                // Clone Skill
                SetBossState(BossState.cloneSkill);
                yield return StartCoroutine(CloneSkill());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(5f);

                SetBossState(BossState.teleporting);
                yield return StartCoroutine(TeleportRoutine());

                SetBossState(BossState.idle);
                yield return new WaitForSeconds(5f);

                SetBossState(BossState.meteoritoRain);
                yield return StartCoroutine(MeteorRoutine());
            }
        }
    }

    private void SetBossState(BossState newState)
    {
        if (state != newState)
        {
            state = newState;
        }
    }

    void Update()
    {
        if (health.IsDead && !IsRedMode)
        {
            SceneManager.LoadScene("CutSceneStage1.3(BossTransform)");
        }
        if (state != BossState.defeated && state != BossState.teleporting)
        {
            RotateTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        if (Player == null) return;
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Fire Ball Attack
    private IEnumerator ShootFireBallRoutine()
    {
        if (state != BossState.fireAttack) yield break;
        if(IsRedMode)
        {
            ActiveShield();
            for (int i = 0; i < 10; i++)
            {
                if (state != BossState.fireAttack) yield break;
                ShootFireBall();
                yield return new WaitForSeconds(0.5f);
            }
            DeactiveShield();
        }
        else
        {
            ActiveShield();
            for (int i = 0; i < 10; i++)
            {
                if (state != BossState.fireAttack) yield break;
                ShootFireBall();
                yield return new WaitForSeconds(0.5f);
            }
            DeactiveShield();
        }
    }

    private void ShootFireBall()
    {
        animator.Play("Attack01");
    }

    // Shield Activation
    private void ActiveShield()
    {
        ShieldFx.SetActive(true);
    }

    private void DeactiveShield()
    {
        ShieldFx.SetActive(false);
    }

    // Clone Skill
    private IEnumerator CloneSkill()
    {
        if (state != BossState.cloneSkill) yield break;

        LockFx.SetActive(true);
        ActiveShield();
        Player.transform.position = LockPlayerPos.position;
        ChangeCameraPos();

        for (int i = 0; i < ClonePos.Length; i++)
        {
            if (state != BossState.cloneSkill) yield break;

            GameObject newClone = Instantiate(ClonePrefab, ClonePos[i].position, Quaternion.identity);
            newClone.GetComponent<BossClone>().Player = this.Player;
            cloneList.Add(newClone);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(15f);

        foreach (var clone in cloneList)
        {
            Destroy(clone);
        }
        cloneList.Clear();

        ResetCameraPos();
        LockFx.SetActive(false);
        DeactiveShield();
    }

    // Camera Control Methods
    Vector2 DefaultCameraPos;
    float DefaultCameraDistance;
    float DefaultCameraRotationX;

    private void ChangeCameraPos()
    {
        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        HpBarPanel.SetActive(false);
        if (framingTransposer != null)
        {
            DefaultCameraDistance = framingTransposer.m_CameraDistance;
            DefaultCameraRotationX = transform.rotation.eulerAngles.x;
            framingTransposer.m_CameraDistance = 19.3f;
            transform.rotation = Quaternion.Euler(61.99f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    private void ResetCameraPos()
    {
        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        HpBarPanel.SetActive(true);
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = DefaultCameraDistance;
            transform.rotation = Quaternion.Euler(DefaultCameraRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    //TeleportSkill
    Vector3 defaultPosition;
    private IEnumerator TeleportRoutine()
    {

        Vector3 defaultPosition = gameObject.transform.position;

        for (int i = 0; i < 10; i++)
        {
            int n = Random.Range(0, ClonePos.Length);
            gameObject.transform.position = ClonePos[n].position;
            RotateInstantlyTowardsPlayer();
            ShootFireBall();

            yield return new WaitForSeconds(0.8f);
        }
        gameObject.transform.position = defaultPosition;
        RotateInstantlyTowardsPlayer();
    }

    private void RotateInstantlyTowardsPlayer()
    {
        if (Player == null) return;
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
    }

    private IEnumerator MeteorRoutine()
    {
        animator.Play("Attack02");
        for(int i = 0; i< 10;i++)
        {
            Vector3 spawnPos = Player.transform.position;
            Instantiate(MeteorPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator IceAttackRoutine()
    {
        for(int i = 0; i < 3; i++)
        {
            ActiveShield();
            int n= Random.Range(0, IceAttackSpawnPos.Length);
            GameObject iceAttackRange = Instantiate(IceSkillHitBox, IceAttackSpawnPos[n].transform.position, Quaternion.Euler(-90,0,0));
            yield return new WaitForSeconds(1f);
            Destroy( iceAttackRange );
            GameObject iceAttack = Instantiate(IceAttackPrefab, IceAttackSpawnPos[n].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.6f);
            Destroy(iceAttack);
            DeactiveShield();
        }
    }
}