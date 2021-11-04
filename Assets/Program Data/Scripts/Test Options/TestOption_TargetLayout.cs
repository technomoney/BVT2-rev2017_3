public class TestOption_TargetLayout : TestOption
{
    private readonly float[] m_gridSize = {51, 92, 128, 160};
    private readonly float[] m_targetSize = {.45f, .6f, .8f, .95f};

    public ButtonGroup m_buttons_targetSize, m_buttons_gridSize;


    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons_targetSize.SetNames(new[] {"Normal", "Medium", "Large", "X-Large"});
        m_buttons_targetSize.event_buttonPushed += ButtonPushed_TargetSize;

        m_buttons_gridSize.SetNames(new[] {"Small", "Medium", "Large", "X-Large"});
        m_buttons_gridSize.event_buttonPushed += ButtonPushed_GridSize;
    }

    private void ButtonPushed_TargetSize(int buttonIndex)
    {
        //change the touch sensitivity of this test
        m_test.m_options.m_option_targetSize.Change(m_targetSize[buttonIndex]);
        m_test.m_options.m_option_targetSize.SetName(m_buttons_targetSize.GetName(buttonIndex));
    }

    private void ButtonPushed_GridSize(int buttonIndex)
    {
        //change the touch sensitivity of this test
        m_test.m_options.m_option_gridSize.Change(m_gridSize[buttonIndex]);
        m_test.m_options.m_option_gridSize.SetName(m_buttons_gridSize.GetName(buttonIndex));
    }

    protected override void TestSelected()
    {
        m_buttons_targetSize.m_list_buttons[GetIndexOfOption(m_targetSize, m_test.m_options.m_option_targetSize.value)]
            .onClick.Invoke();

        m_buttons_gridSize.m_list_buttons[GetIndexOfOption(m_gridSize, m_test.m_options.m_option_gridSize.value)]
            .onClick.Invoke();
    }
}