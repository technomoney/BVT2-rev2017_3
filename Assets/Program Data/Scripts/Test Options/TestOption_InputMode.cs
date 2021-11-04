using System;
using UnityEngine;
using UnityEngine.UI;

public class TestOption_InputMode : TestOption
{
    private readonly float[] m_defaultDecayTimes = {0, 1, 2, 3};

    private readonly float[] m_defaultHoverTimes = {0, .25f, .5f, 1, 2}; //{1, 1.5f, 2, 3};
    public ButtonGroup m_buttons_hoverTime, m_buttons_decayTime;
    private float[] m_hoverTimes, m_decayTimes;
    public Toggle m_toggle_touch, m_toggle_balance, m_toggle_ehbMode;
    public float[] m_uniqueHoverTimes, m_uniqueDecayTimes;
    public bool m_useDefaultHoverTimes, m_useDefaultDecayTimes;


    public override void Initialize(Test test)
    {
        base.Initialize(test);

        if (m_useDefaultHoverTimes)
        {
            m_hoverTimes = m_defaultHoverTimes;
        }
        else
        {
            //commenting out these =4 checks allows for button groups with more than 4 elements..
            if (m_uniqueHoverTimes == null) // || m_uniqueHoverTimes.Length != 4)
            {
                Debug.Log("Problem assigning hover times");
                return;
            }

            m_hoverTimes = m_uniqueHoverTimes;
        }

        if (m_useDefaultDecayTimes)
        {
            m_decayTimes = m_defaultDecayTimes;
        }
        else
        {
            if (m_uniqueDecayTimes == null) // || m_uniqueDecayTimes.Length != 4)
            {
                Debug.Log("Problem assigning decay times");
                return;
            }

            m_decayTimes = m_uniqueDecayTimes;
        }

        m_buttons_hoverTime.SetNames(m_hoverTimes);
        m_buttons_hoverTime.event_buttonPushed += ButtonPushed_Hovertime;
        m_buttons_decayTime.SetNames(new[] {"Off", "Slow", "Normal", "Fast"});
        m_buttons_decayTime.event_buttonPushed += ButtonPushed_DecayTime;

        m_toggle_touch.onValueChanged.AddListener(delegate { TogglePushed(InputMode.Touch); });
        m_toggle_balance.onValueChanged.AddListener(delegate { TogglePushed(InputMode.Balance); });
        m_toggle_ehbMode.onValueChanged.AddListener(delegate { TogglePushed(InputMode.Ehb); });
    }

    private void TogglePushed(InputMode newInputMode)
    {
        m_test.m_options.m_option_inputMode.Change(newInputMode);

        Manager_Test.Instance.ChangeInputMode(m_test.m_options.m_option_inputMode.value);

        var name = "";
        switch (m_test.m_options.m_option_inputMode.value)
        {
            case InputMode.Touch:
                name = "Touch";
                break;
            case InputMode.Balance:
                name = "Balance";
                break;
            case InputMode.Ehb:
                name = "Combo";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        m_test.m_options.m_option_inputMode.SetName(name);
    }

    private void ButtonPushed_Hovertime(int buttonIndex)
    {
        m_test.m_options.m_opton_hoverTimeGoal.Change(m_hoverTimes[buttonIndex]);
    }

    private void ButtonPushed_DecayTime(int buttonIndex)
    {
        m_test.m_options.m_option_hoverTimeDecaySpeed.Change(m_decayTimes[buttonIndex]);
        m_test.m_options.m_option_hoverTimeDecaySpeed.SetName(m_buttons_decayTime.GetName(buttonIndex));
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_hoverTime.m_list_buttons[
                GetIndexOfOption(m_hoverTimes, m_test.m_options.m_opton_hoverTimeGoal.value)].onClick
            .Invoke();

        m_buttons_decayTime.m_list_buttons[
                GetIndexOfOption(m_decayTimes, m_test.m_options.m_option_hoverTimeDecaySpeed.value)]
            .onClick.Invoke();

        switch (m_test.m_options.m_option_inputMode.value)
        {
            case InputMode.Touch:
                m_toggle_touch.isOn = true;
                break;
            case InputMode.Balance:
                m_toggle_balance.isOn = true;
                break;
            case InputMode.Ehb:
                m_toggle_ehbMode.isOn = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}