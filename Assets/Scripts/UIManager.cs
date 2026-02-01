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
    public GameObject dialoguePrefab;
    public List<Transform> dialogueSpawnPoints;

    private int lastDialogueSpawnIndex = -1;

    private void Start()
    {
        // Connect to GameManager events in a robust implementation
        // For now, assume assignments in inspector
    }

    public void UpdateTips(int amount)
    {
        if (totalTipsText != null) totalTipsText.text = amount.ToString();
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

    public void ShowDialogue(string text)
    {
        if (dialoguePrefab == null || dialogueSpawnPoints == null || dialogueSpawnPoints.Count == 0 || string.IsNullOrEmpty(text)) return;

        // 隨機選取生成點
        int sIndex = 0;
        if (dialogueSpawnPoints.Count > 1)
        {
            do { sIndex = Random.Range(0, dialogueSpawnPoints.Count); } 
            while (sIndex == lastDialogueSpawnIndex);
        }
        lastDialogueSpawnIndex = sIndex;

        // 直接在選定的生成點底下生成，並重置局部座標
        GameObject dialogueObj = Instantiate(dialoguePrefab, dialogueSpawnPoints[sIndex]);
        dialogueObj.transform.localPosition = Vector3.zero;

        // 取得 Text 組件並設置文字
        Text t = dialogueObj.GetComponentInChildren<Text>();
        if (t != null) t.text = text;
        
        // 1.5 秒後自動銷毀
        Destroy(dialogueObj, 1.5f);
    }

    public void ClearDialogues()
    {
        if (dialogueSpawnPoints == null) return;
        foreach (Transform spawnPoint in dialogueSpawnPoints)
        {
            if (spawnPoint == null) continue;
            foreach (Transform child in spawnPoint)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
