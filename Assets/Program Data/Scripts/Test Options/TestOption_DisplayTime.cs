using Sirenix.OdinInspector;

public class TestOption_DisplayTime : TestOption
{
    public ButtonGroup m_buttons_displayTime;


    [ValueDropdown("TestType")] public TestType type;

    private static ValueDropdownList<TestType> TestType = new ValueDropdownList<TestType>
    {
        {"Flash", global::TestType.Flash},
        {"Sequence", global::TestType.Sequence},
        {"Go No-Go", global::TestType.GoNoGo}
    };

    private float[] m_displayTimes;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        
        //{"Sequence", new[] {1, 1.5f, 3, 5}},
        //{"Go No-Go", new[] {.8f, 1f, 1.5f, 2f}},
        //{"Flash", new[] {1f, 2, 3, 5}}
        if (type == global::TestType.Sequence)
            m_displayTimes = new[] {1, 1.5f, 3, 5};
        else if (type == global::TestType.GoNoGo)
            m_displayTimes = new[] {.8f, 1f, 1.5f, 2f};
        else m_displayTimes = new[] {1f, 2, 3, 5};
        
        m_buttons_displayTime.SetNames(m_displayTimes);
        m_buttons_displayTime.event_buttonPushed += ButtonPushed;
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_displayTime
            .m_list_buttons[GetIndexOfOption(m_displayTimes, m_test.m_options.m_option_displayTime.value)].onClick
            .Invoke();
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_displayTime.Change(m_displayTimes[buttonIndex]);
    }
}
