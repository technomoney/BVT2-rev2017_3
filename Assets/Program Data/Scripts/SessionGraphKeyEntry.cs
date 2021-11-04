using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionGraphKeyEntry : MonoBehaviour
{
    private string m_name;
    private PatientTrainingSummary m_summary;

    private TextMeshProUGUI m_text;

    public void Initialize(string name, Color color, PatientTrainingSummary summary)
    {
        m_name = name;
        m_text = GetComponentInChildren<TextMeshProUGUI>();
        m_text.color = color;
        m_text.text = name;
        m_summary = summary;
        GetComponent<Button>().onClick.AddListener(Pushed);
    }

    private void Pushed()
    {
        //when we click this we'll use the generic message box to show the info
        //first this is we'll have to resize it
        Window_GenericMessage.Inst.GetRect().sizeDelta = new Vector2(646, 850);
        //lets also change the alignment of the text
        Window_GenericMessage.Inst.ChangeTextAlignment(TextAlignmentOptions.Left);

        //then set the title and the text
        var title = m_name;


        //todo this is very stupid to do here... this is essentially identical to how the banner_StartSummary
        //works, but this outputs a string, we should consolidate this to a single call somewhere..

        var summaryText = "";
        //common items
        summaryText += "Test Type: " + m_summary.m_testName + "\n";
        summaryText += "Background: " + m_summary.m_background + "\n";
        summaryText += "Target: " + m_summary.m_target + "\n";

        switch (m_summary.m_testType)
        {
            case TestType.Speed:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Duration: " + m_summary.m_testDuration + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Input Mode: " + m_summary.m_inputMode + "\n";
                summaryText += "Pacing: " + m_summary.m_paceMode + "\n";
                summaryText += "Auto Interval: " + m_summary.m_autopaceInterval + "\n";
                summaryText += "Hover Time: " + m_summary.m_balance_hoverTime + "\n";
                summaryText += "Decay Speed: " + m_summary.m_decayTime;
                break;
            case TestType.Peripheral:
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Duration: " + m_summary.m_testDuration + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Pacing: " + m_summary.m_paceMode + "\n";
                summaryText += "Auto Interval: " + m_summary.m_autopaceInterval + "\n";
                summaryText += "Input Mode: " + m_summary.m_inputMode + "\n";
                summaryText += "Hover Time: " + m_summary.m_balance_hoverTime + "\n";
                summaryText += "Decay Speed: " + m_summary.m_decayTime + "\n";
                summaryText += "Focused Zone: " + m_summary.m_targetArea + "\n";
                summaryText += "Focus Zone Visible: " + m_summary.m_focusZone_staysVisible + "\n";
                summaryText += "Zone Dist: " + m_summary.m_zoneDist * 100 + "%";
                break;
            case TestType.Sequence:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Display Time: " + m_summary.m_sequence_displayTime;
                summaryText += "Difficulty: " + m_summary.m_sequence_difficulty;
                break;
            case TestType.Reaction:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Pacing: " + m_summary.m_paceMode + "\n";
                summaryText += "Auto Interval: " + m_summary.m_autopaceInterval + "\n";
                summaryText += "Orientation: " + m_summary.m_reaction_orientation + "\n";
                summaryText += "Input Mode: " + m_summary.m_inputMode;
                break;
            case TestType.Balance:
                summaryText += "Target Mode: " + m_summary.m_balance_trackingMode + "\n";
                summaryText += "Tracking Direction: " + m_summary.m_balance_trackingDirection + "\n";
                summaryText += "Tracking Speed: " + m_summary.m_balance_trackingSpeed + "\n";
                summaryText += "Duration: " + m_summary.m_testDuration + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Hover Time: " + m_summary.m_balance_hoverTime + "\n";
                summaryText += "Decay Speed: " + m_summary.m_decayTime;
                break;
            case TestType.Flash:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Starting Level: " + m_summary.m_flash_startingLevel + "\n";
                summaryText += "Display Time: " + m_summary.m_sequence_displayTime;
                break;
            case TestType.GoNoGo:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Duration: " + m_summary.m_testDuration + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "No-go Frequency " + m_summary.m_goNogo_frequency + "\n";
                summaryText += "Pacing: " + m_summary.m_paceMode;
                summaryText += "Display Time: " + m_summary.m_sequence_displayTime;
                break;
            case TestType.Contrast:
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Blend Speed: " + m_summary.m_contrast_blendSpeed + "\n";
                summaryText += "Pace Time: " + m_summary.m_autopaceInterval + "\n";
                summaryText += "Big Targets: " + m_summary.m_contrast_oversizeTargets;
                break;
            case TestType.Rhythm:
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Duration: " + m_summary.m_testDuration + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Pacing: " + m_summary.m_paceMode + "\n";
                summaryText += "Auto Interval: " + m_summary.m_autopaceInterval;
                break;
            case TestType.Multi:
                summaryText += "Grid Size: " + m_summary.m_gridSize + "\n";
                summaryText += "Target Size: " + m_summary.m_targetSize + "\n";
                summaryText += "Sensitivity: " + m_summary.m_sensitivity + "\n";
                summaryText += "Audio: " + m_summary.m_audio + "\n";
                summaryText += "Countdown: " + m_summary.m_countdown + "\n";
                summaryText += "Target Sets: " + m_summary.m_multi_targetSets + "\n";
                summaryText += "Rotation Speed: " + m_summary.m_multi_rotationSpeed + "\n";
                summaryText += "Rotation Time: " + m_summary.m_multi_rotationTime + "\n";
                summaryText += "Trials: " + m_summary.m_multi_trials;

                break;
            case TestType.All:
                break;
            case TestType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Window_GenericMessage.Inst.Show(title, summaryText);
    }
}