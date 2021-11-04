using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Flash : Test
{
    public static Test_Flash Instance;

    /// <summary>
    ///     this is the list of all targets in the current flash sequence
    /// </summary>
    private List<int> m_activatedIndeces;

    [InfoBox("How much time between the last target of a sequence being hit, and the new set of targets being shown")]
    public float m_coolDownTime;

    private float m_cooldownTime_elapsed;

    // [HideInEditorMode]public int m_startingLevel;
    //  [HideInEditorMode] public float m_displayTime;
    private int m_currentLevel;

    private bool m_haltSolutionPlayback;

    /// <summary>
    ///     how much time we've spent on the current level
    /// </summary>
    private float m_levelTime;

    private List<float> m_list_levelTimes;
    private FlashMode m_mode;

    /// <summary>
    ///     how long have we been showing the targets in display mode for
    /// </summary>
    private float m_timeDisplayed;

    /// <summary>
    ///     this is how many indices in the current sequence we have activated.  When this equals the count of
    ///     m_activatedIndices
    ///     then we have hit the entire sequence
    /// </summary>
    private int m_uniqueTargetActivated;

    // [BoxGroup("Default Settings")] public int m_startingLevel_default, m_displayTime_default;
    // [HideInInspector] public int m_last_startingLevel, m_last_displayTime;
    // [BoxGroup("Default Settings")] public TestOptions_Flash m_testOptions;


    public override void Initialize()
    {
        base.Initialize();

        Instance = this;

        m_testName = "Flash";
        m_type = TestType.Flash;

        //we'll manually track hit times for this training
        m_trackHitTimes = false;
        m_testIsTimed = false;

        //defaults
        m_options.m_option_displayTime.SetDefault(2);
        m_options.m_option_startingLevel.SetDefault(3);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);


        //m_last_startingLevel = m_startingLevel_default;
        //m_last_displayTime = m_displayTime_default;

        m_list_hitTimes = new List<float>();
        m_list_levelTimes = new List<float>();

        //hook to the targets being hit
        Manager_Targets.Instance.m_targets_flash.ForEach(t => t.event_targetDown += TargetHit);
    }

    public override void Selected()
    {
        base.Selected();
        //m_testOptions.m_buttonGroup_startingLevel.m_list_buttons[m_last_startingLevel].onClick.Invoke();
        // m_testOptions.m_buttonGroup_displayTime.m_list_buttons[m_last_displayTime].onClick.Invoke();
    }

    public override void Update()
    {
        base.Update();

        if (!m_isRunning) return;
        if (m_mode == FlashMode.ShowingSolution) return;
        if (m_mode == FlashMode.Display)
        {
            m_timeDisplayed += Time.deltaTime;

            if (m_timeDisplayed > m_options.m_option_displayTime.value)
            {
                //we're done displaying, so deactivate all targets, and go to input mode
                Manager_Targets.Instance.ClearTargets(TestType.Flash);
                m_mode = FlashMode.Input;
                m_timeDisplayed = 0;
            }
        }
        else if (m_mode == FlashMode.Input)
        {
            //we're in input mode, so we're just waiting for the subject to hit all of the targets
            m_levelTime += Time.deltaTime;
        }
        else
        {
            //cooldown
            m_cooldownTime_elapsed += Time.deltaTime;
            if (m_cooldownTime_elapsed > m_coolDownTime)
            {
                Manager_Targets.Instance.ClearTargets(TestType.Flash);
                m_mode = FlashMode.Display;
                m_cooldownTime_elapsed = 0;
                ActivateNewTargets();
            }
        }
    }

    protected override void TargetHit(Target t)
    {
        if (!m_isRunning) return;

        if (m_mode != FlashMode.Input) return;

        //see if the target we hit has an index in our list of activated indeces
        if (!m_activatedIndeces.Contains(t.m_index))
        {
            //we've hit a target that was not activated, so this is a failed test
            IncorrectTargetHit(t.m_index);
            return;
        }

        //otherwise this target is part of the current sequence
        //see if it is already active or not
        if (!t.m_isActive)
        {
            //this wasn't active yet, so increment our count and activate the target to show it is part of the sequence
            //we can manually handle the audio stuff here since in the default target handling it will only kick if the 
            //target is already active, which is won't be in Flash
            if (m_options.m_option_audioOnhit.value)
                Manager_Audio.PlaySound(Manager_Test.Instance.m_selectedTest.m_options.m_option_audioClip.value);
            m_uniqueTargetActivated++;
            t.Activate();
            m_score++;
        }

        //if it was already active we don't need to do anything, as hitting an already acitve target has no effect

        //the current level is finished we've hit unique targets equal to the sequence count
        if (m_uniqueTargetActivated == m_activatedIndeces.Count)
        {
            //add the level time to the list, and reset the timer
            m_list_levelTimes.Add(m_levelTime);
            m_levelTime = 0;
            m_currentLevel++;
            m_mode = FlashMode.Cooldown;
        }
    }

    public override void StartTest()
    {
        base.StartTest();
        m_currentLevel = m_options.m_option_startingLevel.value;

        //reset the score
        m_haltSolutionPlayback = false;
        m_score = 0;
        m_cooldownTime_elapsed = 0;
        m_mode = FlashMode.Display;
        m_uniqueTargetActivated = 0;
        m_list_levelTimes = new List<float>();
        ActivateNewTargets();
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        if (stoppedWithButton)
            //we need to halt the solution playback if it is showing, otherwise weird shit happens with the solution
            //playback continuing after the test is stopped
            m_haltSolutionPlayback = true;

        Manager_Targets.Instance.ClearTargets(TestType.Flash);
        m_mode = FlashMode.Stopped;

        //todo this is pretty much identical to the sequence test summary.. we may want to change that to something more specific eventually
        if (m_score <= 0 || stoppedWithButton) return;

        //we'll want to add the last level time to the list,
        //even though this level wasn't passed, we'll still display it
        m_list_levelTimes.Add(m_levelTime);

        //add an entry to the scoreboard
        //determine our hits at the highest level
        //figure out how many targets we hit in the highest level
        int runningScore = 0, hitsAtHighestLevel = 0;
        for (var l = m_options.m_option_startingLevel.value; l < m_currentLevel; l++)
            runningScore += l;

        //the hits at the highest level is equal to the total score - running score
        hitsAtHighestLevel = m_score - runningScore;

        //free points
        //these are given based on our starting level, if we start on something besides 1 we have to auto count
        //the targets for all previous levels, so if we start on level 4, we give points for levels 1-3 automatically
        var freePoints = 0;
        for (var x = 1; x < m_options.m_option_startingLevel.value; x++)
            freePoints += x;
        m_score += freePoints;

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);
        e.m_trainingSummary.m_flash_displayTime = m_options.m_option_displayTime.value;
        e.m_trainingSummary.m_flash_startingLevel = m_options.m_option_startingLevel.value;
        e.m_trainingSummary.m_highestlevel = m_currentLevel;
        e.m_trainingSummary.m_hitsOnHighestLevel = hitsAtHighestLevel;
        e.m_trainingSummary.m_list_leveltimes = m_list_levelTimes;
        e.m_trainingSummary.m_startingLevel = m_options.m_option_startingLevel.value;
        e.m_trainingSummary.m_levelsCompleted = m_list_levelTimes.Count;


        FillTestDataToSummary(e);
    }

    private void ActivateNewTargets()
    {
        m_uniqueTargetActivated = 0;
        m_activatedIndeces = Manager_Targets.Instance.Flash_Activate_X_RandomTargets(m_currentLevel);
    }

    public override string GetInfoString()
    {
        return "Mode: " + m_mode + "\nLevel: " + m_currentLevel + "\nScore: " + m_score;
    }

    private void IncorrectTargetHit(int incorrectIndex)
    {
        m_mode = FlashMode.ShowingSolution;
        StartCoroutine(ShowSolution(incorrectIndex));
    }

    /// <summary>
    ///     show the solution to the sequence as well as the incorret
    /// </summary>
    /// <param name="incorrectIndex"></param>
    /// <returns></returns>
    private IEnumerator ShowSolution(int incorrectIndex)
    {
        float showSolutionTime = 0, blinkTime = 0, blinkMax = .7f;
        var showGreenTargets = true;
        Manager_Targets.Instance.Flash_ShowSolution(m_activatedIndeces, incorrectIndex);
        while (showSolutionTime < ConstantDefinitions.Instance.flash_solutionShowTime)
        {
            if (m_haltSolutionPlayback) break;
            blinkTime += Time.deltaTime;
            if (blinkTime >= blinkMax)
            {
                blinkTime = 0;
                showGreenTargets = !showGreenTargets;
                if (showGreenTargets) Manager_Targets.Instance.Flash_ShowSolution(m_activatedIndeces, incorrectIndex);
                else Manager_Targets.Instance.ClearTargets(TestType.Flash);
            }

            showSolutionTime += Time.deltaTime;
            yield return null;
        }

        //we only want to manually invoke stop here if the solution playback ended normally, otherwise we will get a
        //double StopCurrentTestCall
        if (!m_haltSolutionPlayback)
            Manager_Test.Instance.StopCurrentTest();
    }
}