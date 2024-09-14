using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private Image ExpBar;
    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text ExpProgress;

    [SerializeField] private float level;
    private float currentExp;
    [SerializeField] private float neededExpNextLevel;
    private float remainingExp;

    private void Update()
    {
        ExpBar.fillAmount = (float)currentExp/neededExpNextLevel;
        ExpProgress.text = currentExp.ToString() + " / " + neededExpNextLevel.ToString();
    }

    public void EarnExp(int claimedExp)
    {
        currentExp += claimedExp;
        Debug.Log("Claim EXP");
        if(currentExp >= neededExpNextLevel)
        {
            remainingExp= currentExp - neededExpNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level += 1;
        currentExp = 0 + remainingExp;
        remainingExp = 0;
        neededExpNextLevel += 10;
        LevelText.text = level.ToString();
    }
}
