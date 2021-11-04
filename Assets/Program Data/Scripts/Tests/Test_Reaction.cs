using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Stage
{
    /// <summary>
    ///     basically stage 0, waiting for the user to press on the initial target to initiate the test
    /// </summary>
    WaitingForInitialPress,

    /// <summary>
    ///     in self pacing, waiting for the user to lift from the inital target and begin moving to the second
    /// </summary>
    Self_WaitingForLift,

    /// <summary>
    ///     after the user lifts from the initial target, waiting for the second target to be hit
    /// </summary>
    WaitingForSecondPress,

    /// <summary>
    ///     auto, after the initial press, waiting for the cue to lift from the intialy target and move to the second
    /// </summary>
    Auto_WaitingForCue,
    None
}

public class Test_Reaction : Test
{
    public static Test_Reaction Instance;

    /// <summary>
    ///     should we be adding to the cue timing? this should only be tracked after the cue occurs, but before the lift from
    ///     the initial target
    /// </summary>
    [HideInEditorMode] public bool m_accumulateCueTiming;

    /// <summary>
    ///     how much time has passed since we started waiting for cue to now, when this equals our interval time, we can
    ///     initiate the move to next target stage
    /// </summary>
    [HideInEditorMode] public float m_cueTime;

    /// <summary>
    ///     which trial of the current series are we on?
    /// </summary>
    private int m_currentTrial;

    //public TestOption_OrientationDir m_orientation;

    public TargetHighlighter m_highlighter;

    /// <summary>
    ///     reference to the initial target in the reaction test, the one that needs to be hit or held first
    /// </summary>
    private Target m_initialTarget;

    /// <summary>
    ///     bool to check if this test was completed successfully, if not, we won't score it
    /// </summary>
    [HideInEditorMode] public bool m_misTest;

    /// <summary>
    ///     this causes the auto timing to pause and should only be true when the auto time is accumulating, but the balance
    ///     cursor has left the initial
    ///     target, as long as the bp cursor remains within the initial target, this should be false
    /// </summary>
    [HideInEditorMode] public bool m_pauseAutoTiming;

    [HideInEditorMode] public Stage m_stage;

    /// <summary>
    ///     This is the next target we're expecting the user to hit
    /// </summary>
    [HideInEditorMode] public Target m_target_nextTargetToHit;

    /// <summary>
    ///     the time it takes for the user to lift from the initial target after the cue in auto pacing
    /// </summary>
    [HideInEditorMode] public float m_timing_liftAfterCue;

    /// <summary>
    ///     The time it takes for the user to lift from the initial target to down on the second target
    /// </summary>
    [HideInEditorMode] public float m_timing_moveToSecondTarget;

    /// <summary>
    ///     an array of vector 2 to hold the (lift after cue,  move to second target) timing for each trial
    /// </summary>
    private Vector2[] m_trialResults;


    //public TestOptions_Reaction m_testOptions;
    //[BoxGroup("Test Specific Settings")] public int m_pacing_default, m_pacingInterval_default, m_orientation_default;
    //[HideInInspector] public int m_last_pacing, m_last_pacingInterval, m_last_orientation;

    // Use this for initialization
    public override void Initialize()
    {
        base.Initialize();
        Instance = this;
        m_type = TestType.Reaction;
        m_trackHitTimes = false;
        m_testIsTimed = false;
        m_testName = "Reaction";
        m_stage = Stage.None;

        //set defaults
        m_options.m_option_orientation.SetDefault(TestOption_OrientationDir.LtoR);
        m_options.m_option_pacing.SetDefault(TestOption_Pacing.Random);
        m_options.m_option_autoPaceInterval.SetDefault(1.5f);
        m_options.m_option_inputMode.SetDefault(InputMode.Touch);
        m_options.m_opton_hoverTimeGoal.SetDefault(0f);
        m_options.m_option_hoverTimeDecaySpeed.SetDefault(0);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);
        m_options.m_option_trialCount.SetDefault(3);

        //m_last_orientation = m_orientation_default;
        //m_last_pacing = m_pacing_default;
        //m_last_pacingInterval = m_pacingInterval_default;

        //hook all target events that we want to handle
        Manager_Targets.Instance.m_targets_reaction.ForEach(t => t.event_targetDown += TargetDown);
        Manager_Targets.Instance.m_targets_reaction.ForEach(t => t.event_targetUp += TargetUp);
        Manager_Targets.Instance.m_targets_reaction.ForEach(t => t.event_triggerEnter += Target_Trigger_Enter);
        Manager_Targets.Instance.m_targets_reaction.ForEach(t => t.event_triggerStay += Target_Trigger_Hover);
        Manager_Targets.Instance.m_targets_reaction.ForEach(t => t.event_triggerExit += Target_Trigger_Exit);

        Manager_Test.Instance.event_inputModeChanged += InputModeChanged;
    }

    public override void Update()
    {
        if (!m_isRunning) return;

        switch (m_stage)
        {
            case Stage.WaitingForSecondPress:
                m_timing_moveToSecondTarget += Time.deltaTime;
                break;
            case Stage.Auto_WaitingForCue:
            {
                if (m_pauseAutoTiming) break;
                m_cueTime += Time.deltaTime;
                if (m_cueTime >= m_options.m_option_autoPaceInterval.value)
                {
                    m_stage = Stage.WaitingForSecondPress;
                    m_accumulateCueTiming = true;
                    m_highlighter.MoveAndShow(m_target_nextTargetToHit.rectTransform.anchoredPosition);
                    if (m_options.m_option_pacing.value == TestOption_Pacing.Auto_Audio)
                        Manager_Audio.PlaySound(m_options.m_option_audioClip.value);
                }

                break;
            }
        }

        if (m_stage == Stage.WaitingForSecondPress && m_accumulateCueTiming)
            m_timing_liftAfterCue += Time.deltaTime;
    }

    private void InputModeChanged()
    {
        if (Manager_Test.Instance.m_testInputMode == InputMode.Touch) return;

        //in any kind of balance mode we need auto oversize targets
        var ot = Manager_TestOptions.GetTestOption<TestOption_OversizeTargets>(m_type);
        ot.m_toggle_useOversize.isOn = true;
    }

    public override void StartTest()
    {
        base.StartTest();
        HandleTargetScaling();
        m_trialResults = new Vector2[m_options.m_option_trialCount.value];
        m_currentTrial = 1;
        StartNextTrial();
    }

    private void StartNextTrial()
    {
        //each new trial is basically a reset of the entire test

        //technically this wouldn't need to be done at the start of every trial, but in case we want to 
        //add a feature where the test changes trial to trial, we'll just do it here..
        Direction dir;
        //we need to place the highlighter on the initial target
        switch (m_options.m_option_orientation.value)
        {
            case TestOption_OrientationDir.RtoL:
                dir = Direction.E;
                break;
            case TestOption_OrientationDir.LtoR:
                dir = Direction.W;
                break;
            case TestOption_OrientationDir.TtoB:
                dir = Direction.N;
                break;
            case TestOption_OrientationDir.BtoT:
                dir = Direction.S;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //reset the timing
        m_timing_liftAfterCue = 0;
        m_timing_moveToSecondTarget = 0;
        m_misTest = false;
        m_cueTime = 0;
        m_accumulateCueTiming = false;
        //move the highlighter to the correct target
        m_highlighter.MoveAndShow(
            Manager_Targets.Instance.Reaction_GetTarget(dir).rectTransform.anchoredPosition);
        m_target_nextTargetToHit = Manager_Targets.Instance.Reaction_GetTarget(dir);
        m_initialTarget = Manager_Targets.Instance.Reaction_GetTarget(dir);
        //if we're using random pacing we need to get the interval
        if (m_options.m_option_pacing.value == TestOption_Pacing.Random)
            m_options.m_option_autoPaceInterval.Change(Random.Range(.5f, 2.5f));

        //regardless of our pace mode, the initial stage is always the same
        m_stage = Stage.WaitingForInitialPress;
    }

    private void EndCurrentTrial()
    {
        if (m_misTest)
        {
            //do whatever we want to if this was a mis-test in the middle of a series... 
            //we'll just call start next trial without incrementing the trial count, this should restart the same one
            //todo may want tomse kind of warning or notice that this one was a mis-test and doesn't count..
            StartNextTrial();
            return;
        }

        //now store our times into our array for averaging later
        m_trialResults[m_currentTrial - 1] = new Vector2(m_timing_liftAfterCue, m_timing_moveToSecondTarget);

        m_currentTrial++;
        if (m_currentTrial > m_options.m_option_trialCount.value) Manager_Test.Instance.StopCurrentTest();
        else StartNextTrial();
    }

    public override void Selected()
    {
        base.Selected();
        HandleTargetScaling();
    }

    public override void HandleTargetScaling()
    {
        var newScale = Manager_Test.Instance.m_selectedTest.m_options.m_option_useOversizeTargets.value
            ? Manager_Test.Instance.m_selectedTest.m_options.m_option_targetSize.value *
              Manager_Targets.Instance.m_overSizeTargetScaling
            : Manager_Test.Instance.m_selectedTest.m_options.m_option_targetSize.value;

        foreach (var t in Manager_Targets.Instance.m_targets_reaction)
            t.ChangeOption_TargetScale(newScale);
    }

    protected override void TargetHit(Target t)
    {
        //we can only get here from balance mode, so we'll just pass it to the normal
        //target down, which has a few exceptions for which mode we're in..
        TargetDown(t);
    }

    private void TargetUp(Target t)
    {
        if (Manager_Test.Instance.m_testInputMode != InputMode.Touch) return;

        switch (m_stage)
        {
            case Stage.WaitingForInitialPress:
                //getting here used to be an error condition but since a new trial starts when the finger is still
                //holding down from the last one, this case can occur pretty regularly
                break;
            case Stage.Self_WaitingForLift:
                //we're lifting from the initial target
                m_stage = Stage.WaitingForSecondPress;
                break;
            case Stage.WaitingForSecondPress:
                //this only matters if we're in an auto pace, getting here tracks the time after cue to this lift, which is the 
                //second auto timing component
                m_accumulateCueTiming = false;
                break;
            case Stage.Auto_WaitingForCue:
                //if we lift up while waiting for a cue then this is a mis-test
                m_misTest = true;
                EndCurrentTrial();
                break;
            case Stage.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void TargetDown(Target t)
    {
        var inTouchMode = Manager_Test.Instance.m_testInputMode == InputMode.Touch;

        //if this isn't the next target we're expecting, we can ignore it
        if (t != m_target_nextTargetToHit) return;

        switch (m_stage)
        {
            case Stage.WaitingForInitialPress:
                //we've hit the initial press, so we can move on to the next stage depending on the pace mode
                SetNextTargetToHit();
                if (m_options.m_option_pacing.value == TestOption_Pacing.Self)
                {
                    //we skip waiting for lift in balance mode
                    m_stage = inTouchMode ? Stage.Self_WaitingForLift : Stage.WaitingForSecondPress;
                    m_highlighter.MoveAndShow(m_target_nextTargetToHit.rectTransform.anchoredPosition);
                }
                else
                {
                    m_stage = Stage.Auto_WaitingForCue;
                    m_highlighter.Hide();
                }

                break;
            case Stage.Self_WaitingForLift:
                break;
            case Stage.WaitingForSecondPress:
                //we've hit the second target, so the test is essentially done
                m_stage = Stage.None;
                EndCurrentTrial();
                break;
            case Stage.Auto_WaitingForCue:
                Debug.Log("TargetDown() called while stage = waitingForCue, how did this happen??");
                break;
            case Stage.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     Sets nextTargetToHit to the opposite target depending on the orientation
    /// </summary>
    private void SetNextTargetToHit()
    {
        switch (m_options.m_option_orientation.value)
        {
            case TestOption_OrientationDir.RtoL:
                m_target_nextTargetToHit = Manager_Targets.Instance.Reaction_GetTarget(Direction.W);
                break;
            case TestOption_OrientationDir.LtoR:
                m_target_nextTargetToHit = Manager_Targets.Instance.Reaction_GetTarget(Direction.E);
                break;
            case TestOption_OrientationDir.TtoB:
                m_target_nextTargetToHit = Manager_Targets.Instance.Reaction_GetTarget(Direction.S);
                break;
            case TestOption_OrientationDir.BtoT:
                m_target_nextTargetToHit = Manager_Targets.Instance.Reaction_GetTarget(Direction.N);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //hide the highlighter in case it is still showing
        m_highlighter.Hide();

        //add an entry to the scoreboard only if this wasn't a mis-test
        if (m_misTest) return;

        //don't score the test if it was stopped with the button
        if (stoppedWithButton) return;

        //if (m_timing_moveToSecondTarget <= 0) return; //is this necessary?

        //we need to combine our scores for each trial to get an average
        var total_response = 0.0f;
        var total_reaction = 0.0f;

        var trials = m_options.m_option_trialCount.value;
        //sum all of our values here
        for (var x = 0; x < trials; x++)
        {
            total_reaction += m_trialResults[x].x;
            total_response += m_trialResults[x].y;
        }

        //now average
        var avg_reaction = total_reaction / trials;
        var avg_response = total_response / trials;

        //get our fastest/slowest hits
        var list_fastest = new List<float>();
        m_trialResults.ForEach(reactionTime => list_fastest.Add(reactionTime.x));
        var fastest = list_fastest.Min();

        //var list_slowest = new List<float>();
        //m_trialResults.ForEach(responseTime => list_fastest.Add(responseTime.y));
        var slowest = list_fastest.Max();


        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_numberOfTrials = m_options.m_option_trialCount.value;
        e.m_trainingSummary.m_reaction_trialTimes = m_trialResults;

        e.m_trainingSummary.m_reaction_orientation = m_options.m_option_orientation.value;
        e.m_trainingSummary.m_shortScore = avg_reaction.ToString("0.00");
        e.m_trainingSummary.m_reaction_autoPace = m_options.m_option_pacing.value == TestOption_Pacing.Auto ||
                                                  m_options.m_option_pacing.value == TestOption_Pacing.Auto_Audio ||
                                                  m_options.m_option_pacing.value == TestOption_Pacing.Random;
        e.m_trainingSummary.m_reactionTime = avg_reaction; // m_timing_liftAfterCue;
        e.m_trainingSummary.m_responseTime = avg_response; // m_timing_moveToSecondTarget;
        e.m_trainingSummary.m_fastestHit = fastest;
        e.m_trainingSummary.m_slowestHit = slowest;
        
        //testing
        Debug.Log("F: " + fastest + "  S:" + slowest);

        FillTestDataToSummary(e);
    }

    public override string GetInfoString()
    {
        //for now..
        return m_stage.ToString();
    }


    //we have to fully override this base behavior in order for it to work correctly in this test..
    public override void Target_Trigger_Hover(Target t)
    {
        if (!m_isRunning) return;
        if (t != m_target_nextTargetToHit) return;
        if (Manager_Test.Instance.m_testInputMode != InputMode.Balance) return;

        //we only care about hovertime if we're dealing with the first target
        if (m_stage == Stage.WaitingForInitialPress)
        {
            m_hoverTimeOverTarget += Time.deltaTime;
            if (m_options.m_opton_hoverTimeGoal.value != 0) //no dividing by zero..
                BalancePlateCursor.Instance.SetProgressRing(
                    m_hoverTimeOverTarget / m_options.m_opton_hoverTimeGoal.value);

            if (!(m_hoverTimeOverTarget / m_options.m_opton_hoverTimeGoal.value >= 1)) return;
        }

        //this is a hit
        TargetHit(t);
        //reset the progress ring
        m_hoverTimeOverTarget = 0;
        BalancePlateCursor.Instance.SetProgressRing(0);

        //then manually invoke the exit since the cursor won't physically exit, and the Exit() method would never
        //get called on it's own
        //Target_Trigger_Exit(t);
    }

    public override void Target_Trigger_Enter(Target t)
    {
        base.Target_Trigger_Enter(t);
        if (m_stage == Stage.Auto_WaitingForCue && t == m_initialTarget) m_pauseAutoTiming = false;
    }

    public override void Target_Trigger_Exit(Target t)
    {
        base.Target_Trigger_Exit(t);
        if (m_stage == Stage.Auto_WaitingForCue && t == m_initialTarget) m_pauseAutoTiming = true;
        if (m_stage == Stage.WaitingForSecondPress) m_accumulateCueTiming = false;
    }
}