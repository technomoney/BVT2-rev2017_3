using System;
using UnityEngine.UI;

public class TestOption_TargetMode : TestOption
{
    private static readonly float[] m_trackingSpeeds = {1, 2, 3.5f, 5, 7};

    public ButtonGroup m_buttons_trackingSpeed;

    /// <summary>
    ///     we have to use a flag to give the very first time the test is selected a chance to stay static, otherwise,
    ///     when the default is enabled, it will see it as 'clicking' a direction which will auto select
    ///     tracking mode, even though static is the default... of course this wouldn't be a problem if we just used
    ///     tracking as the default..
    /// </summary>
    private bool m_isSetup;

    public Toggle m_toggle_mode_static,
        m_toggle_mode_tracking,
        m_toggle_dir_horizontal,
        m_toggle_dir_vertical,
        m_toggle_dir_diagonal,
        m_toggle_dir_random;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_toggle_mode_static.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingMode(TargetTrackingMode.Static); });

        m_toggle_mode_tracking.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingMode(TargetTrackingMode.Dynamic); });

        m_toggle_dir_horizontal.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingDirection(TargetTrackingDirection.Horizontal); });
        m_toggle_dir_vertical.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingDirection(TargetTrackingDirection.Vertical); });
        m_toggle_dir_diagonal.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingDirection(TargetTrackingDirection.Diagonal); });
        m_toggle_dir_random.onValueChanged.AddListener(
            delegate { TogglePushed_TrackingDirection(TargetTrackingDirection.Random); });


        m_buttons_trackingSpeed.SetNames(new[] {"V. Slow", "Slow", "Normal", "Fast", "V. Fast"});
        m_buttons_trackingSpeed.event_buttonPushed += ButtonPushed_TrackingSpeed;
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        m_buttons_trackingSpeed
            .m_list_buttons[GetIndexOfOption(m_trackingSpeeds, m_test.m_options.m_option_targetTrackingSpeed.value)]
            .onClick.Invoke();

        //direction
        switch (m_test.m_options.m_option_targetTrackingDirection.value)
        {
            case TargetTrackingDirection.Horizontal:
                m_toggle_dir_horizontal.isOn = true;
                break;
            case TargetTrackingDirection.Vertical:
                m_toggle_dir_vertical.isOn = true;
                break;
            case TargetTrackingDirection.Diagonal:
                m_toggle_dir_diagonal.isOn = true;
                break;
            case TargetTrackingDirection.Random:
                m_toggle_dir_random.isOn = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //mode
        if (m_test.m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Static)
            m_toggle_mode_static.isOn = true;
        else m_toggle_mode_tracking.isOn = true;
    }

    private void TogglePushed_TrackingMode(TargetTrackingMode newMode)
    {
        m_test.m_options.m_option_targetTrackingMode.Change(newMode);
    }

    private void TogglePushed_TrackingDirection(TargetTrackingDirection newDirection)
    {
        m_test.m_options.m_option_targetTrackingDirection.Change(newDirection);

        if (m_isSetup)
            //automatically enable tracking mode if a tracking direction is selected
            m_toggle_mode_tracking.isOn = true;

        m_isSetup = true;
    }

    private void ButtonPushed_TrackingSpeed(int buttonIndex)
    {
        m_test.m_options.m_option_targetTrackingSpeed.Change(m_trackingSpeeds[buttonIndex]);
        m_test.m_options.m_option_targetTrackingSpeed.SetName(m_buttons_trackingSpeed.GetName(buttonIndex));
    }
}