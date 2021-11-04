using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailSender : MonoBehaviour
{
    public static MailSender Instance;
    private DirectoryInfo dirInfo;

    /// <summary>
    ///     this allows the send button to work even when there aren't any entries in the
    ///     scoreboard, for testing only
    /// </summary>
    public bool m_allowEmptySummaries;

    public Button m_button_cancel, m_button_send, m_button_show;
    public TMP_InputField m_input_mailto;
    private bool m_isShowing;

    private string m_leadingMessage;
    public string m_mailFrom, m_subject, m_body;
    private string m_mailTo;

    public int m_port;
    public RectTransform m_rectTransform;
    public string m_smtpPassword;
    public string m_smtpserver;
    private Vector2 m_vec2_hiddenPosition;
    public Vector2 m_vec2_showPosition;

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        m_vec2_hiddenPosition = m_rectTransform.anchoredPosition;
        m_isShowing = false;

        m_button_show.onClick.AddListener(ButtonPushed_Show);
        m_button_cancel.onClick.AddListener(ButtonPushed_Cancel);
        m_button_send.onClick.AddListener(ButtonPushed_Send);

        var dirPath = Application.dataPath + @"/Emails";
        Directory.CreateDirectory(dirPath);
        dirInfo = new DirectoryInfo(dirPath);

        m_leadingMessage =
            "---------------------------------------------------------------------------------------------------------------------------\n" +
            "Please Note: This is an early version of the Email Export feature for Bertec Vision Trainer.  " +
            "For demonstration only." +
            "\n---------------------------------------------------------------------------------------------------------------------------\n";
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_isShowing) return;
    }

    private void ButtonPushed_Send()
    {
        //make sure the address is legit
        if (!ValidAddress()) return;

        //compose the body
        m_body = m_leadingMessage + "\n";
        m_body += "-----Test Report Summary-----\n";
        var reports = Menu_Reports.Instance.m_list_reportStrings;
        if (reports.Count > 0)
        {
            m_body += "Date: " + DateTime.Now + "\n";
            m_body += "No. of Trainings: " + reports.Count + "\n\n";
            m_body += "-----------------------------------------------------\n";
            foreach (var s in reports)
            {
                m_body += s + "\n\n";
                m_body += "-----------------------------------------------------\n";
            }
        }

        m_body += "-----End of Report-----";

        //save a backup in case something goes wrong while sending..

        var filePath = Application.dataPath + @"/Emails/" + dirInfo.GetFiles().Length + " - " + m_mailTo + @".txt";
        TextWriter writer = File.CreateText(filePath);
        writer.WriteLine("Send to: " + m_mailTo + "\n" + m_body);
        writer.Close();

        StartCoroutine(SendMail());

        ButtonPushed_Cancel();
    }

    private void ButtonPushed_Cancel()
    {
        m_rectTransform.anchoredPosition = m_vec2_hiddenPosition;
        m_isShowing = false;
    }

    private void ButtonPushed_Show()
    {
        //make sure there is at least one entry in the summary list
        if (!m_allowEmptySummaries && Menu_Reports.Instance.m_list_reportStrings.Count <= 0) return;

        m_input_mailto.text = "";
        m_rectTransform.anchoredPosition = m_vec2_showPosition;
        m_isShowing = true;
    }

    private IEnumerator SendMail()
    {
        var message = new MailMessage();

        message.From = new MailAddress(m_mailFrom);
        message.To.Add(m_mailTo);
        message.To.Add(m_mailFrom); //always send a copy to the sending address so we have a record..
        message.Subject = m_subject;
        message.Body = m_body;

        var smtp = new SmtpClient(m_smtpserver);
        smtp.Port = m_port;
        smtp.Credentials = new NetworkCredential(m_mailFrom, m_smtpPassword);
        smtp.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate { return true; };
        smtp.Send(message);
        Debug.Log("Email successfully sent to: " + m_mailTo);
        yield return null;
    }

    private bool ValidAddress()
    {
        var str = m_input_mailto.text;

        if (str == null) return false;

        //the string must have an @ and at least one .
        if (!str.Contains("@")) return false;
        if (!str.Contains(".")) return false;

        //split the field by the @
        var split = str.Split('@');
        if (split.Length != 2) return false;

        //name can be anything... but we can split it here if necessary
        //we'll comment out to hide the warning..
        //string name = split[0];

        var domain = split[1];

        //name can generally be anything..

        //the domain must have a . in it and be followed by 2 or three characters..
        if (domain.Length < 4) return false;

        split = domain.Split('.');

        //there could potentially be other . in the domain, so lets just look at the last one
        var suffix = split.Last();
        if (suffix.Length != 2 && suffix.Length != 3) return false;

        m_mailTo = str;

        return true;
    }
}