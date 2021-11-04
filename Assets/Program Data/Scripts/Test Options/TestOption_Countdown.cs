using System;
using UnityEngine.UI;

public class TestOption_Countdown : TestOption
{
    private readonly int[] m_countdownTimes = {0, 3, 5};
    public Toggle m_toggle_cd_0, m_toggle_cd_1, m_toggle_cd_2;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_toggle_cd_0.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_cd_0, m_countdownTimes[0]); });
        m_toggle_cd_1.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_cd_1, m_countdownTimes[1]); });
        m_toggle_cd_2.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_cd_2, m_countdownTimes[2]); });
    }

    private void TogglePushed(Toggle t, int newTime)
    {
        if (!t.isOn) return;

        m_test.m_options.m_option_countdowntime.Change(newTime);
    }


    protected override void TestSelected()
    {
        base.TestSelected();

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