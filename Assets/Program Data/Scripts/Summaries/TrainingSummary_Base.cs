using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSummary_Base : SerializedMonoBehaviour

{
    public Button m_button_close;
    public string m_dataLocation;
    public RectTransform m_summaryBase_rectTransform;

    public TextMeshProUGUI m_text_title;
    private Vector2 m_vec2_hiddenPosition;
    public Vector2 m_vec2_showPosition;


    // Use this for initialization
    public virtual void Start()
    {
        m_vec2_hiddenPosition = m_summaryBase_rectTransform.anchoredPosition;

        m_button_close.onClick.AddListener(Close);
    }

    public virtual void Show(PatientTrainingSummary e)
    {
        if (e == null)
        {
            Debug.Log("TrainingSummaryBase.Show() got a null summary...");
            return;
        }

        m_summaryBase_rectTransform.anchoredPosition = m_vec2_showPosition;
        Manager_SummaryGraphs.Instance.ShowRaycastBlocker(true);

        //set the title
        var titleString = "";
        switch (e.m_testType)
        {
            case TestType.Speed:
                titleString = "Speed Training Summary";
                break;
            case TestType.Peripheral:
                titleString = "Peripheral Training Summary";
                break;
            case TestType.Sequence:
                titleString = "Sequence Training Summary";
                break;
            case TestType.Reaction:
                titleString = "Reaction Training Summary";
                break;
            case TestType.Flash:
                titleString = "Flash Training Summary";
                break;
            case TestType.GoNoGo:
                titleString = "Go/No-Go Training Summary";
                break;
            case TestType.Balance:
                titleString = "Balance Summary";
                break;
            case TestType.All:
            case TestType.None:
            default:
                titleString = "Training Summary";
                break;
        }

        m_text_title.text = titleString;
    }

    public virtual void Close()
    {
        m_summaryBase_rectTransform.anchoredPosition = m_vec2_hiddenPosition;
        Manager_SummaryGraphs.Instance.ShowRaycastBlocker(false);
    }
}