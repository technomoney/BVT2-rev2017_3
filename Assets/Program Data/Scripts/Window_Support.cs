using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_Support : SerializedMonoBehaviour
{
    public static Window_Support Instance;
    public Button m_button_send, m_button_cancel;
    public TMP_Dropdown m_dropdown;
    public TMP_InputField m_input_message, m_input_name, m_input_location;

    private bool m_isShowing;

    //private int m_dropdownIndex;
    private Vector2 m_vec2_hidePosition;
    public Vector2 m_vec2_showPosition;

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        m_dropdown.onValueChanged.AddListener(DropdownValueChanged);
        m_button_cancel.onClick.AddListener(ButtonPushed_Cancel);
        m_button_send.onClick.AddListener(ButtonPushed_Send);
        m_vec2_hidePosition = GetComponent<RectTransform>().anchoredPosition;
        m_isShowing = false;
    }

    public void ToggleVisibility()
    {
        m_isShowing = !m_isShowing;
        GetComponent<RectTransform>().anchoredPosition = m_isShowing ? m_vec2_showPosition : m_vec2_hidePosition;
    }

    private void DropdownValueChanged(int index)
    {
    }

    private void ButtonPushed_Cancel()
    {
        ToggleVisibility();
    }

    private void ButtonPushed_Send()
    {
    }
}