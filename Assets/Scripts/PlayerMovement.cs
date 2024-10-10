using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private PLayerControlls controls;
    private Rigidbody rb;
    private Animator animator;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed = 5f;

    private Vector2 moveInput;
    private Vector3 movementDirection;
    private float lastTimeDash;

    [Header("Attack Info")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Collider swordCollider;
    [SerializeField] private ParticleSystem normalAttackFx;
    private float lastTimeAttack;

    [Header("Ultimate Attack Info")]
    [SerializeField] private float ultimateCooldown = 1f;
    [SerializeField] private Collider ultimateSwordCollider;
    [SerializeField] private ParticleSystem ultimateAttackFx;
    private float lastTimeUltimateAttack;

    [Header("Dash Info")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private ParticleSystem dashFX;

    [Header("Shoot Info")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootCooldown = 1f;
    private float lastTimeShoot;

    [Header("Poke Info")]
    [SerializeField] private float pokeCooldown = 1f;
    private float lastTimePoke;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [Header("Cooldown UI")]
    [SerializeField] private Image normalAttackFillAmountImage;
    [SerializeField] private Image ultimateAttackFillAmountImage;
    [SerializeField] private Image dashFillAmountImage;
    [SerializeField] private Image shootFillAmountImage;
    [SerializeField] private Image pokeAttackFillAmountImage;

    private void Awake()
    {
        controls = new PLayerControlls();
        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
        controls.Character.Attack.performed += context => NormalAttack();
        controls.Character.UltimateAttack.performed += context => UltimateAttack();
        controls.Character.Dash.performed += context => Dash();
        controls.Character.Shoot.performed += context => Fire();
        controls.Character.FAttack.performed += context => Poke();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyMovement();
        ApplyMouseRotation();
        UpdateAnimator();
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    private void UpdateCooldowns()
    {
        // Update Normal Attack Cooldown
        float normalAttackTimePassed = Time.time - lastTimeAttack;
        float normalAttackCooldownProgress = Mathf.Clamp01(normalAttackTimePassed / attackCooldown);
        normalAttackFillAmountImage.fillAmount = 1f - normalAttackCooldownProgress;

        // Update Ultimate Attack Cooldown
        float ultimateAttackTimePassed = Time.time - lastTimeUltimateAttack;
        float ultimateAttackCooldownProgress = Mathf.Clamp01(ultimateAttackTimePassed / ultimateCooldown);
        ultimateAttackFillAmountImage.fillAmount = 1f - ultimateAttackCooldownProgress;

        // Update Shoot Cooldown
        float shootTimePassed = Time.time - lastTimeShoot;
        float shootCooldownProgress = Mathf.Clamp01(shootTimePassed / shootCooldown);
        shootFillAmountImage.fillAmount = 1f - shootCooldownProgress;

        // Update Poke Cooldown
        float pokeTimePassed = Time.time - lastTimePoke;
        float pokeCooldownProgress = Mathf.Clamp01(pokeTimePassed / pokeCooldown);
        pokeAttackFillAmountImage.fillAmount = 1f - pokeCooldownProgress;

        // Update Dash Cooldown
        float dashTimePassed = Time.time - lastTimeDash;
        float dashCooldownProgress = Mathf.Clamp01(dashTimePassed / dashCooldown);
        dashFillAmountImage.fillAmount = 1f - dashCooldownProgress;
    }



    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void ApplyMouseRotation()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Ray rayFromCamera = Camera.main.ScreenPointToRay(mouseScreenPosition);

        if (groundPlane.Raycast(rayFromCamera, out float enter))
        {
            Vector3 hitPoint = rayFromCamera.GetPoint(enter);
            Vector3 directionToLook = (hitPoint - transform.position).normalized;
            directionToLook.y = 0f;
            transform.forward = directionToLook;
        }
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (isGrounded)
        {
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            rb.MovePosition(rb.position + move * walkSpeed * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimator()
    {
        float xVelocity = moveInput.x;
        float zVelocity = moveInput.y;
        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);
    }

    #region Normal Attack
    private void NormalAttack()
    {
        if (Time.time > lastTimeAttack + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastTimeAttack = Time.time;
            StartCoroutine(SwordColliderRoutine(0.5f));
            normalAttackFx.Play();
        }
    }

    private IEnumerator SwordColliderRoutine(float time)
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        swordCollider.enabled = false;
    }
    #endregion

    #region Ultimate Attack
    private void UltimateAttack()
    {
        if (Time.time > lastTimeUltimateAttack + ultimateCooldown)
        {
            animator.SetTrigger("Ultimate");
            lastTimeUltimateAttack = Time.time;
            StartCoroutine(UltimateSwordColliderRoutine(2f));
            ultimateAttackFx.Play();
        }
    }

    private IEnumerator UltimateSwordColliderRoutine(float time)
    {
        ultimateSwordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        ultimateSwordCollider.enabled = false;
    }
    #endregion

    #region Dash
    private void Dash()
    {
        if (Time.time >= lastTimeDash + dashCooldown)
        {
            lastTimeDash = Time.time;
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        Vector3 dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
        dashFX.Play();
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector3.zero;
    }
    #endregion

    #region Shoot
    private void Fire()
    {
        if (Time.time > lastTimeShoot + shootCooldown)
        {
            animator.SetTrigger("Shoot");
            lastTimeShoot = Time.time;

            GameObject newBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.Euler(0, transform.eulerAngles.y - 45, 0));
            newBullet.GetComponent<PlayerProjectile>().Init(this.gameObject, transform.forward);
        }
    }
    #endregion

    #region Poke
    private void Poke()
    {
        if (Time.time > lastTimePoke + pokeCooldown)
        {
            animator.SetTrigger("Poke");
            lastTimePoke = Time.time;
        }
    }
    #endregion

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
