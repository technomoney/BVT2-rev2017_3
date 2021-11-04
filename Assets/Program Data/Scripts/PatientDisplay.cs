using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class PatientDisplay : SerializedMonoBehaviour
{
    public static PatientDisplay Instance;
    private Button m_button;
    public TextMeshProUGUI m_text_patientName;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(Clicked);
        m_text_patientName.text = "Select Client";
    }

    private void Clicked()
    {
        //if we ever manage to click this without having a user logged in, do that now..
        if (Manager_Users.Instance.m_currentUser == null)
        {
            Manager_Users.Instance.ShowOnStartUp();
            return;
        }
        if (Manager_Patient.Instance.m_selectedPatient == null)
            //show the patient manager
            Manager_Patient.Instance.Show();
        else
            //show the patient detail
            Window_PatientDetail.Instance.Show(Manager_Patient.Instance.m_selectedPatient, false);
    }

    public void SetPatient(Patient p)
    {
        if (p == null)
            m_text_patientName.text = "Select Client";
        else
            m_text_patientName.text = p.m_name_first.Substring(0, 1) + ". " + p.m_name_last;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}