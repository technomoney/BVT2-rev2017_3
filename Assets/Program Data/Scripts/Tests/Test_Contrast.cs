using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Contrast : Test
{
    public static Test_Contrast Instance;
    public Color[] m_color_contrastColors;
    private Color m_color_initialTargetColor, m_color_bgColor;
    public Color m_color_selected, m_color_unselected;

    private float[] m_contrastSteps;
    private int m_currentContrastStep;
    private List<Button_ImageButton> m_list_bgButtons, m_list_targetButtons;

    //Note, all of the focused zone logic in this test is identical to Peripheral
    /// <summary>
    ///     The radius of the focus zone with respect to its scale, this tells us how far from the origin we can move
    ///     the target when in focused mode
    /// </summary>
    public float m_radius_focusZone;

    public RectTransform m_rectTransform_focusZone;

    [PropertyTooltip("Pushing space will auto spawn a new target, for testing only")]
    public bool m_SpaceForNewTarget;

    public Sprite m_sprite_background_white,
        m_sprite_target_star,
        m_sprite_target_square,
        m_sprite_target_circle,
        m_sprite_target_cross,
        m_sprite_target_sinCircle,
        m_sprite_target_sinSquare;
    

    public Transform m_trans_targetContent, m_trans_backgroundParent;

    public Button_ImageButton pfb_targetButton, pfb_bgButton;

    public override void Initialize()
    {
        base.Initialize();

        Instance = this;

        //make the backgrounds and targets
        m_list_bgButtons = new List<Button_ImageButton>();
        m_list_targetButtons = new List<Button_ImageButton>();

        //this test is essentially always in auto pace mode..
        m_options.m_option_pacing.Change(TestOption_Pacing.Auto);
        m_testName = "Contrast";
        m_type = TestType.Contrast;
        Manager_Targets.Instance.m_target_contrast.event_targetDown += TargetHit;
        m_trackHitTimes = true;
        m_testIsTimed = false;

        m_list_hitTimes = new List<float>();

        //defaults
        m_options.m_option_blendRate.SetDefault(TestOption_Difficulty.Medium);
        m_options.m_option_paceTime.SetDefault(5);
        m_options.m_option_useOversizeTargets.SetDefault(true);
        m_options.m_option_touchSensitivity.SetDefault(1);
        m_options.m_option_touchSensitivity_keepVisible.SetDefault(false);
        m_options.m_option_audioOnhit.SetDefault(false);
        m_options.m_option_countdowntime.SetDefault(3);
        m_options.m_option_targetSize.SetDefault(.8f);
        m_options.m_option_audioClip.SetDefault(ConstantDefinitions.Instance.m_list_feedbackClips[0]);
        m_options.m_option_focusZoneDistribution.SetDefault(.5f);
        m_options.m_option_focusZoneSize.SetDefault(9);
        m_options.m_option_focusZoneStaysVisisble.SetDefault(false);
        m_options.m_option_useFocusedZone.SetDefault(false);

        //the focus zone should be hidden initially
        m_rectTransform_focusZone.gameObject.SetActive(false);

        //these are the logarithmic functions that determine our blend percentages
        //they are completely arbitrary and made up by Cameron..
        //easy: CONTRAST%=13*log10(10000000*STEP#)-14;
        //medium: CONTRAST%=10*log10(100000*STEP#)+33;
        //hard: CONTRAST%=6*log10(10000000*STEP#)+48;

        //this limits our maximum number of blends to X, so once the user arrives at 50 hits any further targets will
        //just use the highest contrast in the array
        m_contrastSteps = new float[50];
        for (var x = 1; x <= 50; x++)
            switch (m_options.m_option_blendRate.value)
            {
                case TestOption_Difficulty.Easy:
                    m_contrastSteps[x - 1] = (13 * Mathf.Log10(10000000 * x) - 14) / 100f;
                    break;
                case TestOption_Difficulty.Medium:
                    m_contrastSteps[x - 1] = (10 * Mathf.Log10(100000 * x) + 33) / 100f;
                    break;
                case TestOption_Difficulty.Hard:
                    m_contrastSteps[x - 1] = (6 * Mathf.Log10(10000000 * x) + 48) / 100f;
                    break;
            }
    }

    /// <summary>
    ///     Contrast is an annoying test in that it is the only once that uses unique backgrounds and targets..
    /// </summary>
    public void MakeBgsAndTargets()
    {
        foreach (var t in m_color_contrastColors)
        {
            //bg
            var button = Instantiate(pfb_bgButton, m_trans_backgroundParent);
            button.Initialize(m_sprite_background_white);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Bg;
            m_list_bgButtons.Add(button);
            //circle
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_circle);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
            //sin circle
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_sinCircle);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
            //square
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_square);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
            //sin square
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_sinSquare);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
            //star
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_star);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
            //cross
            button = Instantiate(pfb_targetButton, m_trans_targetContent);
            button.Initialize(m_sprite_target_cross);
            button.m_image.color = t;
            button.event_imageButtonPushed += ButtonPushed_Target;
            m_list_targetButtons.Add(button);
        }
    }


    public override void StartTest()
    {
        base.StartTest();

        m_currentContrastStep = 0;
        m_score = 0;
        //since we only have 1 target it should always be active
        Manager_Targets.Instance.m_target_contrast.Activate();

        m_color_initialTargetColor = Manager_Targets.Instance.m_currentTarget_color;
        m_color_bgColor = Manager_Background.Instance.image_background.color;

        //handle our scaling here
        //HandleTargetScaling();

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

        BlendToCurrentStep();
        Manager_Targets.Instance.Contrast_MoveTargetToRandomPosition(Manager_Targets.Instance.m_target_contrast
            .rectTransform.anchoredPosition);
    }

    public override void HandleTargetScaling()
    {
        Manager_Targets.Instance.m_target_contrast.ChangeOption_TargetScale(
            m_options.m_option_useOversizeTargets.value
                ? m_options.m_option_targetSize.value * Manager_Targets.Instance.m_overSizeTargetScaling
                : m_options.m_option_targetSize.value);
    }

    /// <summary>
    ///     set the rgb of the contrast target to the value in the index according to m_currentContrastStep
    ///     the value is not incremented here, so do that before calling this..
    /// </summary>
    private void BlendToCurrentStep()
    {
        var newR = Mathf.Lerp(m_color_initialTargetColor.r, m_color_bgColor.r,
            m_contrastSteps[m_currentContrastStep]);
        var newG = Mathf.Lerp(m_color_initialTargetColor.g, m_color_bgColor.g,
            m_contrastSteps[m_currentContrastStep]);
        var newB = Mathf.Lerp(m_color_initialTargetColor.b, m_color_bgColor.b,
            m_contrastSteps[m_currentContrastStep]);

        Manager_Targets.Instance.m_target_contrast.m_image_targetSprite.color = new Color(newR, newG, newB);
    }

    public override void Update()
    {
        base.Update();
        if (!m_isRunning) return;

        if (m_SpaceForNewTarget && Input.GetKey(KeyCode.Space))
            Manager_Targets.Instance.Contrast_MoveTarget_FocusedZone();

        //this is handled in base since this test tracks hit times..
        //m_targetHitTime += Time.deltaTime;

        Debug.Log(m_targetHitTime.ToString("0.0") + "/" + m_options.m_option_paceTime.value);
        if (m_targetHitTime < m_options.m_option_paceTime.value) return;

        //we've hit the pace time, so this ends the test
        Manager_Test.Instance.StopCurrentTest();
    }

    public override string GetInfoString()
    {
        return "Contrast: " + (100 - m_contrastSteps[m_currentContrastStep] * 100).ToString("0.0") + "%\nScore: " +
               m_score;
    }

    private void ButtonPushed_Bg(Button_ImageButton button)
    {
        //set everything to the unselected color
        m_list_bgButtons.ForEach(b => b.m_image_backing.color = m_color_unselected);
        //then set this one to selected
        button.m_image_backing.color = m_color_selected;

        //then set this to the background
        Manager_Background.Instance.ChangeOption_Background(button.m_image, button.m_image.color);
    }

    private void ButtonPushed_Target(Button_ImageButton button)
    {
        //set everything to the unselected color
        m_list_targetButtons.ForEach(b => b.m_image_backing.color = m_color_unselected);
        //then set this one to selected
        button.m_image_backing.color = m_color_selected;

        //then set this to the selected target
        Manager_Targets.Instance.ChangeOption_TargetGraphic(button.m_image, button.m_image.color);

        //in this test specifically, we'll activate the target after setting it
        Manager_Targets.Instance.m_target_contrast.Activate();
    }

    public override void Selected()
    {
        base.Selected();

        //we have to show the special target and background selections for this test
        Options_TargetSelect.Instance.ShowNormalTargets(false);
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds(false);

        //then manually select a bg and target
        ButtonPushed_Bg(m_list_bgButtons[1]);
        ButtonPushed_Target(m_list_targetButtons[0]);

        HandleTargetScaling();

        //m_testOptions.m_buttonGroup_blendSpeed.m_list_buttons[m_last_blendSpeed].onClick.Invoke();
        //m_testOptions.m_buttonGroup_paceTime.m_list_buttons[m_last_paceTime].onClick.Invoke();
        // m_testOptions.m_toggle_useBigTargets.isOn = m_last_useOversizeTargets;
    }

    protected override void TargetHit(Target t)
    {
        if (!m_isRunning) return;

        m_score++;
        //update the hit times
        m_list_hitTimes.Add(m_targetHitTime);
        m_targetHitTime = 0;

        m_currentContrastStep++;
        if (m_currentContrastStep > 49) m_currentContrastStep = 49;
        BlendToCurrentStep();
        Manager_Targets.Instance.Contrast_MoveTargetToRandomPosition(Manager_Targets.Instance.m_target_contrast
            .rectTransform.anchoredPosition);
    }

    public override void StopTest(bool stoppedWithButton)
    {
        //todo there are various reports of contrast hanging when stopped, I've never been able to repeat it..
        base.StopTest(stoppedWithButton);

        //reset the target position and color
        Manager_Targets.Instance.Contrast_ResetTarget();

        if (m_score <= 0 || stoppedWithButton) return;

        //prepare the scoreboard entry for the graphs and such
        var e = Instantiate(Menu_Reports.Instance.pfb_entry_scoreboard,
            Menu_Reports.Instance.m_trans_contentParent);

        e.m_trainingSummary.m_contrast_blendSpeed = m_options.m_option_blendRate.value;
        e.m_trainingSummary.m_contrast_oversizeTargets = m_options.m_option_useOversizeTargets.value ? "Yes" : "No";
        e.m_trainingSummary.m_contrast_paceTime = m_options.m_option_paceTime.value;
        e.m_trainingSummary.m_list_contrastSteps = GetContrastList(m_list_hitTimes.Count);
        e.m_trainingSummary.m_contrast_lowestAchieved = e.m_trainingSummary.m_list_contrastSteps.Last();
        e.m_trainingSummary.m_shortScore =
            (e.m_trainingSummary.m_list_contrastSteps.Last() * 100).ToString("0.0") + "%";

        FillTestDataToSummary(e);
    }

    private List<float> GetContrastList(int hits)
    {
        var steps = new List<float>();
        for (var x = 0; x < hits; x++)
            //there are only 50 contrast steps, so if we somehow get more hits than
            //that we don't want to go out of range..
            //todo if the number of contrast steps become variable, make sure to change this too
            steps.Add(1 - m_contrastSteps[x > 49 ? 49 : x]);

        return steps;
    }
}