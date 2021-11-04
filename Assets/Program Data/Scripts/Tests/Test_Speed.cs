using System.Collections.Generic;
using UnityEngine;

public class Test_Speed : Test
{
    public static Test_Speed Instance;

    /// <summary>
    ///     the index of the last target we activated
    /// </summary>
    private int lastActiveIndex;

    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        m_testName = "Speed";
        m_type = TestType.Speed;
        m_trackHitTimes = true;
        m_testIsTimed = true;
        Instance = this;
        m_list_hitTimes = new List<float>();

        //set our default options
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);
        m_options.m_option_duration.SetDefault(30);
        m_options.m_option_pacing.SetDefault(TestOption_Pacing.Self);
        m_options.m_option_autoPaceInterval.SetDefault(1.5f);
        m_options.m_option_inputMode.SetDefault(InputMode.Touch);
        m_options.m_opton_hoverTimeGoal.SetDefault(1f);
        m_options.m_option_hoverTimeDecaySpeed.SetDefault(0);

        //we need to hook the target hit events
        Manager_Targets.Instance.m_targets_speed.ForEach(t => t.event_targetDown += TargetHit);
        Manager_Targets.Instance.m_targets_speed.ForEach(t => t.event_triggerEnter += Target_Trigger_Enter);
        Manager_Targets.Instance.m_targets_speed.ForEach(t => t.event_triggerExit += Target_Trigger_Exit);
        Manager_Targets.Instance.m_targets_speed.ForEach(t => t.event_triggerStay += Target_Trigger_Hover);
    }

    public override void Update()
    {
        base.Update();
        if (!m_isRunning) return;

        m_targetUpTime += Time.deltaTime;

        if (m_options.m_option_pacing.value == TestOption_Pacing.Self) return;
        if (!(m_targetUpTime >= m_options.m_option_autoPaceInterval.value)) return;

        //we've reached our max time to show a target, so count this as a miss, and show a new one
        MissedTarget();
        m_targetUpTime = 0;
        ActivateRandomTarget();
    }

    public override void PreSpawnTarget()
    {
        ActivateRandomTarget();
        m_preSpawningPerformed = true;
    }

    public override void StartTest()
    {
        base.StartTest();

        //first thing we need to do is set an initial target
        if (!m_preSpawningPerformed) ActivateRandomTarget();

        //reset the score
        m_score = 0;
        m_misses = 0;
        m_targetUpTime = 0;
    }

    protected override void TargetHit(Target t)
    {
        if (!m_isRunning) return;

        //this is only valid in ehb mode if we're over the target
        if (m_options.m_option_inputMode.value == InputMode.Ehb && !m_bpCursorOverTarget) return;

        if (!t.m_isActive)
        {
            m_misses++;
            return;
        }

        //we hit a correct target, so add to the score and make a new target
        //we have to manually invoke the exit event here, see base.TargetHover..
        Target_Trigger_Exit(t);
        m_score++;
        m_targetUpTime = 0;
        ActivateRandomTarget();

        //update the hit times
        m_list_hitTimes.Add(m_targetHitTime);
        m_targetHitTime = 0;
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //don't add anything if the score is 0
        if (m_score <= 0 || stoppedWithButton) return;

        FillTestDataToSummary(null);
    }

    public override string GetInfoString()
    {
        return "Score: " + m_score + "\n" +
               "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
    }

    private void ActivateRandomTarget()
    {
        lastActiveIndex = Manager_Targets.Instance.Speed_ActivateRandomTarget(lastActiveIndex);
        if (m_options.m_option_pacing.value == TestOption_Pacing.Auto_Audio)
            Manager_Audio.PlaySound(m_options.m_option_audioClip.value);
    }
}