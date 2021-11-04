public class TestOption_GridSize : TestOption
{
    private readonly float[] m_gridSizes = {51, 92, 128, 160};
    public ButtonGroup m_buttons_size;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_buttons_size.event_buttonPushed += ButtonPushed;
        m_buttons_size.SetNames(new[] {"Small", "Medium", "Large", "X-Large"});
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_gridSize.Change(m_gridSizes[buttonIndex]);
        m_test.m_options.m_option_gridSize.SetName(m_buttons_size.GetName(buttonIndex));

        Manager_Targets.Instance.ChangeOption_TargetGridSize(m_test.m_options.m_option_gridSize.value);
    }

    protected override void TestSelected()
    {
        m_buttons_size.m_list_buttons[GetIndexOfOption(m_gridSizes, m_test.m_options.m_option_gridSize.value)]
            .onClick.Invoke();
    }
}