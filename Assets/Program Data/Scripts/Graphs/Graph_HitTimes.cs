using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Graph_HitTimes : SerializedMonoBehaviour
{
    public float m_barHeight;
    private float m_bounds_x, m_maxVal;
    public float m_drawTime;
    public TextMeshProUGUI m_text_avg, m_text_fast, m_text_slow;
    public RectTransform m_trans_graphArea, m_trans_avg, m_trans_fast, m_trans_slow;

    private void Awake()
    {
        m_bounds_x = m_trans_graphArea.sizeDelta.x;
        if (m_barHeight <= 0) m_barHeight = 37;
    }

    private void Update()
    {
        //testing
        if (Input.GetKeyDown(KeyCode.B))
            Show(3.24f, 1.17f, 6.2f);
    }

    public void Show(float avg, float fast, float slow)
    {
        //get the max value for the length of the bars..
        m_maxVal = Mathf.Max(avg, slow, fast);

        if (m_maxVal == 0)
        {
            Debug.LogWarning("HitTimes.Show() got all zeroes, returning..");
            return;
        }

        StartCoroutine(GrowBar(m_trans_slow, m_text_slow, slow));
        StartCoroutine(GrowBar(m_trans_avg, m_text_avg, avg));
        StartCoroutine(GrowBar(m_trans_fast, m_text_fast, fast));
    }

    private IEnumerator GrowBar(RectTransform trans, TextMeshProUGUI text, float val)
    {
        float currentTime = 0;
        const float initialSize = 0;
        var finalSize = Mathf.Lerp(0, m_bounds_x, val / m_maxVal);
        var sizeDelta = new Vector2(0, m_barHeight);
        while (currentTime < m_drawTime)
        {
            sizeDelta.x = Mathf.Lerp(initialSize, finalSize, currentTime / m_drawTime);
            trans.sizeDelta = sizeDelta;

            text.text = Mathf.Lerp(0, val, currentTime / m_drawTime).ToString("0.00");

            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}