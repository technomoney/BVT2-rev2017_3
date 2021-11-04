using System;
using UnityEngine.UI;

public class TestOption_Orientation : TestOption
{
    public Toggle m_toggle_LR, m_toggle_RL, m_toggle_TB, m_toggle_BT;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_toggle_RL.onValueChanged.AddListener(delegate { TogglePushed(TestOption_OrientationDir.RtoL); });
        m_toggle_LR.onValueChanged.AddListener(delegate { TogglePushed(TestOption_OrientationDir.LtoR); });
        m_toggle_TB.onValueChanged.AddListener(delegate { TogglePushed(TestOption_OrientationDir.TtoB); });
        m_toggle_BT.onValueChanged.AddListener(delegate { TogglePushed(TestOption_OrientationDir.BtoT); });
    }

    private void TogglePushed(TestOption_OrientationDir dir)
    {
        m_test.m_options.m_option_orientation.Change(dir);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        switch (m_test.m_options.m_option_orientation.value)
        {
            case TestOption_OrientationDir.RtoL:
                m_toggle_RL.isOn = true;
                break;
            case TestOption_OrientationDir.LtoR:
                m_toggle_LR.isOn = true;
                break;
            case TestOption_OrientationDir.TtoB:
                m_toggle_TB.isOn = true;
                break;
            case TestOption_OrientationDir.BtoT:
                m_toggle_BT.isOn = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}