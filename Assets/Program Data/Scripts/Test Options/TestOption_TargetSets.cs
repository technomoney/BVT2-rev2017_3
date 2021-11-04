public class TestOption_TargetSets : TestOption
{
    private readonly int[] m_targetSets = {1, 2, 3, 4};
    public ButtonGroup m_buttons;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons.SetNames(m_targetSets);
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_targetSets.Change(m_targetSets[buttonIndex]);

        Manager_Targets.Instance.Multi_ChangeTargetSets(m_test.m_options.m_option_targetSets.value);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons.m_list_buttons[GetIndexOfOption(m_targetSets, m_test.m_options.m_option_targetSets.value)]
            .onClick.Invoke();
    }
}