using UnityEngine;

/// <summary>
///     TestOptions_Base is a generic container to hold any given options for a test.  It handles getting/setting default
///     current and last values and should make the entire options system more maintainable and usable.  Every possible
///     option for any test is here, even if a test doesn't necessarily use it
/// </summary>
public class TestOptions_Base
{
    public TestOption<AudioClip> m_option_audioClip;
    public TestOption<bool> m_option_audioOnhit;

    public TestOption<float> m_option_autoPaceInterval;

    //background and targets
    public TestOption<string> m_option_background;

    public TestOption<bool> m_option_backgroundIsVideo;

    //balance
    public TestOption<float> m_option_balance_targetSize;
    public TestOption<TestOption_Difficulty> m_option_blendRate;
    public TestOption<int> m_option_countdowntime;

    public TestOption<float> m_option_displayTime;

    //generally universal options.  Used by at least two tests
    public TestOption<int> m_option_duration;
    public TestOption<float> m_option_focusZoneDistribution;
    public TestOption<float> m_option_focusZoneSize;
    public TestOption<bool> m_option_focusZoneStaysVisisble;
    public TestOption<float> m_option_gridSize;
    public TestOption<float> m_option_hoverTimeDecaySpeed;

    public TestOption<InputMode> m_option_inputMode;

    //go no-go
    public TestOption<float> m_option_noGoFrequency;

    public TestOption<Direction> m_option_opticFlowDirection;

    //reaction
    public TestOption<TestOption_OrientationDir> m_option_orientation;
    public TestOption<int> m_option_paceTime;

    public TestOption<TestOption_Pacing> m_option_pacing;

    //sequence
    public TestOption<TestOption_Difficulty> m_option_patternDifficulty;
    public TestOption<float> m_option_rotationSpeed;
    public TestOption<float> m_option_rotationTime;
    public TestOption<int> m_option_startingLevel;
    public TestOption<string> m_option_target;
    public TestOption<int> m_option_targetSets;
    public TestOption<float> m_option_targetSize;
    public TestOption<TargetTrackingDirection> m_option_targetTrackingDirection;
    public TestOption<TargetTrackingMode> m_option_targetTrackingMode;
    public TestOption<float> m_option_targetTrackingSpeed;

    //global options
    public TestOption<float> m_option_touchSensitivity;

    public TestOption<bool> m_option_touchSensitivity_keepVisible;

    //multi
    public TestOption<int> m_option_trialCount;

    //test specific, these only show up in a single test
    //peripheral
    public TestOption<bool> m_option_useFocusedZone;

    //contrast
    public TestOption<bool> m_option_useOversizeTargets;
    public TestOption<float> m_opton_hoverTimeGoal;
}

public struct TestOption<T>
{
    public T value { get; private set; }

    /// <summary>
    ///     If we want a value to be displayed as something different, we can set that here
    ///     example is distribution, the value is .9, but we want the name to be 90%
    /// </summary>
    public string namedValue { get; private set; }

    public bool isImplemented { get; private set; }

    public void Change(T newVal)
    {
        if (value == null)
            Debug.Log("Changing option: " + GetType() + " without setting default first..");
        value = newVal;
        isImplemented = true;
    }

    public void SetDefault(T defaultValue)
    {
        value = defaultValue;
        isImplemented = true;
    }

    public void SetName(string name)
    {
        namedValue = name;
    }
}