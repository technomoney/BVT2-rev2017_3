using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Entry_Scoreboard : MonoBehaviour
{
    public Button m_button_summary;
    public Image m_image_icon;
    public PatientTrainingSummary m_trainingSummary;
    public RectTransform rectTransform;
    public TextMeshProUGUI text_label;

    // Use this for initialization
    private void Start()
    {
        m_button_summary.onClick.AddListener(ShowSummary);
    }

    private void ShowSummary()
    {
        Manager_SummaryGraphs.Instance.ShowSummary(m_trainingSummary);
    }
}