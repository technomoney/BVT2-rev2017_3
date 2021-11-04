using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Generic Template for a window with a message, title, and back button
/// </summary>
public class Window_GenericMessage : BVT_Window
{
    public static Window_GenericMessage Inst;
    private Vector2 m_normalSize;
    private TextMeshProUGUI m_text_title, m_text_message;
    private Action m_okMethod;

    private void Awake()
    {
        Inst = this;
        m_normalSize = GetComponent<RectTransform>().sizeDelta;
    }

    public override void Start()
    {
        base.Start();
        GetComponentInChildren<Button>().onClick.AddListener(Hide);
        m_text_title = transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
        m_text_message = transform.Find("Text_Message").GetComponent<TextMeshProUGUI>();
    }

    public void Show(string title, string message)
    {
        base.Show();
        m_text_title.text = title;
        m_text_message.text = message;
    }

    public void Show(string title, string message, Action okMethod)
    {
        base.Show();
        m_text_title.text = title;
        m_text_message.text = message;
        m_okMethod = okMethod;
    }

    public override void Hide()
    {
        if (m_okMethod != null)
            m_okMethod();
        //this is mainly to handle the session configs after they resize the window...
        GetRect().sizeDelta = m_normalSize;
        ChangeTextAlignment(TextAlignmentOptions.Center);
        base.Hide();
    }

    public void ChangeTextAlignment(TextAlignmentOptions newAlignment)
    {
        m_text_message.alignment = newAlignment;
    }
}