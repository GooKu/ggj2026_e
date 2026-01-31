using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Day HUD")]
    public TextMeshProUGUI totalTipsText;
    public TextMeshProUGUI dayTimerText;

    [Header("Customer UI")]
    public Slider angerSlider;
    public Slider melancholySlider;
    public TextMeshProUGUI customerTimerText;
    public Image faceTimeImage; 

    [Header("Sprites")]
    public Sprite angryFace;
    public Sprite sadFace;
    public Sprite happyFace;

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
        if (dayTimerText != null) dayTimerText.text = string.Format("{0:00}:{1:00}", (int)time / 60, (int)time % 60);
    }

    public void UpdateCustomerEmotions(float angerNormalized, float melancholyNormalized)
    {
        if (angerSlider != null) angerSlider.value = angerNormalized;
        if (melancholySlider != null) melancholySlider.value = melancholyNormalized;

        UpdateFace(angerNormalized, melancholyNormalized);
    }

    public void UpdateCustomerTimer(float time)
    {
        if (customerTimerText != null) customerTimerText.text = time.ToString("F1") + "s";
    }

    private void UpdateFace(float a, float m)
    {
        if (faceTimeImage == null) return;

        // Try to get sprites from current customer
        Sprite currentAngry = angryFace;
        Sprite currentSad = sadFace;
        Sprite currentHappy = happyFace;

        if (GameManager.Instance != null && GameManager.Instance.currentCustomer != null)
        {
            var customer = GameManager.Instance.currentCustomer;
            if (customer.angryFace != null) currentAngry = customer.angryFace;
            if (customer.melancholyFace != null) currentSad = customer.melancholyFace;
            if (customer.happyFace != null) currentHappy = customer.happyFace;
        }

        if (a <= 0 && m <= 0) faceTimeImage.sprite = currentHappy;
        else if (a > m) faceTimeImage.sprite = currentAngry;
        else faceTimeImage.sprite = currentSad;
    }
}
