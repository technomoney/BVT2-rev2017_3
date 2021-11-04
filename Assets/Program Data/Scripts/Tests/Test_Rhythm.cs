using System.Collections.Generic;
using UnityEngine;

public class Test_Rhythm : Test
{
    public static Test_Rhythm Instance;
    private List<float> m_hitTimes_left, m_hitTimes_right;
    private int m_misses_right, m_misses_left;
    private int m_presented_left, m_presented_right;
    private bool m_targetSide_left;

    //[BoxGroup("Default Settings")] public int m_duration_default, m_pacing_default, m_autoPaceInt_default;
    //[BoxGroup("Default Settings")] public TestOptions_Rhythm m_testOptions;
    //[HideInInspector] public int m_last_duration, m_last_pacing, m_last_autoPaceInt;


    public override void Initialize()
    {
        base.Initialize();
        m_testName = "Rhythm";
        m_type = TestType.Rhythm;
        Instance = this;
        m_trackHitTimes = true;
        m_testIsTimed = true;


        //defaults
        m_options.m_option_duration.SetDefault(60);
        m_options.m_option_pacing.SetDefault(TestOption_Pacing.Self);
        m_options.m_option_autoPaceInterval.SetDefault(1.5f);
        m_options.m_opton_hoverTimeGoal.SetDefault(1f);
        m_options.m_option_hoverTimeDecaySpeed.SetDefault(0);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_countdowntime.SetDefault(3);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);

        //m_last_duration = m_duration_default;
        //m_last_pacing = m_pacing_default;
        //m_last_autoPaceInt = m_autoPaceInt_default;

        m_list_hitTimes = new List<float>();

        //hook the target hit event for this test
        Manager_Targets.Instance.m_target_rhythm.event_targetDown += TargetHit;
    }

    public override void Selected()
    {
        base.Selected();

        //m_testOptions.m_buttonGroup_duration.m_list_buttons[m_last_duration].onClick.Invoke();
        //m_testOptions.SetOption_PaceMode(m_last_pacing);
        //m_testOptions.m_buttonGroup_autoInterval.m_list_buttons[m_last_autoPaceInt].onClick.Invoke();
    }

    public override void Update()
    {
        //testing
        //if (Input.GetKey(KeyCode.R))
        //Manager_Targets.Instance.Rhythm_ActivateTarget(m_targetSide_left);
        //if (Input.GetKey(KeyCode.T))
        //Manager_Targets.Instance.Rhythm_ActivateTarget(!m_targetSide_left);

        base.Update();
        if (!m_isRunning) return;

        m_targetUpTime += Time.deltaTime;

        if (m_options.m_option_pacing.value == TestOption_Pacing.Self) return;
        if (!(m_targetUpTime >= m_options.m_option_autoPaceInterval.value)) return;

        //we've reached our max time to show a target, so count this as a miss, and show a new one
        MissedTarget();
        m_targetUpTime = 0;
        //activate a new target
        ShowNewTarget();
    }

    public override void StartTest()
    {
        base.StartTest();

        //first thing is pick a side to spawn the target on
        var coin = Random.Range(0, 2);
        m_targetSide_left = coin == 0;

        //then spawn the target
        ShowNewTarget();

        //reset the score
        m_score = 0;
        m_misses = 0;
        m_targetUpTime = 0;
        m_presented_left = 0;
        m_presented_right = 0;
        m_misses_right = m_misses_left = 0;
        m_hitTimes_left = new List<float>();
        m_hitTimes_right = new List<float>();
    }

    protected override void MissedTarget()
    {
        base.MissedTarget();

        //when we miss we want to keep track of left and right misses separately
        if (m_targetSide_left)
            m_misses_left++;
        else m_misses_right++;
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);
        //reset the target position
        Manager_Targets.Instance.m_target_rhythm.rectTransform.anchoredPosition = Vector2.zero;

        //add an entry to the scoreboard
        //don't add anything if the score is 0
        if (m_score <= 0 || stoppedWithButton) return;

        //prepare the scoreboard entry for the graphs and such
        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_misses_left = m_misses_left;
        e.m_trainingSummary.m_misses_right = m_misses_right;
        e.m_trainingSummary.m_hitTimes_left = m_hitTimes_left;
        e.m_trainingSummary.m_hitTimes_right = m_hitTimes_right;
        e.m_trainingSummary.m_presented_left = m_presented_left;
        e.m_trainingSummary.m_presented_right = m_presented_right;

        FillTestDataToSummary(e);
    }

    protected override void TargetHit(Target t)
    {
        if (!m_isRunning) return;

        if (!t.m_isActive) return;

        //we hit the target
        m_score++;
        m_targetUpTime = 0;

        //update the hit times
        m_list_hitTimes.Add(m_targetHitTime);
        if (m_targetSide_left) m_hitTimes_left.Add(m_targetHitTime);
        else m_hitTimes_right.Add(m_targetHitTime);
        m_targetHitTime = 0;

        //activate a new target
        ShowNewTarget();
    }

    private void ShowNewTarget()
    {
        if (m_options.m_option_pacing.value == TestOption_Pacing.Auto_Audio)
            Manager_Audio.PlaySound(m_options.m_option_audioClip.value);

        m_targetSide_left = !m_targetSide_left;
        Manager_Targets.Instance.Rhythm_ActivateTarget(m_targetSide_left);
        if (m_targetSide_left) m_presented_left++;
        else m_presented_right++;
    }

    public override string GetInfoString()
    {
        return "Score: " + m_score + "\n" +
               "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
    }
}