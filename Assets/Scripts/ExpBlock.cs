using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBlock : MonoBehaviour
{
    [SerializeField] private int expAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerLevel playerLevel = other.gameObject.GetComponent<PlayerLevel>();

            if (playerLevel != null)
            {
                playerLevel.EarnExp(expAmount);
                Destroy(gameObject); // Optional: destroy the block after use
            }
            else
            {
                Debug.LogWarning("PlayerLevel component missing on Player!");
            }
        }
    }
}
