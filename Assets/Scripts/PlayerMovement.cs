using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PLayerControlls controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Info")]
    [SerializeField] private float WalkSpeed;
    public Vector3 movementDirection;

    private float verticalVelocity;
    private Vector2 moveInput;
    private Vector2 aimInput;

    [Header("Aim Info")]
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;

    [Header("Attack")]
    [SerializeField] private float attackCountDown;
    private float lastTimeAttack;
    [SerializeField] private Collider swordCollider;
    [SerializeField] private Collider ultimateSwordCollider;

    [SerializeField] private float ultimateAttackCountDown;
    private float lastTimeUltimateAttack;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    private Vector3 dashDirection;
    [SerializeField] private float dashCountDown; // Time between dashes
    private float lastTimeDash;

    [Header("FX")]
    [SerializeField] private ParticleSystem normalAttackFx;
    [SerializeField] private ParticleSystem ultimateAttackFx;
    [SerializeField] private ParticleSystem dashFX;
    private Health health;
    private void Awake()
    {
        controls = new PLayerControlls();
        controls.Character.Attack.performed += context => NormalAttack();
        controls.Character.UltimateAttack.performed += context => UltimateAttack();
        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;
        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
        controls.Character.Dash.performed += context => Dash();
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if(health.IsDead) return;
        ApplyMovement();
        ApplyMouseRotation();
        AnimatorControllers();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            health.currentHealth += health.startingHealth / 4;
            if(health.currentHealth > health.startingHealth)
            {
                health.currentHealth = health.startingHealth;
            }
        }
    }
    private void CheckIsDead()
    {

    }
    private void ApplyMouseRotation()
    {
        // Lấy vị trí của chuột trên màn hình (Screen Space)
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Tạo mặt phẳng trên trục XZ (nơi nhân vật di chuyển)
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        // Tạo tia từ camera đi qua vị trí của chuột
        Ray rayFromCamera = Camera.main.ScreenPointToRay(mouseScreenPosition);

        // Kiểm tra xem tia có cắt qua mặt phẳng không
        if (groundPlane.Raycast(rayFromCamera, out float enter))
        {
            // Tính vị trí mà tia cắt mặt phẳng
            Vector3 hitPoint = rayFromCamera.GetPoint(enter);

            // Tính hướng từ vị trí nhân vật đến vị trí cắt trên mặt phẳng
            Vector3 directionToLook = hitPoint - transform.position;

            // Đảm bảo hướng nhìn chỉ nằm trên mặt phẳng XZ (loại bỏ trục Y)
            directionToLook.y = 0f;

            // Chuẩn hóa hướng nhìn
            directionToLook.Normalize();

            // Xoay nhân vật về hướng của chuột
            transform.forward = directionToLook;
        }
    }



    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);
        animator.SetFloat("xVelocity", xVelocity, .05f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .05f, Time.deltaTime);
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * WalkSpeed);
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void NormalAttack()
    {
        if (Time.time > lastTimeAttack)
        {
            animator.SetTrigger("Attack");
            lastTimeAttack = Time.time + attackCountDown;
            StartCoroutine(SwordColliderRoutine(0.5f));
            normalAttackFx.Play();
        }
    }

    private void UltimateAttack()
    {
        if (Time.time > lastTimeUltimateAttack)
        {
            ultimateAttackFx.Play();
            animator.SetTrigger("Ultimate");
            lastTimeUltimateAttack = Time.time + ultimateAttackCountDown;
            StartCoroutine(UltimateSwordColliderRoutine(2f));
        }
    }

    IEnumerator SwordColliderRoutine(float time)
    {
        swordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        swordCollider.enabled = false;
    }
    IEnumerator UltimateSwordColliderRoutine(float time)
    {
        ultimateSwordCollider.enabled = true;
        yield return new WaitForSeconds(time);
        ultimateSwordCollider.enabled = false;
    }
    private void Dash()
    {
        if (Time.time > lastTimeDash)
        {
            Debug.Log("Dash");
            lastTimeDash = Time.time + dashCountDown; // Set the next allowed dash time
            StartCoroutine(DashRoutine());
        }
        else
        {
            Debug.Log("Dash on cooldown!");
        }
    }

    IEnumerator DashRoutine()
    {
        characterController.detectCollisions = false;
        float startTime = Time.time;

        // Sử dụng hướng của nhân vật để dash
        dashDirection = transform.forward;

        dashFX.Play();
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
        characterController.detectCollisions = true;
    }

}
