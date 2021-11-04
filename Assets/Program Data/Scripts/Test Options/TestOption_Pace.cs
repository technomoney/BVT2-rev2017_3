using System;
using UnityEngine;
using UnityEngine.UI;

public class TestOption_Pace : TestOption
{
    private readonly float[] m_default_autoPaceIntervals = {.25f, .5f, 1, 1.5f};

    private float[] m_autoPaceIntervals;
    public ButtonGroup m_buttons_autoPaceTime;
    public Toggle m_toggle_self, m_toggle_auto, m_toggle_autoAudio, m_toggle_random;
    public float[] m_uniqueTimes;
    public bool m_useDefaultValues = true;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        if (m_useDefaultValues)
        {
            m_autoPaceIntervals = m_default_autoPaceIntervals;
        }
        else
        {
            if (m_uniqueTimes == null || m_uniqueTimes.Length != 4)
            {
                Debug.Log("Unique times for Option: auto pace interval is null or < 4..");
                return;
            }

            m_autoPaceIntervals = m_uniqueTimes;
        }

        m_buttons_autoPaceTime.SetNames(m_autoPaceIntervals);
        m_buttons_autoPaceTime.event_buttonPushed += ButtonPushed_AutoPaceTime;
        m_toggle_self.onValueChanged.AddListener(delegate { TogglePushed(TestOption_Pacing.Self); });
        m_toggle_auto.onValueChanged.AddListener(delegate { TogglePushed(TestOption_Pacing.Auto); });
        m_toggle_autoAudio.onValueChanged.AddListener(delegate { TogglePushed(TestOption_Pacing.Auto_Audio); });
        m_toggle_random.onValueChanged.AddListener(delegate { TogglePushed(TestOption_Pacing.Random); });
    }

    private void ButtonPushed_AutoPaceTime(int buttonIndex)
    {
        m_test.m_options.m_option_autoPaceInterval.Change(m_autoPaceIntervals[buttonIndex]);
    }

    protected void TogglePushed(TestOption_Pacing newPacingMode)
    {
        m_test.m_options.m_option_pacing.Change(newPacingMode);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_autoPaceTime.m_list_buttons
                [GetIndexOfOption(m_autoPaceIntervals, m_test.m_options.m_option_autoPaceInterval.value)]
            .onClick.Invoke();

        switch (m_test.m_options.m_option_pacing.value)
        {
            case TestOption_Pacing.Self:
                m_toggle_self.isOn = true;
                break;
            case TestOption_Pacing.Auto:
                m_toggle_auto.isOn = true;
                break;
            case TestOption_Pacing.Auto_Audio:
                m_toggle_auto.isOn = true;
                break;
            case TestOption_Pacing.Random:
                m_toggle_random.isOn = true;
                break;
            case TestOption_Pacing.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}