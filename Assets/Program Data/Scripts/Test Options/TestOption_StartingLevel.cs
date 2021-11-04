using Sirenix.OdinInspector;

// ReSharper disable UnusedMember.Local

public class TestOption_StartingLevel : TestOption
{
    public ButtonGroup m_buttons_startingLevel;

    [ValueDropdown("TestType")] public TestType type;

    private static ValueDropdownList<TestType> TestType = new ValueDropdownList<TestType>
    {
        {"Flash", global::TestType.Flash},
        {"Sequence", global::TestType.Sequence}
    };

    private int[] m_startingLevels;


    public override void Initialize(Test test)
    {
        base.Initialize(test);

        //{"Sequence", new[] {2, 3, 4, 5}},
        //{"Flash", new[] {1, 3, 5, 7}}
        if (type == global::TestType.Flash)
            m_startingLevels = new[] {1, 3, 5, 7};
        else
            m_startingLevels = new[] {2, 3, 4, 5};

        m_buttons_startingLevel.SetNames(m_startingLevels);
        m_buttons_startingLevel.event_buttonPushed += ButtonPushed;
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_startingLevel
            .m_list_buttons[GetIndexOfOption(m_startingLevels, m_test.m_options.m_option_startingLevel.value)]
            .onClick
            .Invoke();
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_startingLevel.Change(m_startingLevels[buttonIndex]);
    }
}