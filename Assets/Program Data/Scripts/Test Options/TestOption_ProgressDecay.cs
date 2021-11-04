public class TestOption_ProgressDecay : TestOption
{
    public ButtonGroup m_buttons;

    //these are pulled from the test option Input mode, they should be the same..
    private readonly float[] m_decayTimes = {0, 1, 2, 3};

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_buttons.SetNames(new[] {"Off", "Slow", "Normal", "Fast"});
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        //change the duration of the test to which this belongs
        m_test.m_options.m_option_hoverTimeDecaySpeed.Change(m_decayTimes[buttonIndex]);
        m_test.m_options.m_option_hoverTimeDecaySpeed.SetName(m_buttons.GetName(buttonIndex));
    }

    protected override void TestSelected()
    {
        m_buttons.m_list_buttons[GetIndexOfOption(m_decayTimes, m_test.m_options.m_option_hoverTimeDecaySpeed.value)]
            .onClick.Invoke();
    }
}