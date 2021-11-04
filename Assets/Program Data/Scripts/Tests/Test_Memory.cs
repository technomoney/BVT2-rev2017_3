using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class Test_Memory : Test
{
    public static Test_Memory Instance;
    private int m_currentInputCount;
    private int m_currentLevel;
    private float m_currentTime;
    private bool m_haltPlayback;
    private List<int> m_list_displayOrder;
    private List<float> m_list_levelTimes;
    [HideInEditorMode] private MemoryMode m_mode;
    public TextMeshProUGUI pfb_solutionLabel;

    public override void Initialize()
    {
        base.Initialize();
        Instance = this;
        m_testName = "Sequence";
        m_type = TestType.Sequence;
        m_mode = MemoryMode.Playback;
        m_trackHitTimes = false;
        m_testIsTimed = false;

        m_list_displayOrder = new List<int>();
        m_list_levelTimes = new List<float>();

        //set our default options
        m_options.m_option_displayTime.SetDefault(1.5f);
        m_options.m_option_patternDifficulty.SetDefault(TestOption_Difficulty.Medium);
        m_options.m_option_startingLevel.SetDefault(3);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);

        //we need to hook the target hit events
        Manager_Targets.Instance.m_targets_memory.ForEach(t => t.event_targetDown += TargetHit);
    }

    public override void StartTest()
    {
        base.StartTest();

        //reset the score
        m_score = 0;
        m_list_levelTimes = new List<float>();
        //clear the lists and such
        m_list_displayOrder.Clear();


        //rest the timer
        m_currentTime = 0;
        m_currentInputCount = 0;

        m_currentLevel = m_options.m_option_startingLevel.value;
        //get our sequence
        m_haltPlayback = false; //reset this in case it was halted previously
        GetInitialSequence(m_currentLevel);
        StartCoroutine(PlaybackSequence());
    }


    /// <summary>
    ///     Set the initial pattern depending on the starting level
    /// </summary>
    /// <param name="currentLevel"></param>
    private void GetInitialSequence(int currentLevel)
    {
        m_list_displayOrder = new List<int>();
        var lastIndex = -99;
        for (var x = 0; x < currentLevel; x++)
        {
            lastIndex = Manager_Targets.Instance.Memory_GetRandomIndex(m_options.m_option_patternDifficulty.value,
                lastIndex);
            m_list_displayOrder.Add(lastIndex);
        }
    }

    /// <summary>
    ///     Add a target to the current sequence
    /// </summary>
    private void AddTargetToSequence()
    {
        //get a new index using the last entry of the current list as the lastIndex so we don't get a repeat..
        var newIndex = Manager_Targets.Instance.Memory_GetRandomIndex(m_options.m_option_patternDifficulty.value,
            m_list_displayOrder.Last());
        m_list_displayOrder.Add(newIndex);

        //then go in to playback mode with our new target added
        StartCoroutine(PlaybackSequence());
    }

    private IEnumerator PlaybackSequence()
    {
        m_mode = MemoryMode.Playback;
        var playbackIndex = 0;
        while (playbackIndex < m_list_displayOrder.Count)
        {
            if (m_haltPlayback) break;
            //activate the targets in the order of the list
            Manager_Targets.Instance.Memory_ActivateTargetByIndex(m_list_displayOrder[playbackIndex]);
            yield return new WaitForSeconds(m_options.m_option_displayTime.value);

            playbackIndex++;
        }

        //we're finished with the display mode, so switch the mode to input
        Manager_Targets.Instance.ClearTargets(TestType.Sequence);
        m_currentInputCount = 0;
        m_mode = MemoryMode.Input;
    }

    protected override void TargetHit(Target t)
    {
        //doesn't matter if we hit a target during playback..
        if (m_mode == MemoryMode.Playback || m_mode == MemoryMode.ShowingSolution) return;

        //whenever a target is hit we need to see if it matches the target in the display list
        if (t.m_index != m_list_displayOrder[m_currentInputCount])
        {
            IncorrectTargetHit();
            return;
        }

        //manually handle auto here since default target handling only works if the target is already active
        if (m_options.m_option_audioOnhit.value)
            Manager_Audio.PlaySound(Manager_Test.Instance.m_selectedTest.m_options.m_option_audioClip.value);

        //otherwise, this was correct, so increment the score
        m_score++;

        //and increment the input count
        m_currentInputCount++;


        //if there are still more targets to hit in this sequence just return
        if (m_currentInputCount < m_list_displayOrder.Count) return;

        //otherwise, we've reached the end of this sequence, so we can add a new target and record this level time
        AddTargetToSequence();
        RecordLevelTime();
        //and increase the level
        m_currentLevel++;
    }

    private void IncorrectTargetHit()
    {
        //even though an incorrect target was hit we want to record the level time here so it doesn't just keeping
        //running while the solution is being shown
        RecordLevelTime();
        m_mode = MemoryMode.ShowingSolution;
        StartCoroutine(ShowSolution());
    }

    private IEnumerator ShowSolution()
    {
        var labels = new Dictionary<int, TextMeshProUGUI>();
        var showingIndex = 0;

        while (showingIndex < m_list_displayOrder.Count)
        {
            if (m_haltPlayback) break;
            //first, see if we already have a label for this index, meaning this one was in the solution more than once
            if (labels.ContainsKey(m_list_displayOrder[showingIndex]))
                //we do, so just append to the new sequence number on this label
            {
                labels[m_list_displayOrder[showingIndex]].text += ", " + (showingIndex + 1);
            }
            else
            {
                //otherwise, make a new label
                var label = Instantiate(pfb_solutionLabel, Manager_Targets.Instance.m_trans_targets_memory);
                Vector2 pos = Manager_Targets.Instance.m_targets_memory[m_list_displayOrder[showingIndex]]
                    .transform.localPosition;
                pos.y += 30;
                label.transform.localPosition = pos;
                label.text = (showingIndex + 1).ToString();
                labels.Add(m_list_displayOrder[showingIndex], label);
            }

            yield return new WaitForSeconds(ConstantDefinitions.Instance.memory_showSolutionDelay);
            showingIndex++;
        }

        //then do one final delay
        //only do this if we didn't halt
        if (!m_haltPlayback) yield return new WaitForSeconds(ConstantDefinitions.Instance.memory_showSolutionDelay * 5);
        labels.ForEach(p => Destroy(p.Value.gameObject));
        labels.Clear();

        if (!m_haltPlayback)
            Manager_Test.Instance.StopCurrentTest();
    }

    public override void Update()
    {
        m_currentTime += Time.deltaTime;
    }

    private void RecordLevelTime()
    {
        m_list_levelTimes.Add(m_currentTime);
        m_currentTime = 0;
    }

    public override string GetInfoString()
    {
        return "Score: " + m_score + "\nMode: " + m_mode;
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        m_haltPlayback = true; //this will stop the sequence or solution from being shown if the test is stopped

        if (m_score <= 0 || stoppedWithButton) return;

        //determine the highest possible score
        var highestPossible = 0;
        for (var x = m_options.m_option_startingLevel.value; x <= m_currentLevel; x++)
            highestPossible += x;
        //now the number of hits that the highest level is the = highest level -(highest possible-score)
        var hitsAtHighestLevel = m_currentLevel - (highestPossible - m_score);

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_sequence_difficulty = m_options.m_option_patternDifficulty.value;
        e.m_trainingSummary.m_sequence_displayTime = m_options.m_option_displayTime.value;
        e.m_trainingSummary.m_highestlevel = m_currentLevel;
        e.m_trainingSummary.m_hitsOnHighestLevel = hitsAtHighestLevel;
        e.m_trainingSummary.m_list_leveltimes = m_list_levelTimes;
        e.m_trainingSummary.m_startingLevel = m_options.m_option_startingLevel.value;
        e.m_trainingSummary.m_levelsCompleted = m_list_levelTimes.Count;

        FillTestDataToSummary(e);
    }
}