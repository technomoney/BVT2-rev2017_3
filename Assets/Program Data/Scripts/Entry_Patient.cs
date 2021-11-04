using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class Entry_Patient : SerializedMonoBehaviour
{
    private Patient m_patient;
    public TextMeshProUGUI m_text_name_first, m_text_name_last, m_text_id;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    public void Initialize(Patient patient)
    {
        m_patient = patient;
        m_text_name_first.text = m_patient.m_name_first;
        m_text_name_last.text = m_patient.m_name_last;

        //todo legacy check here if there is no id..
        if (patient.m_id == null || patient.m_id.Equals(string.Empty)) m_patient.SetId("No Id");

        else m_text_id.text = m_patient.m_id;
    }

    private void Clicked()
    {
        Window_PatientDetail.Instance.Show(m_patient);
    }
}