using System.Collections;
using UnityEngine;

public class ShowMessage : MonoBehaviour
{
    [SerializeField] float ShowTime = 1.0f;

    private void OnEnable()
    {
        StartCoroutine(Disappoint());
    }

    IEnumerator Disappoint()
    {
        yield return new WaitForSeconds(ShowTime);
        Destroy(gameObject);
    }

}
