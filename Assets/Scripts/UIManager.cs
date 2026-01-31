using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Day HUD")]
    public Text totalTipsText;
    public Image dayTimerFillImage;

    [Header("Customer UI")]
    public Image angerImage;
    public Image melancholyImage;
    public Text customerTimerText;
    public Image faceTimeImage; 

    private void Start()
    {
        // Connect to GameManager events in a robust implementation
        // For now, assume assignments in inspector
    }

    public void UpdateTips(int amount)
    {
        if (totalTipsText != null) totalTipsText.text = "Tips: " + amount;
    }

    public void UpdateDayTimer(float time)
    {
        if (dayTimerFillImage != null && GameManager.Instance != null)
        {
            float progress = 1f - (time / GameManager.Instance.totalDaySeconds);
            dayTimerFillImage.fillAmount = Mathf.Clamp01(progress);
        }
    }

    public void UpdateCustomerEmotions(float angerNormalized, float melancholyNormalized)
    {
        if (angerImage != null) angerImage.fillAmount = angerNormalized;
        if (melancholyImage != null) melancholyImage.fillAmount = melancholyNormalized;
    }

    public void UpdateCustomerTimer(float time)
    {
        if (customerTimerText != null) customerTimerText.text = time.ToString("F1") + "s";
    }

    public void UpdateFaceSprite(Sprite sprite)
    {
        if (faceTimeImage != null) faceTimeImage.sprite = sprite;
    }
}
