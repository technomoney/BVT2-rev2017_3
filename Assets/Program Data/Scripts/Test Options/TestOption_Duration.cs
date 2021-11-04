using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     How long, in seconds, does a test run for?
/// </summary>
public class TestOption_Duration : TestOption
{
    private readonly int[] m_countdownTimes = {0, 3, 5};

    private readonly int[] m_defaultTimes = {15, 30, 45, 60};

    public ButtonGroup m_buttons;
    public int[] m_durations;
    public Toggle m_toggle_cd_0, m_toggle_cd_1, m_toggle_cd_2;
    public int[] m_uniqueTimes;
    public bool m_useDefaultValues;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        if (m_useDefaultValues)
        {
            m_durations = m_defaultTimes;
        }
        else
        {
            if (m_uniqueTimes == null || m_uniqueTimes.Length != 4)
            {
                Debug.LogError("Unique times for Option: duration is null or < 4..");
                return;
            }

            m_durations = m_uniqueTimes;
        }

        m_buttons.SetNames(m_durations);
        m_buttons.event_buttonPushed += ButtonPushed;

        m_toggle_cd_0.onValueChanged.AddListener(delegate
        {
            TogglePushed_Countdown(m_toggle_cd_0, m_countdownTimes[0]);
        });
        m_toggle_cd_1.onValueChanged.AddListener(delegate
        {
            TogglePushed_Countdown(m_toggle_cd_1, m_countdownTimes[1]);
        });
        m_toggle_cd_2.onValueChanged.AddListener(delegate
        {
            TogglePushed_Countdown(m_toggle_cd_2, m_countdownTimes[2]);
        });
    }

    private void ButtonPushed(int buttonIndex)
    {
        //change the duration of the test to which this belongs
        m_test.m_options.m_option_duration.Change(m_durations[buttonIndex]);
    }

    private void TogglePushed_Countdown(Toggle t, int newTime)
    {
        if (!t.isOn) return;

        m_test.m_options.m_option_countdowntime.Change(newTime);
    }

    protected override void TestSelected()
    {
        m_buttons.m_list_buttons[GetIndexOfOption(m_durations, m_test.m_options.m_option_duration.value)]
            .onClick.Invoke();

        switch (m_test.m_options.m_option_countdowntime.value)
        {
            case 0:
                m_toggle_cd_0.isOn = true;
                break;
            case 3:
                m_toggle_cd_1.isOn = true;
                break;
            case 5:
                m_toggle_cd_2.isOn = true;
                break;
            default: throw new NotImplementedException();
        }
    }
}