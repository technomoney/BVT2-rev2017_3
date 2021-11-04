using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    private float i;
    public Vector3 m_hiddenPosition, m_openPosition;

    private Vector3 m_initialPosition, m_targetPosition;
    private bool m_isMoving;
    public bool m_startOpen;
    public float moveSpeed;
    private RectTransform rectTransform;
    public bool m_isOpen { private set; get; }

    // Use this for initialization
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (m_startOpen) Open();
        else Hide();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_isMoving) return;

        i += Time.deltaTime * moveSpeed;

        rectTransform.anchoredPosition = Vector3.Lerp(m_initialPosition, m_targetPosition, i);

        if (i < 1) return;

        //set the position manually here in case there are any wacky rounding errors..
        rectTransform.anchoredPosition = m_targetPosition;

        i = 0;
        m_isMoving = false;
    }

    public void Open()
    {
        m_isMoving = true;
        i = 0;
        m_initialPosition = rectTransform.anchoredPosition;
        m_targetPosition = m_openPosition;
        m_isOpen = true;
    }

    public void Close()
    {
        m_isMoving = true;
        i = 0;
        m_initialPosition = rectTransform.anchoredPosition;
        m_targetPosition = m_hiddenPosition;
        m_isOpen = false;
    }

    private void Hide()
    {
        m_isOpen = false;
        i = 0;
        rectTransform.anchoredPosition = m_hiddenPosition;
    }
}