using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Peripheral : Test
{
    public static Test_Peripheral Instance;

    /// <summary>
    ///     The radius of the focus zone with respect to its scale, this tells us how far from the origin we can move
    ///     the target when in focused mode
    /// </summary>
    public float m_radius_focusZone;

    public RectTransform m_rectTransform_focusZone;

    [PropertyTooltip("Pushing space will auto spawn a new target, for testing only")]
    public bool m_SpaceForNewTarget;

    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();

        Instance = this;
        m_testName = "Peripheral";
        m_type = TestType.Peripheral;
        m_trackHitTimes = true;
        m_testIsTimed = true;

        //set our default options
        m_options.m_option_duration.SetDefault(30);
        m_options.m_option_focusZoneDistribution.SetDefault(.5f);
        m_options.m_option_focusZoneSize.SetDefault(9);
        m_options.m_option_focusZoneStaysVisisble.SetDefault(false);
        m_options.m_option_useFocusedZone.SetDefault(false);
        m_options.m_option_pacing.SetDefault(TestOption_Pacing.Self);
        m_options.m_option_autoPaceInterval.SetDefault(1.5f);
        m_options.m_option_inputMode.SetDefault(InputMode.Touch);
        m_options.m_opton_hoverTimeGoal.SetDefault(.25f);
        m_options.m_option_hoverTimeDecaySpeed.SetDefault(0);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_countdowntime.SetDefault(3);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);


        //the focus zone should be hidden initially
        m_rectTransform_focusZone.gameObject.SetActive(false);

        //we need to hook the target hit events
        Manager_Targets.Instance.m_target_peripheral.event_targetDown += TargetHit;

        //we need to override the events for dealing with balance input mode
        Manager_Targets.Instance.m_target_peripheral.event_triggerEnter += Target_Trigger_Enter;
        Manager_Targets.Instance.m_target_peripheral.event_triggerStay += Target_Trigger_Hover;
        Manager_Targets.Instance.m_target_peripheral.event_triggerExit += Target_Trigger_Exit;

        //we want to subscribe to the input mode being changed so we can auto selected focused mode if
        //we change to balance mode
        Manager_Test.Instance.event_inputModeChanged += InputModeChanged;
    }

// Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!m_isRunning) return;

        if (m_SpaceForNewTarget && Input.GetKey(KeyCode.Space))
            Manager_Targets.Instance.Peripheral_MoveTarget_FullScreen();

        m_targetUpTime += Time.deltaTime;
        if (m_options.m_option_pacing.value == TestOption_Pacing.Self) return;
        if (m_targetUpTime < m_options.m_option_autoPaceInterval.value) return;

        //we've reached our max time to show a target, so count this as a miss, and move the target
        MissedTarget();
        //if we're in balance or ehb mode, reset the progress ring in case it was moving up while this happens
        if (m_options.m_option_inputMode.value != InputMode.Touch)
        {
            BalancePlateCursor.Instance.SetProgressRing(0);
            m_hoverTimeOverTarget = 0;
        }

        m_targetUpTime = 0;
        MoveTargetToNewPosition();
    }

    public override void PreSpawnTarget()
    {
        //since we only have 1 target it should always be active
        Manager_Targets.Instance.m_target_peripheral.Activate();

        if (m_options.m_option_useFocusedZone.value)
        {
            //we're in focused mode
            //zone visibility
            m_rectTransform_focusZone.gameObject.SetActive(m_options.m_option_focusZoneStaysVisisble.value);
            //radius
            m_radius_focusZone = m_rectTransform_focusZone.rect.width * m_options.m_option_focusZoneSize.value / 2;
        }
        else
        {
            m_rectTransform_focusZone.gameObject.SetActive(false);
        }


        MoveTargetToNewPosition();

        m_preSpawningPerformed = true;
    }

    public override void StartTest()
    {
        base.StartTest();

        m_score = 0;
        m_misses = 0;
        m_targetUpTime = 0;

        if (m_preSpawningPerformed) return;

        //since we only have 1 target it should always be active
        Manager_Targets.Instance.m_target_peripheral.Activate();

        if (m_options.m_option_useFocusedZone.value)
        {
            //we're in focused mode
            //zone visibility
            m_rectTransform_focusZone.gameObject.SetActive(m_options.m_option_focusZoneStaysVisisble.value);
            //radius
            m_radius_focusZone = m_rectTransform_focusZone.rect.width * m_options.m_option_focusZoneSize.value / 2;
        }
        else
        {
            m_rectTransform_focusZone.gameObject.SetActive(false);
        }

        MoveTargetToNewPosition();
    }

    protected override void TargetHit(Target t)
    {
        //this is only valid in ehb mode if we're over the target
        if (m_options.m_option_inputMode.value == InputMode.Ehb && !m_bpCursorOverTarget) return;

        m_score++;
        m_targetUpTime = 0;
        MoveTargetToNewPosition();

        //update the hit times
        m_list_hitTimes.Add(m_targetHitTime);
        m_targetHitTime = 0;
    }


    private void MoveTargetToNewPosition()
    {
        //todo could have a magnitude that makes targets appear X distance away from each other..
        if (m_options.m_option_useFocusedZone.value)
            Manager_Targets.Instance.Peripheral_MoveTarget_FocusedZone();
        else Manager_Targets.Instance.Peripheral_MoveTarget_FullScreen();

        if (m_options.m_option_pacing.value == TestOption_Pacing.Auto_Audio)
            Manager_Audio.PlaySound(m_options.m_option_audioClip.value);
    }

    public override string GetInfoString()
    {
        return "Score: " + m_score + "\n" +
               "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //deactivate the target and move it back to the middle
        Manager_Targets.Instance.m_target_peripheral.Deactivate();
        Manager_Targets.Instance.m_target_peripheral.rectTransform.anchoredPosition = Vector2.zero;

        if (m_score <= 0 || stoppedWithButton) return; //don't make a summary if the score is 0

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);
        e.m_trainingSummary = new PatientTrainingSummary
        {
            m_zoneSize = m_options.m_option_focusZoneSize.value,
            m_zoneDist = m_options.m_option_focusZoneDistribution.value,
            m_targetArea = m_options.m_option_useFocusedZone.value ? "Focused Zone" : "Fullscreen",
            m_focusZone_staysVisible = m_options.m_option_focusZoneStaysVisisble.value
        };

        FillTestDataToSummary(e);
    }

    private void InputModeChanged()
    {
        if (Manager_Test.Instance.m_selectedTest != this) return;

        if (Manager_Test.Instance.m_testInputMode == InputMode.Touch) return;

        //then manually set the toggles for the focus zone options

        //m_option_focusedZone.m_toggle_keepVisible.isOn = false;
        //m_option_focusedZone.m_toggle_useFocusedZone.isOn = true;
        var fz = Manager_TestOptions.GetTestOption<TestOption_FocusedZone>(m_type);
        fz.m_toggle_keepVisible.isOn = false;
        fz.m_toggle_useFocusedZone.isOn = true;
        //click the 4th button, 100%
        fz.m_buttons_distribution.m_list_buttons[3].onClick.Invoke();

        //change the background and target
        Options_BackgroundSelect.Instance.SetBackgroundByName("Solid Black", false);
        Options_TargetSelect.Instance.SetTargetByName("Circle Green");

        var ts = Manager_TestOptions.GetTestOption<TestOption_TargetScale>(m_type);
        //change the sensitivity to x-large
        ts.m_buttons_sensitivity.m_list_buttons[3].onClick.Invoke();
        //change the target size to large
        ts.m_buttons_targetSize.m_list_buttons[2].onClick.Invoke();
    }
}