public class TestOption_BlendRate : TestOption
{
    public ButtonGroup m_buttons;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons.SetNames(new[] {"Slow", "Medium", "Fast"});
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons.m_list_buttons[(int) m_test.m_options.m_option_blendRate.value].onClick.Invoke();
    }

    private void ButtonPushed(int buttonIndex)
    {
        TestOption_Difficulty diff;
        if (buttonIndex == 2) diff = TestOption_Difficulty.Hard;
        else if (buttonIndex == 1) diff = TestOption_Difficulty.Medium;
        else diff = TestOption_Difficulty.Easy;

        m_test.m_options.m_option_blendRate.Change(diff);
        m_test.m_options.m_option_blendRate.SetName(m_buttons.GetName(buttonIndex));
    }
}