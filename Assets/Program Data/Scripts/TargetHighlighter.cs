using UnityEngine;

public class TargetHighlighter : MonoBehaviour
{
    private bool m_scalingUp;
    public float m_speed, m_minSize, m_maxSize;
    public RectTransform rectTransform;


    // Use this for initialization
    private void Start()
    {
    }

    /// <summary>
    ///     Move the highlighter to the given position and show it
    /// </summary>
    public void MoveAndShow(Vector2 position)
    {
        gameObject.SetActive(true);
        m_scalingUp = true;
        rectTransform.localScale = Vector3.one;

        rectTransform.anchoredPosition = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!gameObject.activeSelf) return;

        var newScale = m_scalingUp
            ? rectTransform.localScale.x + Time.deltaTime * m_speed
            : rectTransform.localScale.x - Time.deltaTime * m_speed;

        rectTransform.localScale = new Vector3(newScale, newScale);

        if (m_scalingUp && newScale > m_maxSize || !m_scalingUp && newScale < m_minSize)
            m_scalingUp = !m_scalingUp;
    }
}