using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_PatientDetail : BVT_Window
{
    public static Window_PatientDetail Instance;
    private bool dateUp;
    public Button m_button_close, m_button_select, m_button_backToManager, m_button_delete, m_button_generateReport, m_btn_edit;
    private Entry_PatientTestSummary m_deletingEntry;
    public List<Entry_PatientTestSummary> m_list_testEntries;
    private Patient m_showingPatient;

    public TextMeshProUGUI m_text_name,
        m_text_id,
        m_text_gender,
        m_text_dob,
        m_text_height,
        m_text_weight,
        m_text_notes;

    public Transform m_trans_trainingSummaryParent;
    public Entry_PatientTestSummary pfb_patientTestSummary;

    private void Awake()
    {
        Instance = this;
        m_list_testEntries = new List<Entry_PatientTestSummary>();
        m_button_close.onClick.AddListener(Hide);
        m_button_select.onClick.AddListener(SelectPatient);
        m_button_delete.onClick.AddListener(ButtonPushed_DeleteClient);
        m_button_backToManager.onClick.AddListener(BackToManager);
        m_btn_edit.onClick.AddListener(ButtonPushed_Edit);
        m_button_generateReport.onClick.AddListener(ButtonPushed_Report);
        transform.Find("Training View").Find("Headers").Find("Button_Sort_ByName").GetComponent<Button>().onClick
            .AddListener(ButtonPushed_SortByName);
        transform.Find("Training View").Find("Headers").Find("Button_Sort_ByDate").GetComponent<Button>().onClick
            .AddListener(ButtonPushed_SortByDate);
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        ClearAllFields();
    }


    public void Show(Patient p, bool showSelectButton = true, bool doBaseShow = true)
    {
        //todo do show animation
        if (doBaseShow)
            base.Show();

        ClearAllFields();
        //set the vitals fields
        m_text_name.text = p.m_name_first + " " + p.m_name_last;
        m_text_id.text = p.m_id;
        m_text_gender.text = p.m_gender;
        m_text_dob.text = p.m_dob_month + " " + p.m_dob_day + " " + p.m_dob_year;
        m_text_height.text = p.m_height_feet + "' " + p.m_height_inches + "\"";
        m_text_weight.text = p.m_weight + " lbs";
        m_text_notes.text = p.m_notes;

        m_showingPatient = p;

        m_button_select.gameObject.SetActive(showSelectButton);
        m_button_backToManager.gameObject.SetActive(!showSelectButton);

        //we need to populate the training summary area with the tests for this patient
        //first we want to nuke any existing entries in there
        m_list_testEntries.ForEach(e => Destroy(e.gameObject));
        m_list_testEntries.Clear();

        //now make an entry for every test in this patient directory
        //first, get all files in our patient directory
        var dirInfo = new DirectoryInfo(Manager_Patient.Instance.GetPatientDirectory(p));
        //get all files
        var files = dirInfo.GetFiles();

        for (var x = 0; x < files.Length; x++)
        {
            //we don't want to try and read the patient info, so we'll skip that one
            if (files[x].Name.Equals("patientInfo.xml")) continue;
            //we don't want to try and read the data files either, so skip those
            if (files[x].Name.ToLower().Contains("data")) continue;

            //we'll try to deserialize every file in to a patientSummary
            PatientTrainingSummary summary = null;

            var reader = new XmlSerializer(typeof(PatientTrainingSummary));
            var file = new StreamReader(files[x].FullName);
            summary = (PatientTrainingSummary) reader.Deserialize(file);
            file.Close();
            summary.m_fileName = files[x].Name;

            //if (summary == null) continue;
            //todo should have some kind of check here to make sure this succeeded 
            var e = Instantiate(pfb_patientTestSummary, m_trans_trainingSummaryParent);
            e.Initialize(summary, p);

            m_list_testEntries.Add(e);
        }

        //finally, we need to show the delete client button if we're logged in as admin only
        if (Manager_Users.Instance.m_currentUser != null && Manager_Users.Instance.m_currentUser.m_isAdmin)
            m_button_delete.gameObject.SetActive(true);
        else m_button_delete.gameObject.SetActive(false);
    }

    private void SelectPatient()
    {
        if (m_showingPatient == null)
        {
            Debug.Log("Trying to select a null patient somehow..");
            return;
        }

        Manager_Patient.Instance.SetSelectedPatient(m_showingPatient);
        Hide();
        Manager_Patient.Instance.Hide();
        //nuke the scoreboard when selecting a new patient
        Menu_Reports.Instance.ClearEntries();
    }

    private void ButtonPushed_Edit()
    {
        Window_AddPatient.Instance.ShowForEdit(m_showingPatient);
    }

    private void ButtonPushed_Report()
    {
        Window_SessionReportMaker.Inst.Show(m_showingPatient);
    }

    private void BackToManager()
    {
        Hide();
        Manager_Patient.Instance.Show();
    }

    private void ButtonPushed_DeleteClient()
    {
        Window_GenericConfirmation.Inst.Show("Confirm Delete Client",
            "This will remove this client and all collected data and test results.\nThis cannot be undone.\nAre you sure you want to proceed?",
            ConfirmDeleteClient, null);
    }

    private void ConfirmDeleteClient()
    {
        //to nuke a client completely we just have to delete their patient folder
        //this will be in our persistent data location /patients/pat_Id_[patient id]
        Directory.Delete(Manager_Patient.Instance.GetPatientDirectory(m_showingPatient), true);
        //allow the patient manager to catch up to what we just did
        Manager_Patient.Instance.PatientDeleted(m_showingPatient);
        //then clear it out
        m_showingPatient = null;
        //and go back to the manager
        BackToManager();
    }

    private void ClearAllFields()
    {
        m_text_name.text = "";
        m_text_gender.text = "";
        m_text_dob.text = "";
        m_text_height.text = "";
        m_text_weight.text = "";
    }

    private void ButtonPushed_SortByName()
    {
        //sort the entries by last name
        m_list_testEntries.Sort((e1, e2) => e1.GetPatient().m_name_last.CompareTo(e2.GetPatient().m_name_last));
        //now the list is sorted so let's change the parenting to something else to pop them out of the scroll view
        m_list_testEntries.ForEach(e => e.transform.SetParent(transform));
        //then change them back so the layout group fixes them
        m_list_testEntries.ForEach(e => e.transform.SetParent(m_trans_trainingSummaryParent));

        //then rotate the triangle //todo could do a nice lerp for the rotation..
        transform.Find("Training View").Find("Headers").Find("Button_Sort_ByName").transform.Rotate(0, 0, 180);
    }

    private void ButtonPushed_SortByDate()
    {
        //we have to use this flip flag here because compareTo won't switch between asc/descending with the dateTime like it does
        //with string.... for whatever reason
        dateUp = !dateUp;
        //sort the entries by date
        if (dateUp)
            m_list_testEntries.Sort((e1, e2) =>
                e1.m_trainingSummary.m_dateTime.CompareTo(e2.m_trainingSummary.m_dateTime));
        else
            m_list_testEntries.Sort((e1, e2) =>
                e2.m_trainingSummary.m_dateTime.CompareTo(e1.m_trainingSummary.m_dateTime));

        //now the list is sorted so let's change the parenting to something else to pop them out of the scroll view
        m_list_testEntries.ForEach(e => e.transform.SetParent(transform));
        //then change them back so the layout group fixes them
        m_list_testEntries.ForEach(e => e.transform.SetParent(m_trans_trainingSummaryParent));

        //then rotate the triangle
        transform.Find("Training View").Find("Headers").Find("Button_Sort_ByDate").transform.Rotate(0, 0, 180);
    }

    public void ShowDeleteConfirmation(Entry_PatientTestSummary summary)
    {
        m_deletingEntry = summary;
        Window_GenericConfirmation.Inst.Show("Confirm Delete Training Entry",
            "Are you sure you want to delete this entry?  It will be removed from the client history and cannot be undone.",
            ConfirmDeleteEntry, null);
    }

    private void ConfirmDeleteEntry()
    {
        if (m_deletingEntry == null)
        {
            Debug.LogWarning("Entry to delete is null..");
            return;
        }

        //nuke this entry
        //using the file name, delete the file
        var path = Manager_Patient.Instance.GetPatientDirectory(m_showingPatient) + "/" +
                   m_deletingEntry.m_trainingSummary.m_fileName;

        if (!File.Exists(path))
        {
            Debug.LogWarning("Can't find file to delete test entry...");
            return;
        }

        File.Delete(path);
        m_deletingEntry = null;
        //then redraw the list of entries
        Show(m_showingPatient);
    }
}