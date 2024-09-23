using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image hpFillAmount;
    [SerializeField] private Health playerHealth;

    private void Update()
    {
        hpFillAmount.fillAmount = (float) playerHealth.currentHealth / playerHealth.startingHealth;
    }
}
