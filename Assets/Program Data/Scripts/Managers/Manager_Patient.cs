using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Patient : SerializedMonoBehaviour
{
    public static Manager_Patient Instance;
    public Button m_button_add, m_button_cancel;
    private List<Entry_Patient> m_list_patientEntries;
    public float m_moveSpeed;
    private string m_path_patientDir;
    public Transform m_trans_patientContent;
    private Vector2 m_vec2_hidePosition;
    private Vector2 m_vec2_nextEntryPosition;
    public Vector2 m_vec2_showPosition;
    public Entry_Patient pfb_patientEntry;
    public Patient m_selectedPatient { get; private set; }

    private void Awake()
    {
        Instance = this;
        m_vec2_hidePosition = GetComponent<RectTransform>().anchoredPosition;
        m_selectedPatient = null;
        m_list_patientEntries = new List<Entry_Patient>();
        m_button_cancel.onClick.AddListener(Hide);
        m_button_add.onClick.AddListener(ButtonClicked_AddPatient);
    }

    private void Start()
    {
        BuildPatientList();
    }

    private void ButtonClicked_AddPatient()
    {
        //show the new patient dialog
        Window_AddPatient.Instance.Show();
    }

    public void AddNewPatient(Patient newPatient, bool makeFile = true)
    {
        //make a new entry in the ui
        var p = Instantiate(pfb_patientEntry, m_trans_patientContent);
        p.Initialize(newPatient);

        m_list_patientEntries.Add(p);
        //todo we should check and avoid making duplicate named directories..

        if (!makeFile) return;
        //whenever we add a patient, we have to make a patient directory to store the patient info and test data
        //directory name will be patient name + current date
        var patientDirectory = m_path_patientDir + "/pat_Id_" + newPatient.m_id;
        //newPatient.m_name_first + "_" + newPatient.m_name_last;
        Directory.CreateDirectory(patientDirectory);
        var filePath = patientDirectory + "/patientInfo.xml";
        var writer = new XmlSerializer(typeof(Patient));
        var file = File.Create(filePath);
        writer.Serialize(file, newPatient);
        file.Close();
    }

    public void BuildPatientList()
    {
        //nuke any lists and instantited object we might already have
        m_list_patientEntries.ForEach(e => Destroy(e.gameObject));
        m_list_patientEntries.Clear();
        m_list_patientEntries = new List<Entry_Patient>();

        //set the patient directory path for use
        m_path_patientDir = Application.persistentDataPath + "/Patients";

        //make sure the folder exists, if not, make it
        if (!Directory.Exists(m_path_patientDir))
            Directory.CreateDirectory(m_path_patientDir);

        //now that we have our directory, get all sub directories that have the 'pat' prefix
        var patientDirInfo = new DirectoryInfo(m_path_patientDir);
        var patientDirectories = patientDirInfo.GetDirectories("pat*");
        Debug.Log("Got " + patientDirectories.Length + " patient directories");

        foreach (var dirInfo in patientDirectories)
        {
            //get the patient info file from each directory
            var patientInfoFile = dirInfo.GetFiles("patientInfo.xml");
            if (patientInfoFile.Length != 1)
            {
                Debug.Log("Problem locating patient info file..");
                continue;
            }

            var reader = new XmlSerializer(typeof(Patient));
            var file = new StreamReader(patientInfoFile[0].FullName);
            AddNewPatient((Patient) reader.Deserialize(file), false);
            file.Close();
        }
    }

    public void Show()
    {
        StartCoroutine(MoveToPosition(m_vec2_showPosition));
    }

    public void Hide()
    {
        StartCoroutine(MoveToPosition(m_vec2_hidePosition));
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        var rectTransform = GetComponent<RectTransform>();
        var initialPos = rectTransform.anchoredPosition;
        float i = 0;
        while (i <= 1)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(initialPos, targetPosition, i);
            i += Time.deltaTime * m_moveSpeed;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    /// <summary>
    ///     we should call this whenever a patient is deleted to we can rebuild the list and clear things
    ///     out if necessary
    /// </summary>
    public void PatientDeleted(Patient deletePatient)
    {
        BuildPatientList();
        //if the patient we just deleted was also selected we have to wipe it out
        if (!m_selectedPatient.m_id.Equals(deletePatient.m_id)) return;
        //fix the patient name display
        PatientDisplay.Instance.SetPatient(null);
        //and clear the selected patient from here
        m_selectedPatient = null;
    }

    public void SetSelectedPatient(Patient p)
    {
        m_selectedPatient = p;
        //update the patient display info
        PatientDisplay.Instance.SetPatient(m_selectedPatient);
    }

    public string GetSelectedPatientDirectory()
    {
        return GetPatientDirectory(m_selectedPatient);
    }

    public string GetPatientDirectory(Patient p)
    {
        var path = m_path_patientDir + "/pat_Id_" + p.m_id;

        //make sure that is a valid directory
        if (!Directory.Exists(path))
        {
            Debug.Log("Problem locating selected patient directory");
            return "NULL";
        }

        return path;
    }
}