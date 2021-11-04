using System;
using System.Collections.Generic;
using SWS;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Test_Bp_TargetHover : Test
{
    public static Test_Bp_TargetHover Instance;
    private PathManager m_activePath;
    private int m_dynamicTargetExited;
    public List<PathManager> m_paths;
    private splineMove m_splineMover;

    private float m_totalTimeOverTarget;
    
    //testing
    public Toggle m_toggle_bgTracking;
    public RawImage m_rawImage;
    private bool m_bgTracking;

    public override void Initialize()
    {
        base.Initialize();
        Instance = this;
        m_type = TestType.Balance;
        m_testName = "Balance";
        m_trackHitTimes = true;
        m_testIsTimed = true;

        m_options.m_option_duration.SetDefault(60);
        m_options.m_option_balance_targetSize.SetDefault(300);
        m_options.m_opton_hoverTimeGoal.SetDefault(.25f);
        m_options.m_option_hoverTimeDecaySpeed.SetDefault(0);
        m_options.m_option_targetTrackingDirection.SetDefault(TargetTrackingDirection.Random);
        m_options.m_option_targetTrackingMode.SetDefault(TargetTrackingMode.Static);
        m_options.m_option_targetTrackingSpeed.SetDefault(2f);
        m_options.m_option_countdowntime.SetDefault(3);

        m_splineMover = Manager_Targets.Instance.m_bpHoverTarget.GetComponent<splineMove>();


        BalancePlateCursor.Instance.event_triggerEnter += Target_Trigger_Enter;
        BalancePlateCursor.Instance.event_triggerStay += Target_Trigger_Hover;
        BalancePlateCursor.Instance.event_triggerExit += Target_Trigger_Exit;
        
        //testing
        m_bgTracking = false;
        m_toggle_bgTracking.onValueChanged.AddListener(BgTogglePushed);
    }

    public override void Update()
    {
        base.Update();

        //testing
        //if (Input.GetKeyDown(KeyCode.P)) MoveTarget();
        if (m_bgTracking)
        {
            m_rawImage.rectTransform.anchoredPosition =
                BalancePlateCursor.Instance.m_rectTransform_base.anchoredPosition;
        }

        if (!m_isRunning) return;

        if (m_options.m_option_targetTrackingMode.value != TargetTrackingMode.Dynamic) return;
        if (m_bpCursorOverTarget) m_totalTimeOverTarget += Time.deltaTime;
    }

    public override void StartTest()
    {
        base.StartTest();
        m_totalTimeOverTarget = 0;
        m_dynamicTargetExited = 0;
        BalancePlateCursor.Instance.SetCursorVisibility(true);
        MoveTarget();
    }

    public override void Selected()
    {
        base.Selected();

        //BalancePlateCursor.Instance.SetCursorVisibility(true);
        Manager_Test.Instance.ChangeInputMode(InputMode.Balance);

        //this is the only test that hides the target selector
        Options_TargetSelect.Instance.Show(false);
    }

    protected override void TargetHit()
    {
        switch (m_options.m_option_targetTrackingMode.value)
        {
            case TargetTrackingMode.Static:
                m_score++;
                m_list_hitTimes.Add(m_targetHitTime);
                m_targetHitTime = 0;
                MoveTarget();
                break;
            case TargetTrackingMode.Dynamic:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Target_Trigger_Hover(Target t)
    {
        //we can use the base target hover if we're in static mode
        if (m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Static)
            base.Target_Trigger_Hover(t);
        //but in dynamic mode we don't want any of the progress ring junk, all we care about is enter/exit
        //which sets and clears m_cursorOverTarget
        //however, we do need to set m_cursorOverTarget = true here in case the bp cursor never actually leaves 
        //the circle and then comes back in to trigger an Enter...
        else m_bpCursorOverTarget = true;
    }

    public override void Target_Trigger_Exit(Target t)
    {
        if (!m_isRunning) return;

        base.Target_Trigger_Exit(t);

        //in a dynamic test, whenever we exit the circle during a test, increment the counter
        if (m_options.m_option_targetTrackingMode.value != TargetTrackingMode.Dynamic) return;
        m_dynamicTargetExited++;
    }

    private void MoveTarget()
    {
        //todo when moving the target if is spawns colliding with the edge collider it will not register a hit and can
        //move off screen.. so we should fix that

        //we want to move the target to a new position only if we're in static mode
        if (m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Static)
        {
            Manager_Targets.Instance.Balance_MoveTarget();

            //weird things can happen if the new location is very close to the old location and the cursor is under the
            //new location as soon as the target moves.  The Enter event will not be called, so we can check for that here
            //and manually call if if necessary
            if (Vector2.Distance(
                    Manager_Targets.Instance.m_trans_target_balance.GetComponent<RectTransform>().anchoredPosition,
                    BalancePlateCursor.Instance.m_rectTransform_base.anchoredPosition) <
                Manager_Targets.Instance.m_bpHoverTarget.m_rectTransform.sizeDelta.x / 2)
                //we're inside the new target
                Target_Trigger_Enter();


            return;
        }

        //handle things for dynamic mode

        float bearing = 0;
        var coin = Random.Range(0, 2);
        switch (m_options.m_option_targetTrackingDirection.value)
        {
            case TargetTrackingDirection.Horizontal:
                bearing = coin == 0 ? 0 : 3.14f;
                break;
            case TargetTrackingDirection.Vertical:
                bearing = coin == 0 ? 1.57f : 4.71f;
                break;
            case TargetTrackingDirection.Diagonal:
                bearing = Random.Range(0, 6.28f);
                break;
            case TargetTrackingDirection.Random:
                //pick a path
                m_activePath = m_paths[Random.Range(0, m_paths.Count)];
                m_activePath.gameObject.SetActive(true);
                m_splineMover.SetPath(m_activePath);
                //adjust he speed by half since the spline move speed is a slightly different scale than our normal one
                m_splineMover.speed = m_options.m_option_targetTrackingSpeed.value / 2;
                m_splineMover.startPoint = 7;
                m_splineMover.StartMove();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Manager_Targets.Instance.Balance_SetTargetInMotion(bearing, m_options.m_option_targetTrackingSpeed.value);
    }

    public override void StopTest(bool stoppedWithButton)
    {
        base.StopTest(stoppedWithButton);

        //BalancePlateCursor.Instance.SetCursorVisibility(false);
        //reset the target position
        BP_HoverTarget.Instance.StopMoving();
        Manager_Targets.Instance.Balance_SetTargetPosition(Vector2.zero);
        //clear the progress ring in case it has any progress
        BalancePlateCursor.Instance.SetProgressRing(0);

        m_splineMover.Stop();
        if (m_activePath != null) m_activePath.gameObject.SetActive(false);

        if (stoppedWithButton) return;

        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_list_touchTimes = m_touchTimes;
        e.m_trainingSummary.m_balance_targetSize =
            m_options.m_option_balance_targetSize.value.ToString();
        e.m_trainingSummary.m_balance_hoverTime =
            m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Dynamic
                ? 0
                : m_options.m_opton_hoverTimeGoal.value;
        e.m_trainingSummary.m_balance_trackingDirection = m_options.m_option_targetTrackingDirection.value;
        e.m_trainingSummary.m_balance_progressDecay =
            m_options.m_option_hoverTimeDecaySpeed.value.ToString();
        e.m_trainingSummary.m_balance_trackingMode = m_options.m_option_targetTrackingMode.value;
        e.m_trainingSummary.m_inputMode = "Balance";
        if (m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Dynamic)
        {
            e.m_trainingSummary.m_balance_targetDeviations = m_dynamicTargetExited;
            e.m_trainingSummary.m_balance_dynamic_totalTimeOverTarget = m_totalTimeOverTarget;
            e.m_trainingSummary.m_balance_percentageOverTarget =
                m_totalTimeOverTarget / (m_timeElapsed <= 0 ? 1f : m_timeElapsed) * 100;

            e.m_trainingSummary.m_shortScore =
                e.m_trainingSummary.m_balance_percentageOverTarget.ToString("0.0") + "%";
        }

        e.m_trainingSummary.m_balance_trackingSpeed = m_options.m_option_targetTrackingSpeed.value;

        FillTestDataToSummary(e);
    }

    public override string GetInfoString()
    {
        if (m_options.m_option_targetTrackingMode.value == TargetTrackingMode.Static)
            return "Score: " + m_score + "\n" +
                   "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
        //dynamic mode
        return "Score: " +
               (m_totalTimeOverTarget / (m_timeElapsed <= 0 ? 1f : m_timeElapsed) * 100).ToString("0.0") + "%" +
               "\n" +
               "Time: " + (m_options.m_option_duration.value - m_timeElapsed).ToString("0.00");
    }
    
    //testing
    private void BgTogglePushed(bool val)
    {
        m_bgTracking = val;
    }
}