using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private void Start()
    {
#if UNITY_STANDALONE && UNITY_EDITOR


        foreach (Transform t in transform)
            Destroy(t.gameObject);

        Destroy(this);
        return;
#endif

        // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable 162
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        StartCoroutine(Countdown());
#pragma warning restore 162
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(3);

        foreach (Transform t in transform)
            Destroy(t.gameObject);

        Destroy(this);
    }
}