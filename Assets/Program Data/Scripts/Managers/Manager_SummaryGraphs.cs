using System;
using UnityEngine;
using UnityEngine.UI;

public class Manager_SummaryGraphs : MonoBehaviour
{
    public static Manager_SummaryGraphs Instance;
    public Image image_raycastBlocker;

    public TrainingSummary_Base m_summary_speed_peri,
        m_summary_memory,
        m_summary_reaction,
        m_summary_goNogo,
        m_summary_flash,
        m_summary_balance,
        m_summary_balance_static,
        m_summary_balance_dynamic,
        m_summary_rhythm,
        m_summary_contrast,
        m_summary_multi;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        //initially make sure the raycast blocker is disabled
        image_raycastBlocker.gameObject.SetActive(false);
    }

    public void HideAll()
    {
        m_summary_speed_peri.Close();
        m_summary_reaction.Close();
        m_summary_memory.Close();
        ShowRaycastBlocker(false);
    }

    public void ShowSummary(PatientTrainingSummary e)
    {
        switch (e.m_testType)
        {
            case TestType.Speed:
                m_summary_speed_peri.Show(e);
                break;
            case TestType.Peripheral:
                m_summary_speed_peri.Show(e);
                break;
            case TestType.Sequence:
                m_summary_memory.Show(e);
                break;
            case TestType.Reaction:
                m_summary_reaction.Show(e);
                break;
            case TestType.GoNoGo:
                m_summary_goNogo.Show(e);
                break;
            case TestType.Flash:
                m_summary_flash.Show(e);
                break;
            case TestType.Balance:
                if (e.m_balance_trackingMode == TargetTrackingMode.Dynamic)
                    m_summary_balance_dynamic.Show(e);
                else m_summary_balance_static.Show(e);
                break;
            case TestType.Rhythm:
                m_summary_rhythm.Show(e);
                break;
            case TestType.Contrast:
                m_summary_contrast.Show(e);
                break;
            case TestType.Multi:
                m_summary_multi.Show(e);
                break;
            case TestType.All:
                break;
            case TestType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ShowRaycastBlocker(bool show)
    {
        image_raycastBlocker.gameObject.SetActive(show);
    }
}