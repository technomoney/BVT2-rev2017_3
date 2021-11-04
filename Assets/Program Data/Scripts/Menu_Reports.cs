using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Reports : MonoBehaviour
{
    public static Menu_Reports Instance;

    public Button m_button_clear, m_button_discard;
    public Button m_button_expandPanel, m_button_showFullReport;
    private Entry_Scoreboard m_lastEntry;
    private List<Entry_Scoreboard> m_list_entries;

    /// <summary>
    /// a list of all of the strings currently shown in the scoreboard
    /// </summary>
    public List<string> m_list_reportStrings;

    public List<Sprite> m_list_testIcons;

    public MenuPanel m_panel_reports;

    public Transform m_trans_contentParent;
    public Entry_Scoreboard pfb_entry_scoreboard;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        m_button_expandPanel.onClick.AddListener(TogglePanel);
        m_button_showFullReport.onClick.AddListener(ButtonPushed_ShowFullReport);
        m_button_clear.onClick.AddListener(ButtonPushed_Clear);
        m_button_discard.onClick.AddListener(ButtonPushed_Discard);
        m_list_reportStrings = new List<string>();

        if (m_list_entries == null) m_list_entries = new List<Entry_Scoreboard>();
    }

    private void ButtonPushed_Discard()
    {
        if (m_list_entries.Count <= 0) return;
        if (m_lastEntry == null) return;

        //Window_DiscardLastConfirmation.Inst.Show();
        Window_GenericConfirmation.Inst.Show("Confirm Discard Last Trial",
            "Are you sure you want to discard the last trial?\nThis will remove it from the client history and cannot be undone",
            ConfirmDiscardLast, null);
    }

    private void ConfirmDiscardLast()
    {
        //nuke this entry
        //using the file name, delete the file
        var path = Manager_Patient.Instance.GetPatientDirectory(Manager_Patient.Instance.m_selectedPatient) + "/" +
                   m_lastEntry.m_trainingSummary.m_fileName;

        if (!File.Exists(path))
        {
            Debug.LogWarning("Can't find file to delete test entry...");
            return;
        }

        File.Delete(path);

        //then remove the scoreboard entry
        m_list_entries.Remove(m_lastEntry);
        Destroy(m_lastEntry.gameObject);
    }

    private void ButtonPushed_Clear()
    {
        if (m_list_entries.Count <= 0) return;

        Window_GenericConfirmation.Inst.Show("Confirm Clear Scoreboard",
            "Are you sure you want to clear the current scoreboard?  This will not remove anything from the client history.",
            ClearEntries, null);
    }

    public void ClearEntries()
    {
        m_list_entries.ForEach(e => Destroy(e.gameObject));
        m_list_entries = new List<Entry_Scoreboard>();

        m_list_reportStrings.Clear();
    }

    private void TogglePanel()
    {
        if (m_panel_reports.m_isOpen)
            m_panel_reports.Close();
        else
            m_panel_reports.Open();

        m_button_expandPanel.transform.GetChild(0).GetComponent<RectTransform>().localRotation =
            m_panel_reports.m_isOpen ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 90);
    }

    private void ButtonPushed_ShowFullReport()
    {
        FullReport.Instance.Show();
    }

    /// <summary>
    ///     Add an entry to the scoreboard using the new method with a predetermined entry and its necessary
    ///     data values assigned by the test that is adding it
    /// </summary>
    /// <param name="entry">The instantiated entry from the test that is adding it</param>
    public void AddEntry(Entry_Scoreboard entry)
    {
        //whenever we add a new entry to the scoreboad, add it to the current patient, if there is one
        if (Manager_Patient.Instance.m_selectedPatient == null)
        {
            Debug.Log("Can't add patient summary with no patient selected..");
            Destroy(entry.gameObject);
            return;
        }


        m_list_entries.Add(entry);
        m_lastEntry = entry;

        //set the icon
        entry.m_image_icon.sprite = GetTestIcon(entry.m_trainingSummary.m_testType);

        //make the image preserve the aspect ratio of the sprite
        entry.m_image_icon.preserveAspect = true;

        //set the label
        entry.text_label.text = entry.m_trainingSummary.m_shortScore;

        Debug.Log(entry.m_trainingSummary.m_testName + " entry added to scordboard");
    }

    public Sprite GetTestIcon(TestType type)
    {
        switch (type)
        {
            case TestType.Speed:
                return m_list_testIcons[0];
            case TestType.Peripheral:
                return m_list_testIcons[1];
            case TestType.Sequence:
                return m_list_testIcons[2];
            case TestType.Reaction:
                return m_list_testIcons[3];
            case TestType.Balance:
                return m_list_testIcons[4];
            case TestType.GoNoGo:
                return m_list_testIcons[5];
            case TestType.Flash:
                return m_list_testIcons[6];
            case TestType.Rhythm:
                return m_list_testIcons[7];
            case TestType.Contrast:
                return m_list_testIcons[8];
            case TestType.Multi:
                return m_list_testIcons[9];
            case TestType.All:
            case TestType.None:
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}