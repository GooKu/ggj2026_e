using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text tipsText;
    public Text perfectServicesText;
    public Text reviewText;

    [Header("Review Pool")]
    public string[] perfectReviews = new string[] {
        "手法太棒了！就像在雲端一樣！",
        "這是我按過最舒服的一次，技術驚人！",
        "完全恢復了精神，師傅真是高手！"
    };

    public string[] normalReviews = new string[] {
        "還不錯，感覺放鬆了不少。",
        "手法中規中矩，之後會考慮再來。",
        "謝謝師傅，感覺好多了。"
    };

    public void Setup(int tips, int perfectCount, int totalCount)
    {
        if (tipsText != null) tipsText.text = $"小費: ${tips}";
        if (perfectServicesText != null) perfectServicesText.text = $"完美服務: {perfectCount}/{totalCount}";

        if (reviewText != null)
        {
            float ratio = totalCount > 0 ? (float)perfectCount / totalCount : 0;
            string[] pool = ratio >= 0.8f ? perfectReviews : normalReviews;
            reviewText.text = $"{pool[Random.Range(0, pool.Length)]}";
        }
    }

    public void OnNextDayClick()
    {
        // 隱藏結算畫面並重啟遊戲
        gameObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
