using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour
{
    [Tooltip("如果為空，則使用 AudioManager 中的預設點擊音效")]
    public AudioClip customClickSound;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager Instance 尚未建立，無法播放音效。");
            return;
        }

        if (customClickSound != null)
        {
            AudioManager.Instance.PlaySFX(customClickSound);
        }
        else
        {
            AudioManager.Instance.PlayClickSound();
        }
    }
}
