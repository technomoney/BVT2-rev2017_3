﻿public class TestOption_RotationSpeed : TestOption
{
    private readonly float[] m_rotationSpeed = {3, 5, 7, 10};
    public ButtonGroup m_buttons;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons.SetNames(new[] {"Slow", "Medium", "Fast", "V. Fast"});
        m_buttons.event_buttonPushed += ButtonPushed;
    }

    private void ButtonPushed(int buttonIndex)
    {
        m_test.m_options.m_option_rotationSpeed.Change(m_rotationSpeed[buttonIndex]);
        m_test.m_options.m_option_rotationSpeed.SetName(m_buttons.GetName(buttonIndex));
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons.m_list_buttons[GetIndexOfOption(m_rotationSpeed, m_test.m_options.m_option_rotationSpeed.value)]
            .onClick.Invoke();
    }
}