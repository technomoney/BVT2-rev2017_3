public class TestOption_TargetSize : TestOption
{
    public ButtonGroup m_buttons;
    private readonly float[] m_sizes = {150, 250, 300, 450};

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_buttons.SetNames(new[] {"Small", "Medium", "Large", "X-Large"});
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_balance_targetSize.Change(m_sizes[buttonIndex]);
        m_test.m_options.m_option_balance_targetSize.SetName(m_buttons.GetName(buttonIndex));

        //change the size of the target
        Manager_Targets.Instance.Balance_SetTargetSize(m_test.m_options.m_option_balance_targetSize.value);
    }

    protected override void TestSelected()
    {
        m_buttons.m_list_buttons[GetIndexOfOption(m_sizes, m_test.m_options.m_option_balance_targetSize.value)]
            .onClick.Invoke();
    }
}