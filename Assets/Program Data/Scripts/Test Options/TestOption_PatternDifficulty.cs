public class TestOption_PatternDifficulty : TestOption
{
    public ButtonGroup m_buttons_difficulty;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_buttons_difficulty.SetNames(new[] {"Easy", "Medium", "Hard"});
        m_buttons_difficulty.event_buttonPushed += ButtonPushed;
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_difficulty.m_list_buttons[(int) m_test.m_options.m_option_patternDifficulty.value].onClick
            .Invoke();
    }

    private void ButtonPushed(int buttonIndex)
    {
        TestOption_Difficulty diff;
        if (buttonIndex == 2) diff = TestOption_Difficulty.Hard;
        else if (buttonIndex == 1) diff = TestOption_Difficulty.Medium;
        else diff = TestOption_Difficulty.Easy;

        m_test.m_options.m_option_patternDifficulty.Change(diff);

        Manager_Targets.Instance.Memory_ShowTargetsByDifficulty();
    }
}