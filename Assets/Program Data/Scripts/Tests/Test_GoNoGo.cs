using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_GoNoGo : Test
{
    public static Test_GoNoGo Instance;

    [InfoBox("The target images for go/no.  This test ignores the globally selected target and will only use these")]
    public Sprite image_target_go;

    public Sprite image_target_noGo;

    /// <summary>
    ///     how many no-go targets were hit
    /// </summary>
    private int m_faults;

    //[HideInEditorMode] public float m_noGoFrequency;

    /// <summary>
    ///     the index of the last target we activated
    /// </summary>
    private int m_lastActiveIndex;

    private int m_presented_go, m_presented_nogo;

    // [HideInInspector]public float m_targetDisplayTime;

    //[BoxGroup("Test Specific Settings")] public int m_duration_default, m_noGoFreq_default, m_displayTime_default;
    // [BoxGroup("Test Specific Settings")] public TestOptions_GoNoGo m_testOptions;
    // [HideInInspector] public int m_last_duration, m_last_noGoFreq, m_lastDisplayTime;


    public override void Initialize()
    {
        base.Initialize();
        Instance = this;
        //very important!!!!
        //even though this test is generally called go no/go, we cannot use the slash '/' in the name when storing
        //data using the test name since windows will see that as a directory separator and screw up the file location!
        m_testName = "Go No-Go";
        m_type = TestType.GoNoGo;
        //disable this, and we'll want to track hit times manually for this test
        m_trackHitTimes = false;
        m_testIsTimed = true;
        m_list_hitTimes = new List<float>();


        //defaults
        m_options.m_option_duration.SetDefault(60);
        m_options.m_option_noGoFrequency.SetDefault(.5f);
        m_options.m_option_displayTime.SetDefault(1);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);


        // m_last_duration = m_duration_default;
        // m_last_noGoFreq = m_noGoFreq_default;
        // m_lastDisplayTime = m_displayTime_default;

        //we need to hook the target hit events
        Manager_Targets.Instance.m_targets_goNoGo.ForEach(t => t.event_targetDown += TargetHit);
    }

    public override void StartTest()
    {
        base.StartTest();

        //reset the score
        m_score = 0;
        m_faults = 0;
        m_misses = 0;
        m_targetUpTime = 0;
        m_targetHitTime = 0;
        m_presented_go = 0;
        m_presented_nogo = 0;

        //first thing we need to do is set an initial target
        //m_lastActiveIndex = Manager_Targets.Instance.GoNoGo_ActivateRandomTarget(Go_NoGo());
        ActivateRandomTarget();
    }

    public override void Selected()
    {
        base.Selected();
        //m_testOptions.m_buttonGroup_duration.m_list_buttons[m_last_duration].onClick.Invoke();
        //m_testOptions.m_buttonGroup_frequency.m_list_buttons[m_last_noGoFreq].onClick.Invoke();
        //m_testOptions.m_buttonGroup_displayTime.m_list_buttons[m_lastDisplayTime].onClick.Invoke();
    }

    protected override void TargetHit(Target t)
    {
        if (!m_isRunning) return;

        if (!t.m_isActive) //we hit a target that isn't active
            //todo this probably shouldn't be counted as a miss in this test since the subject may 
            //hit a number of targets that 'just' deactivated when trying to hit them quickly
            //or at least be an option
            //m_misses++;
            return;

        //we hit an active target, we need to see if it is go or no go
        if (t.m_image_targetSprite.sprite == image_target_go)
        {
            //this was a go target, so we can count the score and record the hit time
            m_score++;
            m_list_hitTimes.Add(m_targetHitTime);
        }
        else
        {
            m_faults++; //this is a nogo target, so this counts as a fault
        }

        ActivateRandomTarget();
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //don't add anything if the score is 0
        if (m_score <= 0 || stoppedWithButton) return;

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_goNogo_frequency = (m_options.m_option_noGoFrequency.value * 100).ToString("0") + "%";
        e.m_trainingSummary.m_goNogo_displayTime = m_options.m_option_displayTime.value;
        e.m_trainingSummary.m_faults = m_faults;
        e.m_trainingSummary.m_presented_go = m_presented_go;
        e.m_trainingSummary.m_presented_nogo = m_presented_nogo;

        FillTestDataToSummary(e);
    }

    private void ActivateRandomTarget()
    {
        //we need to reset these every time we activate a new target, otherwise, many no-gos in a row
        //will appear as a very long hit time..
        m_targetUpTime = 0;
        m_targetHitTime = 0;

        var go = Go_NoGo();
        if (go)
            m_presented_go++;
        else m_presented_nogo++;

        m_lastActiveIndex = Manager_Targets.Instance.GoNoGo_ActivateRandomTarget(go, m_lastActiveIndex);
    }

    public override string GetInfoString()
    {
        return m_score + "/" + m_misses + "/" + m_faults + "\n" +
               "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
    }

    private bool Go_NoGo()
    {
        var random = Random.value;
        //Debug.Log(random + "/" + m_noGoFrequency);
        return random >= m_options.m_option_noGoFrequency.value;
    }

    public override void Update()
    {
        base.Update();
        if (!m_isRunning) return;

        m_targetUpTime += Time.deltaTime;
        m_targetHitTime += Time.deltaTime;

        if (m_targetUpTime > m_options.m_option_displayTime.value)
        {
            //we've run out of time
            //this only counts as a miss if this was a go target
            if (Manager_Targets.Instance.m_targets_goNoGo[m_lastActiveIndex].m_image_targetSprite.sprite ==
                image_target_go)
                m_misses++;


            m_targetUpTime = 0;
            ActivateRandomTarget();
        }
    }
}