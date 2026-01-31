using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomerResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text tipText;
    public GameObject perfectGroup;
    public GameObject disappointingGroup;
    
    [Header("Settings")]
    public float displayDuration = 2f;

    public void ShowResult(int tips, bool isPerfect)
    {
        gameObject.SetActive(true);
        
        tipText.text = "$" + tips;
        perfectGroup.SetActive(isPerfect);
        disappointingGroup.SetActive(!isPerfect);

        StopAllCoroutines();
        StartCoroutine(AutoHideRoutine());
    }

    private IEnumerator AutoHideRoutine()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        gameObject.SetActive(false);
    }
}
