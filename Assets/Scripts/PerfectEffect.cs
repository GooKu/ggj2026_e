using System.Collections;
using UnityEngine;

public class PerfectEffect : MonoBehaviour
{
    [SerializeField] float ShowTime = 0.15f;

    private void OnEnable()
    {
        StartCoroutine(Disappoint());
    }

    IEnumerator Disappoint()
    {
        yield return new WaitForSeconds(ShowTime);
        gameObject.SetActive(false);
    }
}
