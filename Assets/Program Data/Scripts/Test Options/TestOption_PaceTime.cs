public class TestOption_PaceTime : TestOption
{
    private readonly int[] m_paceTimes = {3, 5, 10, 15};
    public ButtonGroup m_buttons_paceTime;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons_paceTime.SetNames(m_paceTimes);
        m_buttons_paceTime.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_paceTime.Change(m_paceTimes[buttonIndex]);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_paceTime.m_list_buttons[GetIndexOfOption(m_paceTimes, m_test.m_options.m_option_paceTime.value)]
            .onClick.Invoke();
    }
}