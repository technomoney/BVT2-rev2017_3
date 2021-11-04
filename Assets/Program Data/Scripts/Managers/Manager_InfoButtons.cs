using System.Collections.Generic;
using UnityEngine;

public enum InfoButtonType
{
    Duration,
    Pacing,
    Background,
    Target,
    TargetScale,
    TouchSensitivity,
    GridSize,
    TargetMode,
    TargetLayout,
    AudioFeedback,
    Countdown,
    RecordData,
    TargetArea,
    FocusedZone,
    PatternDifficulty,
    PlaybackDelay,
    Orientation,
    Contrast_BlendSpeed,
    Contrast_PaceTime,
    Contrast_OversizeTargets,
    Multi_TargetSets,
    Multi_RotationSpeed,
    Multi_RotationTime,
    Multi_Trials,
    Pacing_Reaction,
    NoGo_Frequency,
    DisplayTime,
    StartingLevel,
    Balance_TargetSize,
    Balance_TargetSpacing,
    HoverTime,
    ProgressDecay,
    InputMode,
    TrainingSummary_SpeedPeri,
    TrainingSummary_Memory,
    TrainingSummary_Reaction,
    TrainingSummary_GoNoGo,
    TrainingSummary_Flash,
    TrainingSummary_Contrast,
    TrainingSummary_Multi,
    TrainingSummary_Rhythm,
    PatientManager,
    PatientDetail
}

public class Manager_InfoButtons : MonoBehaviour
{
    public static Manager_InfoButtons Instance;
    private bool m_bool_isShowingPopup;

    private List<InfoPopup> popups;


    // Use this for initialization
    private void Start()
    {
        Instance = this;
        //everything needs to be enabled to grab the components..
        foreach (Transform t in transform)
            t.gameObject.SetActive(true);

        popups = new List<InfoPopup>(GetComponentsInChildren<InfoPopup>());
        ClosePopups();
        m_bool_isShowingPopup = false;
    }

    private void Update()
    {
        if (!m_bool_isShowingPopup) return;

        if (Input.GetMouseButtonDown(0)) ClosePopups();
    }

    /// <summary>
    ///     Hide any popups that are currently open
    /// </summary>
    private void ClosePopups()
    {
        popups.ForEach(p => p.Hide());
        m_bool_isShowingPopup = false;
    }

    public void ShowInfo(InfoButtonType type)
    {
        ClosePopups();

        var popup = popups.Find(p => p.m_infoType == type);

        if (popup == null)
        {
            Debug.Log("Couldn't find matching popup for type: " + type);
            return;
        }

        popup.Show();
        m_bool_isShowingPopup = true;
    }
}