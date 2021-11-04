using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Multi : Test
{
    public static Test_Multi Instance;

    /// <summary>
    ///     the list that holds the arrows to show the targets we need to follow
    /// </summary>
    private List<IndicatorArrow> arrows;

    [InfoBox("How high off the sprite the indicator arrow appears to show which target to hit")]
    public float m_arrowYposOffset;

    /// <summary>
    ///     this lets us keep track if we've hit the right or wrong target for each set
    /// </summary>
    private bool[] m_bool_hitResults;

    /// <summary>
    ///     this lets us track which targets have been hit or the available sets, when they are true then we've hit all of
    ///     our targets for the test and it can be scored
    /// </summary>
    private bool[] m_bool_targetsHit;

    /// <summary>
    ///     this is the current angle of each target, so we can keep track and resume from any given rotation
    /// </summary>
    private float[] m_currentTargetBearing;

    private int m_currentTrial;

    /// <summary>
    ///     this is our array of arrays that will track our hits of each target set for each trial..
    /// </summary>
    private bool[][] m_fullTestResults;

    private MultiMode m_mode;

    private float m_targetMagnitude;
    public IndicatorArrow pfb_indicatorArrow;

    public override void Initialize()
    {
        base.Initialize();
        m_testName = "Multi";
        m_type = TestType.Multi;
        Instance = this;
        m_trackHitTimes = false;
        m_testIsTimed = false;
        m_mode = MultiMode.Showing;

        //defaults
        m_options.m_option_targetSets.SetDefault(2);
        m_options.m_option_rotationSpeed.SetDefault(5);
        m_options.m_option_rotationTime.SetDefault(3);
        m_options.m_option_trialCount.SetDefault(3);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_gridSize.SetDefault(128);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_countdowntime.SetDefault(3);

        //hook our target events
        Manager_Targets.Instance.m_targets_multi.ForEach(t => t.event_targetDown += TargetHit);
    }

    public override void Update()
    {
        base.Update();
        if (!m_isRunning) return;
        if (m_mode != MultiMode.Spinning) return;
    }

    public override void StartTest()
    {
        base.StartTest();
        //these are the starting positions of each target
        m_currentTargetBearing = new[] {1.57f, 4.71f, 1.57f, 4.71f, 1.57f, 4.71f, 1.57f, 4.71f};

        //we have to use jagged arrays here because we can't serialized 2d arrays..
        m_fullTestResults = new bool[m_options.m_option_trialCount.value][];
        for (var x = 0; x < m_options.m_option_trialCount.value; x++)
            m_fullTestResults[x] = new bool[m_options.m_option_targetSets.value];

        m_currentTrial = 1;
        StartNextTrial();
    }

    private void StartNextTrial()
    {
        if (m_currentTrial > m_options.m_option_trialCount.value)
        {
            Manager_Test.Instance.StopCurrentTest();
            return;
        }

        //we'll always indicate target [0] from each set
        m_bool_targetsHit = new[] {false, false, false, false};
        m_bool_hitResults = new bool[4];

        //calculate our magnitude
        m_targetMagnitude = Vector2.Distance(Vector2.zero,
            Manager_Targets.Instance.m_targets_multi[0].rectTransform.anchoredPosition);

        //the parameter we pass here is if we want to do the cooldown, which we only want to do
        //between trials.  This should only be false on the first trial..
        StartCoroutine(ShowIndicatedTargets(m_currentTrial != 1));
    }

    protected override void TargetHit(Target t)
    {
        if (m_mode != MultiMode.Waiting) return;
        //when we hit a target in wait mode we need to see if it was an indicated target
        //depending on the sets used and which target we indicated check if this is the right target
        //target [0] in each set is the correct target, so 0/2/4 are correct and 1/3/5 are wrong depending on
        //how many target sets this test is using
        switch (t.m_index)
        {
            //set 1
            case 0:
                m_bool_hitResults[0] = true;
                m_bool_targetsHit[0] = true;
                break;
            case 1:
                m_bool_hitResults[0] = false;
                m_bool_targetsHit[0] = true;
                break;
            case 2:
                m_bool_hitResults[1] = true;
                m_bool_targetsHit[1] = true;
                break;
            case 3:
                m_bool_hitResults[1] = false;
                m_bool_targetsHit[1] = true;
                break;
            case 4:
                m_bool_hitResults[2] = true;
                m_bool_targetsHit[2] = true;
                break;
            case 5:
                m_bool_hitResults[2] = false;
                m_bool_targetsHit[2] = true;
                break;
            case 6:
                m_bool_hitResults[3] = true;
                m_bool_targetsHit[3] = true;
                break;
            case 7:
                m_bool_hitResults[3] = false;
                m_bool_targetsHit[3] = true;
                break;
        }

        //manually handle the audio on hit
        if (m_options.m_option_audioOnhit.value)
            Manager_Audio.PlaySound(Manager_Test.Instance.m_selectedTest.m_options.m_option_audioClip.value);

        //once we hit a target in a set, we want to reveal that set
        var allTargetsHit = true;
        for (var x = 0; x < m_options.m_option_targetSets.value; x++)
            if (m_bool_targetsHit[x])
                Manager_Targets.Instance.Multi_ActivateTargetsFromSet(x);
            else allTargetsHit = false;

        if (!allTargetsHit) return;

        //once we've hit all targets, we want to record the result of this trial
        for (var targetSet = 0; targetSet < m_options.m_option_targetSets.value; targetSet++)
            m_fullTestResults[m_currentTrial - 1][targetSet] = m_bool_hitResults[targetSet];


        //we're finished with this trial
        Debug.Log("Multi trial " + m_currentTrial + " finished");
        m_currentTrial++;
        StartNextTrial();
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //in case this test is stopped prematurely..
        StopAllCoroutines();
        DestroyArrows();
        Manager_Targets.Instance.Multi_ResetTargetPositions(Manager_Test.Instance.m_selectedTest.m_options
            .m_option_gridSize.value);

        //do NOT score multi if the test is stopped with the button, the arrays used below are populated as the test
        //progresses so we can have a size mismatch or wacky division by zero shit if this happens... alternatively
        //we could set those arrays before the test starts.......
        if (stoppedWithButton) return;

        #region Obsolete - old scoring method

        //get our short score
        /*
        m_score = 0;
        for(int x = 0;x<m_numberOfTrials;x++)
            for(int y = 0; y<m_targetSets;y++)
                if (m_fullTestResults[x][y])
                    m_score++;

        float s = m_score / (float) (m_numberOfTrials * m_last_targetSets);
        s *= 100;
        */

        #endregion

        //get the total number correct
        m_score = 0;
        for (var x = 0; x < m_options.m_option_trialCount.value; x++)
        for (var y = 0; y < m_options.m_option_targetSets.value; y++)
            if (m_fullTestResults[x][y])
                m_score++;

        var possibleCorrect = m_options.m_option_trialCount.value * m_options.m_option_targetSets.value;
        var multiplier = GetScoreMultiplier();
        var score = m_score / (float) possibleCorrect * multiplier;

        //the only way the score would be zero here is if you suck and missed every target... which I suppose is possible...

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);
        e.m_trainingSummary = new PatientTrainingSummary
        {
            m_shortScore = score.ToString("0.0"),
            m_multi_score = score,
            m_multi_trials = m_options.m_option_trialCount.value,
            m_trialResults = m_fullTestResults,
            m_multi_targetSets = m_options.m_option_targetSets.value,
            m_multi_rotationSpeed = m_options.m_option_rotationSpeed.value.ToString(),
            m_multi_rotationTime = m_options.m_option_rotationTime.value
        };

        FillTestDataToSummary(e);
    }

    public override string GetInfoString()
    {
        return "Mode: " + m_mode;
    }

    private IEnumerator ShowIndicatedTargets(bool doCooldown)
    {
        if (doCooldown)
        {
            m_mode = MultiMode.Cooldown;
            yield return new WaitForSeconds(ConstantDefinitions.Instance.multi_cooldownTime);
        }

        m_mode = MultiMode.Showing;

        Manager_Targets.Instance.Multi_VisuallyResetAllTargets();

        //make the indicator arrows appear above our indicated targets
        arrows = new List<IndicatorArrow>();
        var index = 0;
        for (var x = 0; x < m_options.m_option_targetSets.value; x++)
        {
            var arrow =
                Instantiate(pfb_indicatorArrow, Manager_Targets.Instance.m_targets_multi[index].transform);
            arrow.m_rectTransform.anchoredPosition = new Vector2(0, m_arrowYposOffset);
            arrows.Add(arrow);
            index += 2;
        }

        yield return new WaitForSeconds(ConstantDefinitions.Instance.multi_showtime);

        DestroyArrows();
        StartCoroutine(SpinTargets());
    }

    private void DestroyArrows()
    {
        if (arrows != null)
        {
            for (var x = arrows.Count - 1; x >= 0; x--)
                Destroy(arrows[x].gameObject);

            arrows.Clear();
        }
    }

    private IEnumerator SpinTargets()
    {
        //note spin speed is theoretically limited by the refresh rate of the monitor.  If we're spinning faster than
        //the refresh rate can keep up with there is no way a user can track the target since it will just be a blur..
        float currentTime = 0;
        m_mode = MultiMode.Spinning;

        //determine which direction to go
        var direction = new bool[8];
        direction[0] = direction[1] = Random.Range(0, 2) == 0;
        direction[2] = direction[3] = Random.Range(0, 2) == 0;
        direction[4] = direction[5] = Random.Range(0, 2) == 0;
        direction[6] = direction[7] = Random.Range(0, 2) == 0;

        //determine the speed
        //todo: there should be some logic here, but for now, it can just be random..
        var rotateSpeed = m_options.m_option_rotationSpeed.value;
        var speed = new float[8];
        speed[0] = speed[1] =
            Random.Range(rotateSpeed - rotateSpeed * .5f, rotateSpeed + rotateSpeed * .5f);
        speed[2] = speed[3] =
            Random.Range(rotateSpeed - rotateSpeed * .4f, rotateSpeed + rotateSpeed * .4f);
        speed[4] = speed[5] =
            Random.Range(rotateSpeed - rotateSpeed * .3f, rotateSpeed + rotateSpeed * .3f);
        speed[6] = speed[7] =
            Random.Range(rotateSpeed - rotateSpeed * .2f, rotateSpeed + rotateSpeed * .2f);


        //float[] angle = {1.57f, 4.71f, 1.57f, 4.71f, 1.57f, 4.71f};

        while (currentTime < m_options.m_option_rotationTime.value)
        {
            for (var x = 0; x < m_options.m_option_targetSets.value * 2; x++)
            {
                //spin the targets
                var inc = Time.deltaTime * speed[x];

                if (direction[x])
                    m_currentTargetBearing[x] += inc;
                else
                    m_currentTargetBearing[x] -= inc;

                var newPos = new Vector2
                {
                    x = Mathf.Cos(m_currentTargetBearing[x]) * m_targetMagnitude,
                    y = Mathf.Sin(m_currentTargetBearing[x]) * m_targetMagnitude
                };
                Manager_Targets.Instance.m_targets_multi[x].rectTransform.anchoredPosition = newPos;
            }

            currentTime += Time.deltaTime;
            yield return null;
        }

        m_mode = MultiMode.Waiting;
    }

    private float GetScoreMultiplier()
    {
        float multiplier = 0;

        //trials
        switch (m_options.m_option_trialCount.value)
        {
            case 3:
                multiplier += 2;
                break;
            case 5:
                multiplier += 4.5f;
                break;
            case 7:
                multiplier += 7;
                break;
            default:
                Debug.Log("Problem calculating multiplier with trials..");
                break;
        }

        //target sets
        switch (m_options.m_option_targetSets.value)
        {
            case 1:
                multiplier += 6;
                break;
            case 2:
                multiplier += 13.5f;
                break;
            case 3:
                multiplier += 21.3f;
                break;
            case 4:
                multiplier += 30;
                break;
            default:
                Debug.Log("Problem calculating multiplier with target sets..");
                break;
        }

        //rotation speed
        if (m_options.m_option_rotationSpeed.value == 3)
            multiplier += 3;
        else if (m_options.m_option_rotationSpeed.value == 5)
            multiplier += 6.75f;
        else if (m_options.m_option_rotationSpeed.value == 7)
            multiplier += 10.5f;
        else if (m_options.m_option_rotationSpeed.value == 10)
            multiplier += 15;
        else Debug.Log("Problem calculating multiplier with target sets..");

        //rotation time
        if (m_options.m_option_rotationTime.value == 3)
            multiplier += 9;
        else if (m_options.m_option_rotationTime.value == 5)
            multiplier += 20.25f;
        else if (m_options.m_option_rotationTime.value == 7)
            multiplier += 31.5f;
        else if (m_options.m_option_rotationTime.value == 10)
            multiplier += 45;
        else Debug.Log("Problem calculating multiplier with target sets..");


        Debug.Log("Multi score multiplier: " + multiplier);
        return multiplier;
    }
}