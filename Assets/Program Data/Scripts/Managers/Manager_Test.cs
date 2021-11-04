using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum TestMode
{
    Idle,
    Running,
    Countdown
}


public class Manager_Test : MonoBehaviour
{
    public delegate void Manager_Test_Event();

    public static Manager_Test Instance;
    public Manager_Test_Event event_inputModeChanged;
    public Manager_Test_Event event_newTestSelected;
    public Button m_button_setFocusZone_Cancel;

    public Button m_button_stopTest, m_button_startTest;

    public Canvas m_canvas;
    public GameObject m_gameObj_optionsMenu;

    public GameObject m_gameObj_setFocusZoneTools;

    /// <summary>
    ///     A global option, set only by the system menu, all tests are either yes/no to record data based on this bool
    /// </summary>
    public bool m_recordBpData;

    public Test m_selectedTest;
    public TestType m_selectedTestType;
    private bool m_settingFocusZone;

    public TestMode m_mode { get; private set; }

    public InputMode m_testInputMode { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log(GetType() + " starting");

        m_button_stopTest.onClick.AddListener(ButtonPushed_StopTest);
        m_button_startTest.onClick.AddListener(ButtonPushed_StartTest);

        m_mode = TestMode.Idle;

        //stop button is initially hidden
        m_button_stopTest.gameObject.SetActive(false);

        m_button_setFocusZone_Cancel.onClick.AddListener(ButtonPushed_FocusZoneCancel);
        //focus zone junk is hidden initially
        m_gameObj_setFocusZoneTools.SetActive(false);

        m_testInputMode = InputMode.Touch;

        //initialize all of our tests
        GetComponent<Test_Speed>().Initialize();
        GetComponent<Test_Peripheral>().Initialize();
        GetComponent<Test_Memory>().Initialize();
        GetComponent<Test_Reaction>().Initialize();
        GetComponent<Test_Bp_TargetHover>().Initialize();
        GetComponent<Test_GoNoGo>().Initialize();
        GetComponent<Test_Flash>().Initialize();
        GetComponent<Test_Rhythm>().Initialize();
        GetComponent<Test_Contrast>().Initialize();
        GetComponent<Test_Multi>().Initialize();
        StartCoroutine(DelayedStartup());
    }

    /// <summary>
    ///     this handles the loading of the backgrounds and targets so everything show up when we expect it to.
    ///     This MUST be last in the script execution order, and the issues that this corrects DO NOT occur in the
    ///     editor mode, only in built versions..
    /// </summary>
    private IEnumerator DelayedStartup()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("Custom Sprites").GetComponent<CustomSprites>().Initialize();
        yield return new WaitForSeconds(.2f);
        GameObject.Find("Background Image Select").GetComponent<Options_BackgroundSelect>().Initialize();
        yield return new WaitForSeconds(.2f);
        GameObject.Find("Target Select").GetComponent<Options_TargetSelect>().Initialize();
        GetComponent<Test_Contrast>().MakeBgsAndTargets();
        GameObject.Find("Main Menu").GetComponent<Menu_Main>().Initialize();
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds(false);
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds();

        //todo timing is a big issue during startup
        //especially with loading custom things, best case a loading screen should be used to make sure everything loads
        //correctly before the user can start clicking around
        //the waiting above is a band aid, we'll want to use a real confirmation at some point in the future..
        //especially since those delays will only work to a given point, with enough custom things to load that
        //won't be enough time anyway..
    }


    private void Update()
    {
        if (m_settingFocusZone && Input.GetMouseButtonDown(0))
        {
            Vector2 v;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvas.transform as RectTransform,
                Input.mousePosition, m_canvas.worldCamera, out v);

            Debug.Log("Setting Focused zone to: " + v);

            //see if we're hitting right on top of the cancel button
            //this is a slight mess since the test manager and the button are in different parent transforms we 
            //can't use the contains() on the rect of the button.  So basically we make sure the point being clicked
            //isn't withtin the bottom left corner of the cancel button, since this calls the setZone(false) function,
            //it may be a bit redundant to have the cancel button do the same, but can't hurt..
            var ap = m_button_setFocusZone_Cancel.GetComponent<RectTransform>();
            if (v.x < ap.anchoredPosition.x + ap.rect.width / 2 && v.y < ap.anchoredPosition.y + ap.rect.height / 2)
            {
                SetFocusZoneOrigin(false);
                return;
            }

            //otherwise, set the place touched as the origin for the focus zone
            Test_Peripheral.Instance.m_rectTransform_focusZone.anchoredPosition = v;

            SetFocusZoneOrigin(false);
        }
    }

    public void SetActiveTest(int testIndex)
    {
        switch (testIndex)
        {
            case 0:
                m_selectedTestType = TestType.Speed;
                m_selectedTest = GetComponent<Test_Speed>();
                break;
            case 1:
                m_selectedTestType = TestType.Peripheral;
                m_selectedTest = GetComponent<Test_Peripheral>();
                break;
            case 2:
                m_selectedTestType = TestType.Sequence;
                m_selectedTest = GetComponent<Test_Memory>();
                break;
            case 3:
                m_selectedTestType = TestType.Reaction;
                m_selectedTest = GetComponent<Test_Reaction>();
                break;
            case 4:
                m_selectedTestType = TestType.Balance;
                m_selectedTest = GetComponent<Test_Bp_TargetHover>();
                break;
            case 5:
                m_selectedTestType = TestType.GoNoGo;
                m_selectedTest = GetComponent<Test_GoNoGo>();
                break;
            case 6:
                m_selectedTestType = TestType.Flash;
                m_selectedTest = GetComponent<Test_Flash>();
                break;
            case 7:
                m_selectedTestType = TestType.Rhythm;
                m_selectedTest = GetComponent<Test_Rhythm>();
                break;
            case 8:
                m_selectedTestType = TestType.Contrast;
                m_selectedTest = GetComponent<Test_Contrast>();
                break;
            case 9:
                m_selectedTestType = TestType.Multi;
                m_selectedTest = GetComponent<Test_Multi>();
                break;
            default:
                Debug.Log("Error assigning Active Test..");
                break;
        }

        Debug.Log("Active Test: " + m_selectedTestType);
        event_newTestSelected();
    }

    private void ButtonPushed_StartTest()
    {
        //if we ever manage to click this without having a user logged in, do that now..
        if (Manager_Users.Instance.m_currentUser == null)
        {
            Manager_Users.Instance.ShowOnStartUp();
            return;
        }
        
        //we shouldn't be able to do this if a test is already running..
        if (m_mode != TestMode.Idle) return;

        //if the current test is in balance mode, we need to make sure the balance plate is connected
        if (m_testInputMode == InputMode.Balance)
            if (BalancePlate.Instance.IsNull())
            {
                Debug.Log("Trying to start a test in balance mode with a null force plate");
                return;
            }
        //todo should probably have some kind of popup or message here..

        //hide everything
        Menu_Main.Instance.gameObject.SetActive(false);
        m_gameObj_optionsMenu.SetActive(false);
        Menu_Reports.Instance.gameObject.SetActive(false);
        Banner_StartSummary.Instance.gameObject.SetActive(false);
        Manager_SummaryGraphs.Instance.HideAll();
        PatientDisplay.Instance.Show(false);

        //show the stop button
        m_button_stopTest.gameObject.SetActive(true);

        //Show the display
        Display_TestInfo.Instance.Show();
        //clear the info box so it doesn't show leftover junk from the last test
        Display_TestInfo.Instance.Clear();

        //deal with the sensitivity
        Manager_Targets.Instance.SetSensitivityVisibility(m_selectedTest.m_options.m_option_touchSensitivity_keepVisible
            .value);

        //check for pre-spawning of targets only when in a balance mode
        if (m_testInputMode != InputMode.Touch) m_selectedTest.PreSpawnTarget();

        //but the test doesn't actually start until the countdown calls CountdownFinished()
        //the reaction test is immune to the countdown, so set the effective time to 0 for that test only
        //it is muy important that the test mode is changed here, not after the startcountdown() call!!
        m_mode = TestMode.Countdown;
        Countdown.Instance.StartCountdown(m_selectedTest.m_type == TestType.Reaction
            ? 0f
            : m_selectedTest.m_options.m_option_countdowntime.value);
    }

    /// <summary>
    ///     this should ONLY be called by Countdown.  This is what actually starts the test
    /// </summary>
    public void CountdownFinished()
    {
        //we should only be able to get here in countdown mode
        if (m_mode != TestMode.Countdown) return;

        m_mode = TestMode.Running;

        //start the test
        m_selectedTest.StartTest();
    }

    private void ButtonPushed_StopTest()
    {
        if (m_mode == TestMode.Countdown)
            Countdown.Instance.HaltCountdown();

        StopCurrentTest(true);
    }

    public void StopCurrentTest(bool stoppedWithButton = false)
    {
        //stop the currently running test, if there is one..
        if (m_mode == TestMode.Idle) return;

        m_mode = TestMode.Idle;
        m_selectedTest.StopTest(stoppedWithButton);

        //bring everything back
        Menu_Main.Instance.gameObject.SetActive(true);
        m_gameObj_optionsMenu.SetActive(true);
        Menu_Reports.Instance.gameObject.SetActive(true);
        Banner_StartSummary.Instance.gameObject.SetActive(true);
        PatientDisplay.Instance.Show(true);
        //hide the stop button
        m_button_stopTest.gameObject.SetActive(false);
        //hide the display
        Display_TestInfo.Instance.Hide();
        //hide the countdown in case the test was stopped during the countdown
        Countdown.Instance.Finish();
        //clear all targets
        Manager_Targets.Instance.ClearTargets();
        //show sensitivity
        Manager_Targets.Instance.SetSensitivityVisibility(true);
        //show or hide the focus zone
        Test_Peripheral.Instance.m_rectTransform_focusZone.gameObject.SetActive(m_selectedTest.m_options
            .m_option_useFocusedZone.value);
    }

    public void SetFocusZoneOrigin(bool set)
    {
        //hide everything
        Menu_Main.Instance.gameObject.SetActive(!set);
        Banner_StartSummary.Instance.gameObject.SetActive(!set);
        Menu_Reports.Instance.gameObject.SetActive(!set);
        m_gameObj_optionsMenu.SetActive(!set);
        Display_TestInfo.Instance.gameObject.SetActive(!set);
        Manager_Targets.Instance.gameObject.SetActive(!set);
        //show the tools
        m_gameObj_setFocusZoneTools.SetActive(set);
        m_settingFocusZone = set;
    }

    private void ButtonPushed_FocusZoneCancel()
    {
        SetFocusZoneOrigin(false);
    }

    public bool IsTestRunning()
    {
        return Test_Peripheral.Instance.m_isRunning ||
               Test_Speed.Instance.m_isRunning ||
               Test_Reaction.Instance.m_isRunning ||
               Test_Memory.Instance.m_isRunning;
    }

    public void ChangeInputMode(InputMode newMode)
    {
        m_testInputMode = newMode;

        event_inputModeChanged();
    }

    public void SetMenuVisibility(bool set)
    {
        m_gameObj_optionsMenu.SetActive(set);
        m_button_stopTest.gameObject.SetActive(set);
    }
}