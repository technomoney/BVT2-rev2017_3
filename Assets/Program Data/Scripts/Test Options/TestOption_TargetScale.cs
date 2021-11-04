using UnityEngine.UI;

public class TestOption_TargetScale : TestOption
{
    private readonly float[] m_targetSensitivity = {.45f, .6f, 1f, 1.2f};
    private readonly float[] m_targetSize = {.45f, .6f, .8f, .95f};

    public ButtonGroup m_buttons_targetSize, m_buttons_sensitivity;
    public Toggle m_toggle_keepVisible;


    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_buttons_targetSize.SetNames(new[] {"Normal", "Medium", "Large", "X-Large"});
        m_buttons_targetSize.event_buttonPushed += ButtonPushed_TargetSize;

        m_buttons_sensitivity.SetNames(new[] {"Small", "Medium", "Large", "X-Large"});
        m_buttons_sensitivity.event_buttonPushed += ButtonPushed_Sensitivity;

        m_toggle_keepVisible.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_keepVisible); });
    }

    private void ButtonPushed_TargetSize(int buttonIndex)
    {
        //change the touch sensitivity of this test
        m_test.m_options.m_option_targetSize.Change(m_targetSize[buttonIndex]);
        m_test.m_options.m_option_targetSize.SetName(m_buttons_targetSize.GetName(buttonIndex));
        Manager_Targets.Instance.ChangeOption_TargetGraphicScale(m_test.m_options.m_option_targetSize.value);
        Manager_Test.Instance.m_selectedTest.HandleTargetScaling();
    }

    private void ButtonPushed_Sensitivity(int buttonIndex)
    {
        //change the touch sensitivity of this test
        m_test.m_options.m_option_touchSensitivity.Change(m_targetSensitivity[buttonIndex]);
        m_test.m_options.m_option_touchSensitivity.SetName(m_buttons_sensitivity.GetName(buttonIndex));
        Manager_Targets.Instance.ChangeOption_TouchSensitivity(m_test.m_options.m_option_touchSensitivity.value);
    }

    private void TogglePushed(Toggle t)
    {
        m_test.m_options.m_option_touchSensitivity_keepVisible.Change(t.isOn);
        Manager_Targets.Instance.SetSensitivityVisibility(m_test.m_options.m_option_touchSensitivity_keepVisible.value);
    }

    protected override void TestSelected()
    {
        m_buttons_targetSize.m_list_buttons[GetIndexOfOption(m_targetSize, m_test.m_options.m_option_targetSize.value)]
            .onClick.Invoke();

        m_buttons_sensitivity
            .m_list_buttons[GetIndexOfOption(m_targetSensitivity, m_test.m_options.m_option_touchSensitivity.value)]
            .onClick.Invoke();

        m_toggle_keepVisible.isOn = m_test.m_options.m_option_touchSensitivity_keepVisible.value;
    }
}