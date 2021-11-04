public class TestOption_TrialCount : TestOption
{
    private readonly int[] m_trialCounts_multi = {3, 5, 7};
    private readonly int[] m_trialCounts_reaction = {1, 3, 5, 7};
    public ButtonGroup m_buttons;
    private TestType m_testType;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_testType = m_buttons.m_list_buttons.Count == 3 ? TestType.Multi : TestType.Reaction;

        m_buttons.SetNames(GetCounts());
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_trialCount.Change(GetCounts()[buttonIndex]);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons.m_list_buttons[GetIndexOfOption(GetCounts(), m_test.m_options.m_option_trialCount.value)]
            .onClick.Invoke();
    }

    private int[] GetCounts()
    {
        return m_testType == TestType.Multi ? m_trialCounts_multi : m_trialCounts_reaction;
    }
}