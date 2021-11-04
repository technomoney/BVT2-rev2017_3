using UnityEngine.UI;

public class TestOption_TouchSensitivity : TestOption
{
    private readonly float[] m_sensitivities = {.45f, .6f, 1f, 1.2f};
    public ButtonGroup m_buttons;
    public Toggle m_toggle_keepVisible;


    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons.SetNames(new[] {"Normal", "Medium", "Large", "X-Large"});
        m_buttons.event_buttonPushed += ButtonPushed;
        m_toggle_keepVisible.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_keepVisible); });
    }

    private void ButtonPushed(int buttonIndex)
    {
        //change the touch sensitivity of this test
        m_test.m_options.m_option_touchSensitivity.Change(m_sensitivities[buttonIndex]);
        m_test.m_options.m_option_touchSensitivity.SetName(m_buttons.GetName(buttonIndex));
    }

    private void TogglePushed(Toggle t)
    {
        m_test.m_options.m_option_touchSensitivity_keepVisible.Change(t.isOn);
    }

    protected override void TestSelected()
    {
        m_buttons.m_list_buttons[GetIndexOfOption(m_sensitivities, m_test.m_options.m_option_touchSensitivity.value)]
            .onClick.Invoke();

        m_toggle_keepVisible.isOn = m_test.m_options.m_option_touchSensitivity_keepVisible.value;
    }
}