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
        "師傅的手法令我著迷",
        "我會推薦朋友來 <3",
        "10/10",
        "真是太烏茲哭西了⋯⋯",
        "一定是大拇指的啦"
    };

    public string[] normalReviews = new string[] {
        "做 der 好 社會點數 +15",
        "CP 值過低，不會回購",
        "開心 ( ^ _ ^ )",
        "會再來，要更進步唷 <3",
        "可再接再厲"
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
