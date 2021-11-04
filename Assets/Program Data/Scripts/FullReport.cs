using UnityEngine;
using UnityEngine.UI;

public class FullReport : MonoBehaviour
{
    public static FullReport Instance;
    private float i;
    public Button m_button_close;
    private bool m_isMoving;
    public float m_moveSpeed;
    public Vector2 m_position_hidden, m_position_shown;
    private Vector2 m_position_initial, m_position_target;
    private RectTransform rectTransform;

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        m_button_close.onClick.AddListener(ButtonPushed_Close);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_isMoving) return;

        i += Time.deltaTime * m_moveSpeed;

        rectTransform.anchoredPosition = Vector2.Lerp(m_position_initial, m_position_target, i);

        if (i < 1) return;

        //set the position manually here in case there are any wacky rounding errors..
        rectTransform.anchoredPosition = m_position_target;

        i = 0;
        m_isMoving = false;
    }

    public void Show()
    {
        m_isMoving = true;
        m_position_initial = rectTransform.anchoredPosition;
        m_position_target = m_position_shown;
        i = 0;
    }

    public void Hide()
    {
        m_isMoving = true;
        m_position_initial = rectTransform.anchoredPosition;
        m_position_target = m_position_hidden;
        i = 0;
    }

    public void ButtonPushed_Close()
    {
        Hide();
    }
}