public class TestOption_NoGoFrequency : TestOption
{
    private readonly float[] m_frequency = {.1f, .25f, .5f, .75f};
    public ButtonGroup m_buttons_frequency;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_buttons_frequency.SetNames(new[] {"10%", "25%", "50%", "75%"});
        m_buttons_frequency.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_noGoFrequency.Change(m_frequency[buttonIndex]);
        m_test.m_options.m_option_noGoFrequency.SetName(m_buttons_frequency.GetName(buttonIndex));
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_frequency
            .m_list_buttons[GetIndexOfOption(m_frequency, m_test.m_options.m_option_noGoFrequency.value)].onClick
            .Invoke();
    }
}