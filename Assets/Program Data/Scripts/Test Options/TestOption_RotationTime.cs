public class TestOption_RotationTime : TestOption
{
    private readonly float[] m_rotationTime = {3, 5, 7, 10};
    public ButtonGroup m_buttons;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons.SetNames(m_rotationTime);
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_rotationTime.Change(m_rotationTime[buttonIndex]);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons.m_list_buttons[GetIndexOfOption(m_rotationTime, m_test.m_options.m_option_rotationTime.value)]
            .onClick.Invoke();
    }
}