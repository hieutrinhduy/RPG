using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PLayerControlls controls;
    private Rigidbody rb;
    private Animator animator;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2f;

    private Vector2 moveInput;
    private Vector3 movementDirection;

    [Header("Attack Info")]
    [SerializeField] private float attackCountDown = 0.5f;
    private float lastTimeAttack;
    [SerializeField] private Collider swordCollider;
    [SerializeField] private Collider ultimateSwordCollider;
    [SerializeField] private float ultimateAttackCountDown = 2f;
    private float lastTimeUltimateAttack;

    private float lastTimeDash;

    [Header("FX")]
    [SerializeField] private ParticleSystem normalAttackFx;
    [SerializeField] private ParticleSystem ultimateAttackFx;
    [SerializeField] private ParticleSystem dashFX;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

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

            // Sử dụng MovePosition thay vì điều chỉnh velocity trực tiếp
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

    private void NormalAttack()
    {
        if (Time.time > lastTimeAttack)
        {
            animator.SetTrigger("Attack");
            lastTimeAttack = Time.time + attackCountDown;
            StartCoroutine(SwordColliderRoutine(0.5f));
            normalAttackFx.Play(); // Chơi hiệu ứng tấn công thường
        }
    }

    private void UltimateAttack()
    {
        if (Time.time > lastTimeUltimateAttack)
        {
            animator.SetTrigger("Ultimate");
            lastTimeUltimateAttack = Time.time + ultimateAttackCountDown;
            StartCoroutine(UltimateSwordColliderRoutine(2f));
            ultimateAttackFx.Play(); // Chơi hiệu ứng tấn công đặc biệt
        }
    }

    private void Dash()
    {
        if (Time.time >= lastTimeDash)
        {
            lastTimeDash = Time.time + dashCooldown;
            StartCoroutine(DashRoutine());
        }
    }

    private void Fire()
    {
        animator.SetTrigger("Shoot");
    }
    private void Poke()
    {
        animator.SetTrigger("Poke");
    }

    private IEnumerator DashRoutine()
    {
        Vector3 dashDirection = transform.forward;
        //rb.velocity = dashDirection * dashSpeed;
        rb.AddForce(dashDirection* dashSpeed , ForceMode.VelocityChange);
        dashFX.Play(); // Chơi hiệu ứng dash
        yield return new WaitForSeconds(dashDuration);

        // Đặt lại vận tốc sau khi dash
        rb.velocity = Vector3.zero;
    }

    private IEnumerator SwordColliderRoutine(float time)
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        swordCollider.enabled = false;
    }

    private IEnumerator UltimateSwordColliderRoutine(float time)
    {
        ultimateSwordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        ultimateSwordCollider.enabled = false;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
