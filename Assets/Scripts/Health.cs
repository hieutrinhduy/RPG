using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Diagnostics.Tracing;

public class Health : MonoBehaviour
{

    public static Action<Health> OnDeath;
    public bool IsDead {  get; private set; }
    public int startingHealth = 3;

    private Health health;

    public int currentHealth;

    [SerializeField] private Image HpBarFillAmount;
    [SerializeField] private GameObject HpBar;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private float whiteFlashTime;
    [SerializeField] private SkinnedMeshRenderer[] playerVisuals;
    [SerializeField] private MeshRenderer[] playerMeshVisuals;
    [SerializeField] private AIEnemy aIEnemy;
    Animator animator;
    [SerializeField] private GameObject dropedExpPrefab;
    [SerializeField] private GameObject BloodSplatterFx;

    private Bomb bomb;



    private void Start()
    {
        ResetHealth();
        animator = GetComponent<Animator>();
        aIEnemy = GetComponent<AIEnemy>();
        bomb= GetComponent<Bomb>();
        IsDead = false;
    }
    private void Awake()
    {
        health = GetComponent<Health>();
    }
    private void Update()
    {
        if (HpBarFillAmount != null)
        {
            HpBarFillAmount.fillAmount = (float)currentHealth/startingHealth;
        }
    }
    public void ResetHealth()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("- " + amount);
        StartCoroutine(WhiteFlashRountine(whiteFlashTime));
        if(BloodSplatterFx != null)
        {
            Instantiate(BloodSplatterFx, transform.position, Quaternion.identity);
        }
        if (currentHealth <= 0)
        {
            animator.ResetTrigger("Hit"); // Reset any "Hit" triggers
            animator.SetTrigger("Die");
            IsDead = true;
            if(bomb != null)
            {
                bomb.Explode();
            }
            if (HpBar != null)
            {
                HpBar.transform.parent.gameObject.SetActive(false);
                HpBar.gameObject.SetActive(false);
            }
            if(aIEnemy != null)
            {
                aIEnemy.isDead = true;
            }
            if(HpBar != null)
            {
                HpBar.SetActive(false);
            }
            OnDeath?.Invoke(this);
            Destroy(gameObject, 1f);
            if(dropedExpPrefab != null)
            {
                Instantiate(dropedExpPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {

            animator.SetTrigger("Hit");
        }
    }

    IEnumerator WhiteFlashRountine(float time)
    {
        foreach (SkinnedMeshRenderer player in playerVisuals)
        {
            player.material = whiteMaterial;
        }
        foreach (MeshRenderer player in playerMeshVisuals)
        {
            player.material = whiteMaterial;
        }
        yield return new WaitForSeconds(time);
        foreach (SkinnedMeshRenderer player in playerVisuals)
        {
            player.material = defaultMaterial;
        }
        foreach (MeshRenderer player in playerMeshVisuals)
        {
            player.material = defaultMaterial;
        }
    }


    public void TakeDamage(Vector2 DamageSourceDir, int damageAmout, float knockBackThurst)
    {
        health.TakeDamage(damageAmout);
    }

    public void TakeHit()
    {
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Health"))
        {
            currentHealth += 30;
            if (currentHealth > startingHealth)
            {
                currentHealth = startingHealth;
            }
            Destroy(collision.gameObject);
        }
    }

}