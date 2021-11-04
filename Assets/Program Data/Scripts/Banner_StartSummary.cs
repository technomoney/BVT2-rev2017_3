using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Banner_StartSummary : MonoBehaviour
{
    public static Banner_StartSummary Instance;

    /// <summary>
    ///     How often do we update the lower summary banner?  This is easier than having every single option trigger
    ///     and event to this to rewrite the entire thing..
    /// </summary>
    private readonly float m_updateRate = .5f;


    /// <summary>
    ///     Array holding all of the text fields we have to work with
    /// </summary>
    private List<TextMeshProUGUI> m_textFields;

    // Use this for initialization
    private void Start()
    {
        Instance = this;

        //get our text fields
        m_textFields = new List<TextMeshProUGUI>();
        foreach (Transform t in transform.Find("Text Fields"))
        {
            var text = t.GetComponent<TextMeshProUGUI>();
            if (text == null) continue;
            m_textFields.Add(text);
        }

        Manager_Test.Instance.event_newTestSelected += UpdateBannerInfo;
        InvokeRepeating("UpdateBannerInfo", 2, m_updateRate);
    }

    private void UpdateBannerInfo()
    {
        var test = Manager_Test.Instance.m_selectedTest;

        //we have 15 fields to work with
        //clear all of the specific fields and we'll only use the ones we need to
        m_textFields.ForEach(sp => sp.text = string.Empty);

        //the only options we show for absolutely every test is name, bg, and target
        m_textFields[0].text = "Test: " + test.m_testName;
        m_textFields[1].text = "Background: " + Options_BackgroundSelect.Instance.m_selectedBackgroundName;
        m_textFields[2].text = "Target: " + Manager_Targets.Instance.currentTargetGraphic.name;

        //depends on the active test
        switch (Manager_Test.Instance.m_selectedTestType)
        {
            case TestType.Speed:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Duration: " + test.m_options.m_option_duration.value;
                m_textFields[8].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[9].text = "Input Mode: " + test.m_options.m_option_inputMode.namedValue;
                m_textFields[10].text = "Pacing: " + test.m_options.m_option_pacing.value;
                m_textFields[11].text = "Auto Interval: " + test.m_options.m_option_autoPaceInterval.value;
                m_textFields[12].text = "Hover Time: " + test.m_options.m_opton_hoverTimeGoal.value;
                m_textFields[13].text = "Decay Speed: " + test.m_options.m_option_hoverTimeDecaySpeed.namedValue;
                break;
            case TestType.Peripheral:
                m_textFields[3].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[4].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[5].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[6].text = "Duration: " + test.m_options.m_option_duration.value;
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Pacing: " + test.m_options.m_option_pacing.value;
                m_textFields[9].text = "Auto Interval: " + test.m_options.m_option_autoPaceInterval.value;
                m_textFields[10].text = "Input Mode: " + test.m_options.m_option_inputMode.namedValue;
                m_textFields[11].text = "Hover Time: " + test.m_options.m_opton_hoverTimeGoal.value;
                m_textFields[12].text = "Decay Speed: " + test.m_options.m_option_hoverTimeDecaySpeed.namedValue;
                m_textFields[13].text = "Focused Zone: " +
                                        (test.m_options.m_option_useFocusedZone.value
                                            ? test.m_options.m_option_focusZoneSize.namedValue
                                            : "None");
                m_textFields[14].text = "Zone Dist: " + (test.m_options.m_option_focusZoneDistribution.value * 100)
                                        .ToString("0") + "%";
                break;
            case TestType.Sequence:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Display Time: " + test.m_options.m_option_displayTime.value;
                m_textFields[9].text = "Difficulty: " + test.m_options.m_option_patternDifficulty.value;
                break;
            case TestType.Reaction:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Pacing: " + test.m_options.m_option_pacing.value;
                m_textFields[9].text = "Auto Interval: " +
                                       test.m_options.m_option_autoPaceInterval.value;
                m_textFields[10].text = "Orientation: " + test.m_options.m_option_orientation.value;
                m_textFields[11].text = "Input Mode: " + test.m_options.m_option_inputMode.namedValue;
                m_textFields[12].text = "Oversize Targets: " +
                                        (test.m_options.m_option_useOversizeTargets.value ? "Yes" : "No");
                break;
            case TestType.Balance:
                //we normally start on [3], but we can just overwrite it since this is the only test that 
                //doesn't use it..
                m_textFields[2].text = "Target Mode: " + test.m_options.m_option_targetTrackingMode.value;
                m_textFields[3].text = "Tracking Dir: " + test.m_options.m_option_targetTrackingDirection.value;
                m_textFields[4].text = "Tracking Speed: " + test.m_options.m_option_targetTrackingSpeed.namedValue;
                m_textFields[5].text = "Duration: " + test.m_options.m_option_duration.value;
                m_textFields[6].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[7].text = "Target Size: " + test.m_options.m_option_balance_targetSize.namedValue;
                m_textFields[8].text = "Hover Time: " + test.m_options.m_opton_hoverTimeGoal.value;
                m_textFields[9].text = "Decay Speed: " + test.m_options.m_option_hoverTimeDecaySpeed.namedValue;
                break;
            case TestType.GoNoGo:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Duration: " + test.m_options.m_option_duration.value;
                m_textFields[8].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[9].text = "No-Go Frequency: " + test.m_options.m_option_noGoFrequency.namedValue;
                m_textFields[10].text = "Pacing: " + test.m_options.m_option_pacing.value;
                m_textFields[11].text = "Display Time: " + test.m_options.m_option_displayTime.value;
                break;
            case TestType.Flash:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Starting Level: " + test.m_options.m_option_startingLevel.value;
                m_textFields[9].text = "Display Time: " + test.m_options.m_option_displayTime.value;
                break;
            case TestType.Rhythm:
                m_textFields[3].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[4].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[5].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[6].text = "Duration: " + test.m_options.m_option_duration.value;
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Pacing: " + test.m_options.m_option_pacing.value;
                m_textFields[9].text = "Auto Interval: " + test.m_options.m_option_autoPaceInterval.value;
                break;
            case TestType.Contrast:
                m_textFields[3].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[4].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[5].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[6].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[7].text = "Blend Rate: " + test.m_options.m_option_blendRate.namedValue;
                m_textFields[8].text = "Pacing: " + test.m_options.m_option_autoPaceInterval.value;
                m_textFields[9].text = "Oversize Targets: " +
                                       (test.m_options.m_option_useOversizeTargets.value ? "Yes" : "No");
                break;
            case TestType.Multi:
                m_textFields[3].text = "Grid Size: " + test.m_options.m_option_gridSize.namedValue;
                m_textFields[4].text = "Target Size: " + test.m_options.m_option_targetSize.namedValue;
                m_textFields[5].text = "Sensitivity: " + test.m_options.m_option_touchSensitivity.namedValue;
                m_textFields[6].text = "Audio: " + (test.m_options.m_option_audioOnhit.value
                                           ? test.m_options.m_option_audioClip.value.name
                                           : "Off");
                m_textFields[7].text = "Countdown: " + test.m_options.m_option_countdowntime.value;
                m_textFields[8].text = "Target Sets: " + test.m_options.m_option_targetSets.value;
                m_textFields[9].text = "Rotation Speed: " + test.m_options.m_option_rotationSpeed.namedValue;
                m_textFields[10].text = "Rotation Time: " + test.m_options.m_option_rotationTime.value;
                m_textFields[11].text = "Trials: " + test.m_options.m_option_trialCount.value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}