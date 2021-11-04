using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BalancePlateCursor : SerializedMonoBehaviour
{
    public delegate void BalancePlateCursorEvent();

    public static BalancePlateCursor Instance;

    /// <summary>
    ///     this event will be kicked when the cursor is shown, and is under trigger enter with something tagged
    ///     as 'BpTarget', we can't use stay since it won't update if the cursor isn't moving... which is weird
    /// </summary>
    public BalancePlateCursorEvent event_triggerEnter;

    /// <summary>
    ///     kicked when the cursor is shown and trigger exits with something tagged 'BpTarget'
    /// </summary>
    public BalancePlateCursorEvent event_triggerExit;

    public BalancePlateCursorEvent event_triggerStay;

    public Image image_cursor, image_progressRing;

    [InfoBox("Testing to allow the mouse to control the bp cursor")]
    public bool m_mouseMode;

    public RectTransform m_rectTransform_base;
    private bool m_showCursor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Manager_Test.Instance.event_inputModeChanged += InputStateChanged;
        if (m_mouseMode) SetCursorVisibility(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_showCursor) return;

        if (m_mouseMode)
            m_rectTransform_base.anchoredPosition = new Vector2(Input.mousePosition.x - Screen.width / 2,
                Input.mousePosition.y - Screen.height / 2);
        else
            m_rectTransform_base.anchoredPosition = BalancePlate.Instance.GetAdjustedBpPosition();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!m_showCursor) return;
        if (col.gameObject.CompareTag("BpTarget"))
            event_triggerEnter();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!m_showCursor) return;
        if (col.gameObject.CompareTag("BpTarget"))
            event_triggerExit();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!m_showCursor) return;
        if (col.gameObject.CompareTag("BpTarget"))
            event_triggerStay();
    }

    public void SetProgressRing(float percentage)
    {
        image_progressRing.fillAmount = 1 * percentage;
    }

    public void SetCursorVisibility(bool visible)
    {
        m_showCursor = visible;
        image_cursor.gameObject.SetActive(m_showCursor);
    }

    private void InputStateChanged()
    {
        SetCursorVisibility(Manager_Test.Instance.m_testInputMode == InputMode.Balance ||
                            Manager_Test.Instance.m_testInputMode == InputMode.Ehb);
    }
}