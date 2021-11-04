using System;
using TMPro;
using UnityEngine.UI;

public class Window_GenericConfirmation : BVT_Window
{
    public static Window_GenericConfirmation Inst;
    private Action m_confirmMethod, m_cancelMethod;
    private TextMeshProUGUI m_text_title, m_text_message;

    private void Awake()
    {
        Inst = this;
    }

    public override void Start()
    {
        base.Start();
        m_text_title = transform.Find("Text_Title").GetComponent<TextMeshProUGUI>();
        m_text_message = transform.Find("Text_Message").GetComponent<TextMeshProUGUI>();
        transform.Find("Button_Confirm").GetComponent<Button>().onClick.AddListener(ButtonPushed_Confirm);
        transform.Find("Button_Cancel").GetComponent<Button>().onClick.AddListener(ButtonPushed_Cancel);
    }

    public void Show(string title, string message, Action confirmMethod, Action cancelMethod)
    {
        base.Show();
        m_text_title.text = title;
        m_text_message.text = message;
        m_confirmMethod = confirmMethod;
        m_cancelMethod = cancelMethod;
    }


    private void ButtonPushed_Confirm()
    {
        Hide();
        m_confirmMethod();
    }

    private void ButtonPushed_Cancel()
    {
        Hide();

        if (m_cancelMethod != null)
            m_cancelMethod();
    }
}