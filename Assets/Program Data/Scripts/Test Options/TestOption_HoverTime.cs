using UnityEngine;

public class TestOption_HoverTime : TestOption
{
    public ButtonGroup m_buttons;

    //these are pulled from the test option Input mode, they should be the same..
    private readonly float[] m_default_hoverTimes = {1, 1.5f, 2, 3};
    private float[] m_hoverTimes;
    public float[] m_uniqueHoverTimes;

    public bool m_useDefaultHoverTimes;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        if (m_useDefaultHoverTimes)
        {
            m_hoverTimes = m_default_hoverTimes;
        }
        else
        {
            if (m_uniqueHoverTimes == null || m_uniqueHoverTimes.Length == 0)
            {
                Debug.LogError("Problem assigning unique hover times");
                return;
            }

            m_hoverTimes = m_uniqueHoverTimes;
        }

        m_buttons.SetNames(m_hoverTimes);
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        //change the duration of the test to which this belongs
        m_test.m_options.m_opton_hoverTimeGoal.Change(m_hoverTimes[buttonIndex]);
    }

    protected override void TestSelected()
    {
        m_buttons.m_list_buttons[GetIndexOfOption(m_hoverTimes, m_test.m_options.m_opton_hoverTimeGoal.value)]
            .onClick.Invoke();
    }
}