using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : SerializedMonoBehaviour
{
    public delegate void TestEvent();

    /// <summary>
    ///     Event will be fired whenever a test is selected
    /// </summary>
    [HideInEditorMode] public TestEvent event_selected;

    /// <summary>
    ///     This should be set only by Target() and only if it was active and hit this frame
    /// </summary>
    [HideInEditorMode] public bool m_activeTargetHitThisFrame;

    /// <summary>
    ///     flag to check if the balance plate cursor is currently over the active target or not
    /// </summary>
    public bool m_bpCursorOverTarget;

    /// <summary>
    ///     the running highest streak..
    /// </summary>
    [HideInEditorMode] private int m_currentHighStreak;

    /// <summary>
    ///     If we recorded data, this is where it is stored
    /// </summary>
    private string m_dataLocation;

    /// <summary>
    ///     stream writer for writing bp data
    /// </summary>
    private StreamWriter m_dataWriter;


    public string m_defaultBackground;
    public string m_defaultTarget;

    /// <summary>
    ///     how long have we been hovering over a target?
    /// </summary>
    [HideInEditorMode] public float m_hoverTimeOverTarget;

    /// <summary>
    ///     the time since the last target was hit, when to reset this should be up to each deriving test
    /// </summary>
    [HideInEditorMode] public List<float> m_list_hitTimes;

    /// <summary>
    ///     generic holder for tracking misses during a test
    /// </summary>
    [HideInEditorMode] public int m_misses;

    [HideInInspector] public TestOptions_Base m_options;

    /// <summary>
    ///     A flag to check if a target has been pre=spawned in this test, this allows us not to double up on setup
    ///     junk when StartTest is actually called
    /// </summary>
    protected bool m_preSpawningPerformed;

    /// <summary>
    ///     A generic holder for any type of int scoring
    /// </summary>
    [HideInEditorMode] public int m_score;

    /// <summary>
    ///     generic holder for tracking streak, handled in base.lateUpdate()
    /// </summary>
    [HideInEditorMode] public int m_streak;

    /// <summary>
    ///     allows us to track time between hits
    /// </summary>
    [HideInEditorMode] public float m_targetHitTime;

    /// <summary>
    ///     how long the current target has been shown for
    /// </summary>
    [HideInEditorMode] public float m_targetUpTime;

    /// <summary>
    ///     is this timed to that we should track elapsed time and end the test when we hit a duration
    /// </summary>
    [HideInEditorMode] public bool m_testIsTimed;

    [HideInEditorMode] public string m_testName;

    /// <summary>
    ///     anytime there is a mouseup(0) this is set during update and then checked during lateUpdate
    /// </summary>
    private bool m_touchThisFrame;

    /// <summary>
    ///     The times mousedown(0) occurred, according to m_elapsedTime
    /// </summary>
    [HideInEditorMode] public List<float> m_touchTimes;

    /// <summary>
    ///     Do we want the base class to track timing between hits?  This should be off for memory and reaction..
    /// </summary>
    [HideInEditorMode] public bool m_trackHitTimes;

    [HideInEditorMode] public TestType m_type;

    /// <summary>
    ///     How much time has elapsed since we started this test
    /// </summary>
    protected float m_timeElapsed { get; private set; }

    /// <summary>
    ///     this this test running?
    /// </summary>
    [HideInEditorMode]
    public bool m_isRunning { get; private set; }


    // Use this for initialization
    public virtual void Initialize()
    {
        m_options = new TestOptions_Base();
        m_options.m_option_background.SetDefault(m_defaultBackground);
        m_options.m_option_backgroundIsVideo.SetDefault(false);
        m_options.m_option_target.SetDefault(m_defaultTarget);
    }

    /// <summary>
    ///     Update is called once per frame, base updates the duration and ends the test when the time is up
    ///     no need to call this for tests that aren't timed
    /// </summary>
    public virtual void Update()
    {
        if (!m_isRunning) return;

        m_timeElapsed += Time.deltaTime;


        if (m_testIsTimed && m_timeElapsed >= m_options.m_option_duration.value)
        {
            Manager_Test.Instance.StopCurrentTest();
            return;
        }


        if (m_options.m_option_inputMode.value != InputMode.Balance && Input.GetMouseButtonDown(0))
        {
            //add this mouse down to our touch times
            m_touchTimes.Add(m_timeElapsed);

            //we also have to handle a manual raycast if we're in ehb mode since the bp cursor collider will
            //occulde the default mouse click
            if (Manager_Test.Instance.m_testInputMode == InputMode.Ehb)
            {
                var hits =
                    Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hits.Length > 0)
                    foreach (var h in hits)
                    {
                        if (!h.collider.gameObject.name.Equals("Image_SensitivityRing") || !m_bpCursorOverTarget)
                            continue;
                        //this is a valid ehb hit, so manually invoke the target hit event of the target we just hit
                        var t = h.transform.parent.GetComponent<Target>();
                        t.TargetDown(null);
                        break;
                    }
            }
        }

        if (m_trackHitTimes)
        {
            m_touchThisFrame = Input.GetMouseButtonDown(0);
            m_targetHitTime += Time.deltaTime;
        }

        HandleLateUpdate();
    }

    /// <summary>
    ///     Late update is needed to keep track of our hit times.  This allows us to tell if the user touched the screen this
    ///     frame.  This is useful for knowing when a target is touched, and can be used as a data metric
    /// </summary>
    private void HandleLateUpdate()
    {
        if (!m_isRunning) return;

        //handle the bp cursor if needed
        if (Manager_Test.Instance.m_testInputMode == InputMode.Balance ||
            Manager_Test.Instance.m_testInputMode == InputMode.Ehb)
        {
            //if a test is using the cursor but has hover time set to 0 just ignore this, otherwise the progress ring
            //appears always full..
            if (m_options.m_opton_hoverTimeGoal.value.Equals(0)) return;
            if (!m_bpCursorOverTarget) //decay the progress ring
            {
                if (m_hoverTimeOverTarget > 0)
                    m_hoverTimeOverTarget -= Time.deltaTime * m_options.m_option_hoverTimeDecaySpeed.value;

                BalancePlateCursor.Instance.SetProgressRing(
                    m_hoverTimeOverTarget / m_options.m_opton_hoverTimeGoal.value);
            }
        }

        if (!m_trackHitTimes) return;

        if (m_touchThisFrame && !m_activeTargetHitThisFrame)
            MissedTarget();

        else if (m_touchThisFrame && m_activeTargetHitThisFrame) m_currentHighStreak++;

        if (m_currentHighStreak > m_streak)
            m_streak = m_currentHighStreak;

        //either way, reset the flag
        m_activeTargetHitThisFrame = false;
    }


    /// <summary>
    ///     Each test should handle this individually
    /// </summary>
    protected virtual void TargetHit(Target t)
    {
    }

    /// <summary>
    ///     this is a special mode of the target hit method intended for BP hover test only.
    ///     Since it doesn't use an actual target we can call this instead (see usage in Target_Trigger_Hover)
    /// </summary>
    protected virtual void TargetHit()
    {
    }

    /// <summary>
    ///     this is called when there is a touch this frame, but the active target wasn't hit this frame
    ///     we can override this if we want to do anything special when a miss occurs..
    ///     base updates streak and miss count
    /// </summary>
    protected virtual void MissedTarget()
    {
        //touches and therefore misses can be completely ignored in pure balance mode with self pacing only
        if (m_options.m_option_inputMode.value == InputMode.Balance &&
            m_options.m_option_pacing.value == TestOption_Pacing.Self) return;

        m_currentHighStreak = 0;
        //if this occurs we've broken our streak of targets hit
        Debug.Log("Streak broken");
        //this also counts as a miss
        m_misses++;
    }

    /// <summary>
    ///     Returns the string to be shown in the test info display
    /// </summary>
    public virtual string GetInfoString()
    {
        return "Info String";
    }

    public virtual void HandleTargetScaling()
    {
    }

    /// <summary>
    ///     called when this test is to be started, base sets running = true and resets the elapsed time and score
    /// </summary>
    public virtual void StartTest()
    {
        m_isRunning = true;
        m_timeElapsed = 0;
        m_targetHitTime = 0;
        m_streak = 0;
        m_misses = 0;
        m_currentHighStreak = 0;
        //if either of these are carried over from a previous test and is still true, it can make a false miss appear,
        //so they need to be reset here
        m_touchThisFrame = false;
        m_activeTargetHitThisFrame = false;
        m_list_hitTimes = new List<float>();
        m_touchTimes = new List<float>();
        m_dataLocation = "";
        //todo resetting these here is fine for data purposes, but it will show the previous score and elapsed time
        //when a new test is starting, not a problem necessarily.. but probably want to fix at some point
        m_timeElapsed = 0;
        m_score = 0;
        m_hoverTimeOverTarget = 0;
        m_bpCursorOverTarget = false;
        if (Manager_Test.Instance.m_recordBpData) StartDataRecord();
    }

    /// <summary>
    ///     This is for tests in balance modes that need to spawn their initial target while the countdown is going on, but
    ///     before
    ///     StartTest() is called.
    /// </summary>
    public virtual void PreSpawnTarget()
    {
    }

    /// <summary>
    ///     !!! STOP !!!This should only ever be directly called by the test manager in StopCurrentTest()!!!
    ///     Be sure to manually invoke base.StopTest() in any test that overrides this... which should be all of them
    ///     the stoppedWithButton is used to determine if the test was stopped by just ending or if it was forcibly stopped
    ///     using the button.  This allows us to enable an option to not score the test if it was stopped prematurely
    ///     and stops any problems occuring with scoring incomplete tests, namely multi..
    ///     with all of that in mind this method should be used to do anything we need to do when this test is stopped
    ///     the base call will set running=false.
    /// </summary>
    public virtual void StopTest(bool stoppedWithButton)
    {
        m_isRunning = false;
        StopDataRecord();
        //we can reset the decay of the bp cursor here for now, in case it was mid-progress when a test was stopped
        //eventually we'll want to use test start/stop events
        BalancePlateCursor.Instance.SetProgressRing(0);

        m_preSpawningPerformed = false;
    }

    /// <summary>
    ///     Here we add the test summary with all of the info that test reports need.  The default call will
    ///     add many things... name, date, type, global settings, and general test settings.  The parameter here can
    ///     be null if we don't need to do anything specific in the overridding test, otherwise it should be instantiated
    ///     there, do anything special we need to, and then passed here.
    ///     Not every test uses everything that is recorded here, but it can just be ignored when making the report
    /// </summary>
    protected void FillTestDataToSummary(Entry_Scoreboard e)
    {
        //we don't need to do anything if there is no selected patient
        if (Manager_Patient.Instance.m_selectedPatient == null)
        {
            if (e != null) Destroy(e.gameObject);
            return;
        }

        //prepare the scoreboard entry for the graphs and such
        if (e == null)
            e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
                Menu_Reports.Instance.m_trans_contentParent);
        e.m_trainingSummary.m_testName = m_testName;
        e.m_trainingSummary.m_testType = m_type;
        e.m_trainingSummary.m_operatorAccountName = Manager_Users.Instance.m_currentUser.m_accountName;
        e.m_trainingSummary.m_dateTime = DateTime.Now;
        e.m_trainingSummary.m_background = Options_BackgroundSelect.Instance.m_selectedBackgroundName;
        e.m_trainingSummary.m_target = Manager_Targets.Instance.currentTargetGraphic.name;
        e.m_trainingSummary.m_sensitivity = m_options.m_option_touchSensitivity.value.ToString();
        e.m_trainingSummary.m_touchSensitivity_keepVisible = m_options.m_option_touchSensitivity_keepVisible.value;
        e.m_trainingSummary.m_gridSize = m_options.m_option_gridSize.value.ToString();
        e.m_trainingSummary.m_targetSize = m_options.m_option_targetSize.value.ToString();
        e.m_trainingSummary.m_audio = m_options.m_option_audioOnhit.value
            ? m_options.m_option_audioClip.value.name
            : "Off";
        e.m_trainingSummary.m_testDuration = m_options.m_option_duration.value;

        e.m_trainingSummary.m_paceMode = m_options.m_option_pacing.value.ToString();
        e.m_trainingSummary.m_autopaceInterval = m_options.m_option_autoPaceInterval.value;
        e.m_trainingSummary.m_countdown = m_options.m_option_countdowntime.value.ToString();
        //doing this check allows us to see if the short score is empty or not,
        //the only time it won't be empty is when an overriding test already set this and thus, we won't 
        //mess with it here
        if (string.IsNullOrEmpty(e.m_trainingSummary.m_shortScore))
            e.m_trainingSummary.m_shortScore = m_score.ToString();
        e.m_trainingSummary.m_totalHits = m_score;
        e.m_trainingSummary.m_totalMisses = m_misses;
        e.m_trainingSummary.m_streak = m_streak;

        e.m_trainingSummary.m_inputMode = m_options.m_option_inputMode.isImplemented
            ? m_options.m_option_inputMode.namedValue
            : "Touch";

        e.m_trainingSummary.m_decayTime = m_options.m_option_hoverTimeDecaySpeed.value;
        e.m_trainingSummary.m_balance_hoverTime = m_options.m_opton_hoverTimeGoal.value;

        //not every test uses this, and even when they do, if nothing we recorded, this can throw errors..
        e.m_trainingSummary.m_hitTimes = m_list_hitTimes;
        if (m_list_hitTimes != null && m_list_hitTimes.Count > 0)
        {
            e.m_trainingSummary.m_fastestHit = m_list_hitTimes.Min();
            e.m_trainingSummary.m_slowestHit = m_list_hitTimes.Max();
            e.m_trainingSummary.m_averageHit = m_list_hitTimes.Average();
        }

        e.m_trainingSummary.m_dataLocation = m_dataLocation;
        
        //check if this is a test that needs to have score fatigue calculated
        if (m_list_hitTimes != null && m_list_hitTimes.Count > 0)
        {
            var halfway = m_options.m_option_duration.value / 2f;
            var elapsed = 0f;
            var firstHalf = 0f;
            var secondHalf = 0f;
            foreach (var t in m_list_hitTimes)
            {
                elapsed += t;
                if (t + elapsed <= halfway) firstHalf++;
                else secondHalf++;
            }

            e.m_trainingSummary.m_score_firstHalf = firstHalf.ToString();
            e.m_trainingSummary.m_score_secondHalf = secondHalf.ToString();
            e.m_trainingSummary.m_scoreFatigue = (firstHalf / secondHalf).ToString();
        }

        Menu_Reports.Instance.AddEntry(e);
        Manager_Patient.Instance.m_selectedPatient.AddTrainingSummary(e.m_trainingSummary);
    }

    /// <summary>
    ///     Anything we need to do when this test is selected in the main menu, base will show the target set for this type
    ///     of test
    ///     FYI each test needs to set their test specific options according to the last selected values, the global settings
    ///     are handled here in base but every test has different specific setting so they need to be handled in each Selected
    ///     override
    /// </summary>
    public virtual void Selected()
    {
        Manager_Targets.Instance.ShowTargetSet(m_type);
        Options_TargetSelect.Instance.ShowNormalTargets();
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds();
        Options_BackgroundSelect.Instance.SetBackgroundByName(m_options.m_option_background.value,
            m_options.m_option_backgroundIsVideo.value, true);
        Options_TargetSelect.Instance.Show(true); //show this in case it has been hidden by the balance test
        Options_TargetSelect.Instance.SetTargetByName(m_options.m_option_target.value);
        //this we'll change here since not ever test uses it, and we could get in to a wacky state if this was flipped
        Manager_Test.Instance.ChangeInputMode(InputMode.Touch);
        //SetLastGlobalOptions();

        event_selected();
    }

    /// <summary>
    ///     Start the process of recording force plate data to our patient.  We need to have an active patient and
    ///     a BP, obvs...
    /// </summary>
    private void StartDataRecord()
    {
        //make sure we have a balance plate..
        if (BalancePlate.Instance == null || BalancePlate.Instance.IsNull()) return;

        //make a file in the current patient directory, or a temp place if no patient is selected
        if (Manager_Patient.Instance.m_selectedPatient != null)
            //this matches the naming convention of the main patient info file..
        {
            m_dataLocation = Manager_Patient.Instance.GetSelectedPatientDirectory() + "/" + m_type + "_DATA_";
        }
        else
        {
            //without a patient selected we can record data to a generic location
            if (!Directory.Exists("/Temp_BPData"))
                Directory.CreateDirectory("/Temp_BPData");

            m_dataLocation = Application.persistentDataPath + "/Temp_BPData/" + m_type + "_DATA_";
            return;
        }

        BalancePlate.Instance.StartRecordingData(m_dataLocation);
    }

    /// <summary>
    ///     Stop writing BP data, this MUST be called whenever a test is stopped for ANY reason otherwise it will keep
    ///     the streamwriter open and bad things will happen
    /// </summary>
    protected void StopDataRecord()
    {
        if (BalancePlate.Instance == null) return; //how did you get here?

        BalancePlate.Instance.StopRecordingData();
    }

    /// <summary>
    ///     Handle anything we want to when in balance mode only for the bp cursor interacting with targets
    ///     These should only fire on active targets (handled in Target_BPCursorHandler), and tests that cannot
    ///     enter balance input mode can just ignore these.  However, each test using these will need to hook their
    ///     targets and events individually
    /// </summary>
    public virtual void Target_Trigger_Enter(Target t)
    {
        if (!m_isRunning) return;
        if (t != null && !t.m_isActive) return;

        if (Manager_Test.Instance.m_testInputMode == InputMode.Touch) return;

        m_bpCursorOverTarget = true;
    }

    /// <summary>
    ///     See comment for Target_Enter()
    /// </summary>
    public virtual void Target_Trigger_Hover(Target t)
    {
        if (!m_isRunning) return;
        if (t != null && !t.m_isActive) return;

        var inputMode = Manager_Test.Instance.m_testInputMode;
        if (inputMode == InputMode.Touch) return;

        //only mess with hover time in balance mode, ehb ignore hover time!
        if (inputMode == InputMode.Balance)
        {
            m_hoverTimeOverTarget += Time.deltaTime;
            if (m_options.m_opton_hoverTimeGoal.value != 0) //no dividing by zero..
                BalancePlateCursor.Instance.SetProgressRing(
                    m_hoverTimeOverTarget / m_options.m_opton_hoverTimeGoal.value);

            if (!(m_hoverTimeOverTarget / m_options.m_opton_hoverTimeGoal.value >= 1)) return;
        }

        //this should count as a target hit  if we're in balance mode only, ebh mode is handled below
        //this can happen if we're calling this for the BP hover target which isn't an actual target object, but
        //we still want to use these progress rings functions
        if (inputMode == InputMode.Balance)
        {
            if (t == null)
                TargetHit();
            else TargetHit(t);
        }
        else //ebh mode
        {
            //this is a valid state, but in ebh mode a hit can only be triggered by a click..
            return;
        }


        //reset the progress ring
        m_hoverTimeOverTarget = 0;
        BalancePlateCursor.Instance.SetProgressRing(0);

        //then manually invoke the exit since the cursor won't physically exit, and the Exit() method would never
        //get called on it's own
        Target_Trigger_Exit(t);
    }

    /// <summary>
    ///     See comment for Target_Enter()
    /// </summary>
    public virtual void Target_Trigger_Exit(Target t)
    {
        if (!m_isRunning) return;
        if (t != null && !t.m_isActive) return;

        if (Manager_Test.Instance.m_testInputMode == InputMode.Touch) return;

        //decay is handled in Update using this flag
        m_bpCursorOverTarget = false;
    }

    #region Special parameter-less hover events for BP target

    public void Target_Trigger_Hover()
    {
        Target_Trigger_Hover(null);
    }

    public void Target_Trigger_Exit()
    {
        Target_Trigger_Exit(null);
    }

    public void Target_Trigger_Enter()
    {
        Target_Trigger_Enter(null);
    }

    #endregion
}