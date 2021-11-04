using UnityEngine;
using UnityEngine.UI;

public class TestOption_FocusedZone : TestOption
{
    private readonly float[] m_distributions = {0, .5f, .7f, 1};
    private readonly float[] m_focusZoneSizes = {5, 7, 9, 10};
    public Button m_button_position_set, m_button_position_reset;
    public ButtonGroup m_buttons_size, m_buttons_distribution;
    public Toggle m_toggle_useFocusedZone, m_toggle_keepVisible;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_toggle_keepVisible.onValueChanged.AddListener(delegate { ToggleChanged_KeepVisible(m_toggle_keepVisible); });
        m_toggle_useFocusedZone.onValueChanged.AddListener(delegate
        {
            ToggleChanged_UseFocusedZone(m_toggle_useFocusedZone);
        });

        m_buttons_distribution.SetNames(new[] {"0%", "50%", "70%", "100%"});
        m_buttons_distribution.event_buttonPushed += ButtonPushed_Distribution;
        m_buttons_size.SetNames(new[] {"Small", "Medium", "Large", "X-Large"});
        m_buttons_size.event_buttonPushed += ButtonPushed_Size;

        m_button_position_set.onClick.AddListener(ButtonPushed_SetFocusedZone);
        m_button_position_reset.onClick.AddListener(ButtonPushed_ResetFocusedZone);
    }

    protected override void TestSelected()
    {
        base.TestSelected();
        m_buttons_size.m_list_buttons[GetIndexOfOption(m_focusZoneSizes,
            m_test.m_options.m_option_focusZoneSize.value)].onClick.Invoke();

        m_buttons_distribution.m_list_buttons[GetIndexOfOption(m_distributions,
            m_test.m_options.m_option_focusZoneDistribution.value)].onClick.Invoke();

        m_toggle_keepVisible.isOn = m_test.m_options.m_option_focusZoneStaysVisisble.value;
        m_toggle_useFocusedZone.isOn = m_test.m_options.m_option_useFocusedZone.value;
    }

    private void ToggleChanged_UseFocusedZone(Toggle t)
    {
        m_test.m_options.m_option_useFocusedZone.Change(t.isOn);

        //if we're turing off focused zone, we want to auto uncheck keep visible
        if (!t.isOn) m_toggle_keepVisible.isOn = false;

        //show or hide the focus zone
        Test_Peripheral.Instance.m_rectTransform_focusZone.gameObject.SetActive(m_test.m_options.m_option_useFocusedZone
            .value);
    }

    private void ToggleChanged_KeepVisible(Toggle t)
    {
        m_test.m_options.m_option_focusZoneStaysVisisble.Change(t.isOn);
        //if we are enabling this with focus zone off, turn it on
        if (t.isOn) m_toggle_useFocusedZone.isOn = true;
    }

    private void ButtonPushed_Size(int buttonIndex)
    {
        m_test.m_options.m_option_focusZoneSize.Change(m_focusZoneSizes[buttonIndex]);
        m_test.m_options.m_option_focusZoneSize.SetName(m_buttons_size.GetName(buttonIndex));

        //now change the actual size of the focused zone
        Test_Peripheral.Instance.m_rectTransform_focusZone.localScale = new Vector2(
            m_test.m_options.m_option_focusZoneSize.value,
            m_test.m_options.m_option_focusZoneSize.value);
    }

    private void ButtonPushed_Distribution(int buttonIndex)
    {
        m_test.m_options.m_option_focusZoneDistribution.Change(m_distributions[buttonIndex]);
    }

    private void ButtonPushed_SetFocusedZone()
    {
        //when we go to set the focus zone, we want to flip to focused mode automatically
        m_toggle_useFocusedZone.isOn = true;
        //then show the tool to set the origin
        Manager_Test.Instance.SetFocusZoneOrigin(true);
    }

    private void ButtonPushed_ResetFocusedZone()
    {
        Test_Peripheral.Instance.m_rectTransform_focusZone.anchoredPosition = Vector2.zero;
    }
}