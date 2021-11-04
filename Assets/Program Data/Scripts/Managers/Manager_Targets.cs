using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
///     Target manager is a monster class that handles anything involving moving, hiding, showing targets for any test
/// </summary>
public class Manager_Targets : SerializedMonoBehaviour
{
    public static Manager_Targets Instance;

    public readonly float m_overSizeTargetScaling = 5;


    public Sprite defaultTargetGraphic;

    public BP_HoverTarget m_bpHoverTarget;
    public Color m_currentTarget_color;

    /// <summary>
    ///     need a reference to the focus zone so we can hide it when
    ///     switching away from the peripheral test
    /// </summary>
    public GameObject m_gameObj_focusZone;

    /// <summary>
    ///     the prefab for making all targets
    /// </summary>
    public Target m_pfb_target;

    /// <summary>
    ///     the prefab for making the contrast targets, the only difference here is the default image will not
    ///     have the border..
    /// </summary>
    public Target m_pfb_target_contrast;

    /// <summary>
    ///     we need to know the screen bounds for the bp cursor
    /// </summary>
    [HideInEditorMode] public Vector2 m_screenBounds;

    public bool m_showDebugCoordinates;


    //a reference to the special peripheral targets
    [HideInEditorMode] public Target m_target_peripheral, m_target_rhythm, m_target_contrast;

    // The list of targets for specific tests
    [HideInEditorMode] public List<Target> m_targets_speed,
        m_targets_reaction,
        m_targets_memory,
        m_targets_goNoGo,
        m_targets_flash,
        m_targets_multi;

    public Transform m_trans_target_balance;
    public Transform m_trans_target_contrast;
    public Transform[] m_trans_target_multi;
    public Transform m_trans_target_peripheral;
    public Transform m_trans_target_rhythm;
    public Transform m_trans_targets_flash;
    public Transform m_trans_targets_goNoGo;
    public Transform m_trans_targets_memory;
    public Transform m_trans_targets_reaction;

    [InfoBox("Parent transforms for each set of targets")]
    public Transform m_trans_targets_speed;

    //general options
    /// <summary>
    ///     Do we show the sensitivity ring when the test is running?
    /// </summary>
    [HideInEditorMode]
    public bool m_showTargetSensitivityDuringTest { private set; get; }

    public Sprite currentTargetGraphic { get; private set; }

    // Use this for initialization
    private void Start()
    {
        Instance = this;
        currentTargetGraphic = defaultTargetGraphic;
        m_screenBounds = new Vector2(GetComponent<RectTransform>().rect.width,
            GetComponent<RectTransform>().rect.height);
        Debug.Log("Target Bounds: " + m_screenBounds);
        m_targets_speed = new List<Target>();
        m_targets_memory = new List<Target>();
        m_targets_reaction = new List<Target>();
        m_targets_goNoGo = new List<Target>();
        m_targets_flash = new List<Target>();
        m_targets_multi = new List<Target>();

        //make all the targets we're going to need
        //speed
        MakeTarget("Target_Speed_Center_", 0, TargetLocation.Inner, m_trans_targets_speed, m_targets_speed);
        for (var x = 1; x < 9; x++)
            MakeTarget("Target_Speed_Inner_" + x, x, TargetLocation.Inner, m_trans_targets_speed, m_targets_speed);
        for (var x = 9; x < 17; x++)
            MakeTarget("Target_Speed_Middle_" + x, x, TargetLocation.Middle, m_trans_targets_speed, m_targets_speed);
        for (var x = 17; x < 33; x++)
            MakeTarget("Target_Speed_Outer_" + x, x, TargetLocation.Outer, m_trans_targets_speed, m_targets_speed);

        //peripheral
        m_target_peripheral = Instantiate(m_pfb_target, m_trans_target_peripheral);
        m_target_peripheral.Initialize(0, TargetLocation.Peripheral);
        m_target_peripheral.name = "Target_Peripheral";

        //memory
        //center target
        MakeTarget("Target_Memory_Center_", 0, TargetLocation.Inner, m_trans_targets_memory, m_targets_memory);
        for (var x = 1; x < 9; x++)
            MakeTarget("Target_Memory_Inner_" + x, x, TargetLocation.Inner, m_trans_targets_memory, m_targets_memory);
        for (var x = 9; x < 17; x++)
            MakeTarget("Target__Memory_Middle_" + x, x, TargetLocation.Middle, m_trans_targets_memory,
                m_targets_memory);
        for (var x = 17; x < 33; x++)
            MakeTarget("Target_Memory_Outer_" + x, x, TargetLocation.Outer, m_trans_targets_memory, m_targets_memory);

        //reaction
        //the indices here are odd to match the way we set the positions in ChangeOption_TargetGridSize()
        MakeTarget("Target_Reaction_E", 0, TargetLocation.Reation, m_trans_targets_reaction, m_targets_reaction);
        MakeTarget("Target_Reaction_S", 1, TargetLocation.Reation, m_trans_targets_reaction, m_targets_reaction);
        MakeTarget("Target_Reaction_W", 2, TargetLocation.Reation, m_trans_targets_reaction, m_targets_reaction);
        MakeTarget("Target_Reaction_N", 3, TargetLocation.Reation, m_trans_targets_reaction, m_targets_reaction);

        //go/no go
        MakeTarget("Target_Speed_Center_", 0, TargetLocation.Inner, m_trans_targets_goNoGo, m_targets_goNoGo);
        for (var x = 1; x < 9; x++)
            MakeTarget("Target_Speed_Inner_" + x, x, TargetLocation.Inner, m_trans_targets_goNoGo, m_targets_goNoGo);
        for (var x = 9; x < 17; x++)
            MakeTarget("Target_Speed_Middle_" + x, x, TargetLocation.Middle, m_trans_targets_goNoGo, m_targets_goNoGo);
        for (var x = 17; x < 33; x++)
            MakeTarget("Target_Speed_Outer_" + x, x, TargetLocation.Outer, m_trans_targets_goNoGo, m_targets_goNoGo);

        //flash
        MakeTarget("Target_Speed_Center_", 0, TargetLocation.Inner, m_trans_targets_flash, m_targets_flash);
        for (var x = 1; x < 9; x++)
            MakeTarget("Target_Speed_Inner_" + x, x, TargetLocation.Inner, m_trans_targets_flash, m_targets_flash);
        for (var x = 9; x < 17; x++)
            MakeTarget("Target_Speed_Middle_" + x, x, TargetLocation.Middle, m_trans_targets_flash, m_targets_flash);
        for (var x = 17; x < 33; x++)
            MakeTarget("Target_Speed_Outer_" + x, x, TargetLocation.Outer, m_trans_targets_flash, m_targets_flash);

        //rhythm
        m_target_rhythm = Instantiate(m_pfb_target, m_trans_target_rhythm);
        m_target_rhythm.Initialize(0, TargetLocation.Rhythm);
        m_target_rhythm.name = "Target_Rhythm";

        //contrast
        m_target_contrast = Instantiate(m_pfb_target_contrast, m_trans_target_contrast);
        m_target_contrast.Initialize(0, TargetLocation.Contrast);
        m_target_contrast.name = "Target_Contrast";

        //multi
        MakeTarget("Target_Multi_0_0", 0, TargetLocation.Multi1, m_trans_target_multi[0], m_targets_multi);
        MakeTarget("Target_Multi_0_1", 1, TargetLocation.Multi1, m_trans_target_multi[0], m_targets_multi);
        MakeTarget("Target_Multi_1_0", 2, TargetLocation.Multi2, m_trans_target_multi[1], m_targets_multi);
        MakeTarget("Target_Multi_1_1", 3, TargetLocation.Multi2, m_trans_target_multi[1], m_targets_multi);
        MakeTarget("Target_Multi_2_0", 4, TargetLocation.Multi3, m_trans_target_multi[2], m_targets_multi);
        MakeTarget("Target_Multi_2_1", 5, TargetLocation.Multi3, m_trans_target_multi[2], m_targets_multi);
        MakeTarget("Target_Multi_3_0", 6, TargetLocation.Multi4, m_trans_target_multi[3], m_targets_multi);
        MakeTarget("Target_Multi_3_1", 7, TargetLocation.Multi4, m_trans_target_multi[3], m_targets_multi);


        //then set it to default spacing
        ChangeOption_TargetGridSize(128);
        SetSensitivityVisibility(true);
    }


    private void MakeTarget(string targetName, int index, TargetLocation location, Transform parent, List<Target> list)
    {
        var t = Instantiate(m_pfb_target, parent);
        t.Initialize(index, location);
        t.name = targetName;
        list.Add(t);
    }


    public void ChangeOption_TargetGridSize(float gridSpacingMagnitude)
    {
        //speed, memory, and go/No-go are set up the exact same way, this working depends heavily on these lists
        //being set up the exact same way!

        #region Speed/Sequence

        var magnitude = gridSpacingMagnitude;
        var interval = 0.785398f;
        var angle = 0f;
        Vector2 tempVec2;
        //inner targets
        for (var index = 1; index < 9; index++)
        {
            tempVec2 = new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
            m_targets_speed[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_memory[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_goNoGo[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_flash[index].rectTransform.anchoredPosition = tempVec2;
            angle += interval;
        }

        //middle
        magnitude = gridSpacingMagnitude * 2;
        interval = 0.785398f;
        angle = 0f;
        for (var index = 9; index < 17; index++)
        {
            tempVec2 = new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
            m_targets_speed[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_memory[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_goNoGo[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_flash[index].rectTransform.anchoredPosition = tempVec2;
            angle += interval;
        }

        //outer
        magnitude = gridSpacingMagnitude * 3;
        interval = .3926991f;
        angle = 0f;
        for (var index = 17; index < 33; index++)
        {
            tempVec2 = new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
            m_targets_speed[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_memory[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_goNoGo[index].rectTransform.anchoredPosition = tempVec2;
            m_targets_flash[index].rectTransform.anchoredPosition = tempVec2;
            angle += interval;
        }

        #endregion

        //peripheral is immune to grid changes so we can ignore it

        //reaction behaves as though it were in the outer ring with magnitude, but with a 90 degree interval
        magnitude = gridSpacingMagnitude * 3;
        interval = 1.571f;
        angle = 0f;
        foreach (var t in m_targets_reaction)
        {
            t.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle) * magnitude,
                Mathf.Sin(angle) * magnitude);
            angle += interval;
        }

        //multi has three sets of targets at 90 and 270 and operates on a smaller scale than the other targets
        Multi_ResetTargetPositions(gridSpacingMagnitude);

        //contrast is immune to grid changes
    }

    public void Multi_ResetTargetPositions(float gridSpacingMagnitude)
    {
        //float magnitude = ConstantDefinitions.Instance.gridSize[Options_TargetLayout.Instance.m_currentGridSize];
        //float magnitude = Manager_Test.Instance.m_selectedTest.m_options.m_option_gridSize.value;
        var magnitude = gridSpacingMagnitude;
        //targets at 90
        var angle = 1.57f;
        m_targets_multi[0].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[2].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[4].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[6].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);

        //targets at 270
        angle = 4.71f;
        m_targets_multi[1].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[3].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[5].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
        m_targets_multi[7].rectTransform.anchoredPosition =
            new Vector2(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude);
    }


    public void ChangeOption_ShowTargetSensitivityDuringTest(bool newSetting)
    {
        m_showTargetSensitivityDuringTest = newSetting;
    }

    public void ChangeOption_TargetGraphicScale(float newScale)
    {
        m_targets_speed.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_target_peripheral.ChangeOption_TargetScale(newScale);
        m_targets_memory.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_targets_reaction.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_targets_goNoGo.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_targets_flash.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_target_rhythm.ChangeOption_TargetScale(newScale);
        m_targets_multi.ForEach(t => t.ChangeOption_TargetScale(newScale));
        m_target_contrast.ChangeOption_TargetScale(newScale);
    }

    public void ChangeOption_TargetGraphic(Image newImage)
    {
        currentTargetGraphic = newImage.sprite;
        m_currentTarget_color = Color.white;
    }

    public void ChangeOption_TargetGraphic(Image newImage, Color color)
    {
        currentTargetGraphic = newImage.sprite;
        m_currentTarget_color = color;
    }

    public void ChangeOption_TouchSensitivity(float newSensitivity)
    {
        m_targets_speed.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_target_peripheral.ChangeOption_TouchSensitivity(newSensitivity);
        m_targets_memory.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_targets_reaction.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_targets_goNoGo.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_targets_flash.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_target_rhythm.ChangeOption_TouchSensitivity(newSensitivity);
        m_targets_multi.ForEach(t => t.ChangeOption_TouchSensitivity(newSensitivity));
        m_target_contrast.ChangeOption_TouchSensitivity(newSensitivity);
    }

    /// <summary>
    ///     Show the targets for the given test type, all others will be hidden
    /// </summary>
    public void ShowTargetSet(TestType type)
    {
        //hide everything initially
        m_trans_targets_speed.gameObject.SetActive(false);
        m_trans_target_peripheral.gameObject.SetActive(false);
        m_trans_targets_memory.gameObject.SetActive(false);
        m_trans_targets_reaction.gameObject.SetActive(false);
        m_trans_target_balance.gameObject.SetActive(false);
        m_trans_targets_goNoGo.gameObject.SetActive(false);
        m_trans_targets_flash.gameObject.SetActive(false);
        m_trans_target_rhythm.gameObject.SetActive(false);
        m_trans_target_multi[0].gameObject.SetActive(false);
        m_trans_target_multi[1].gameObject.SetActive(false);
        m_trans_target_multi[2].gameObject.SetActive(false);
        m_trans_target_multi[3].gameObject.SetActive(false);
        m_trans_target_contrast.gameObject.SetActive(false);

        m_gameObj_focusZone.SetActive(false);

        switch (type)
        {
            case TestType.Speed:
                m_trans_targets_speed.gameObject.SetActive(true);
                break;
            case TestType.Peripheral:
                m_trans_target_peripheral.gameObject.SetActive(true);
                m_gameObj_focusZone.SetActive(Test_Peripheral.Instance.m_options.m_option_useFocusedZone.value);
                break;
            case TestType.Sequence:
                m_trans_targets_memory.gameObject.SetActive(true);
                break;
            case TestType.Reaction:
                m_trans_targets_reaction.gameObject.SetActive(true);
                break;
            case TestType.Balance:
                m_trans_target_balance.gameObject.SetActive(true);
                break;
            case TestType.GoNoGo:
                m_trans_targets_goNoGo.gameObject.SetActive(true);
                break;
            case TestType.Flash:
                m_trans_targets_flash.gameObject.SetActive(true);
                break;
            case TestType.Rhythm:
                m_trans_target_rhythm.gameObject.SetActive(true);
                break;
            case TestType.Multi:
                m_trans_target_multi[0].gameObject.SetActive(true);
                m_trans_target_multi[1].gameObject.SetActive(true);
                m_trans_target_multi[2].gameObject.SetActive(true);
                m_trans_target_multi[3].gameObject.SetActive(true);
                break;
            case TestType.Contrast:
                m_trans_target_contrast.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("Error showing target set for type: " + type);
                break;
        }
    }


    /// <summary>
    ///     Set the visibility of the sensitivity rings based on the current setting of the global option
    /// </summary>
    public void SetSensitivityVisibility(bool show)
    {
        if (Manager_Test.Instance == null || Manager_Test.Instance.m_selectedTest == null) return;
        switch (Manager_Test.Instance.m_selectedTest.m_type)
        {
            case TestType.Speed:
                m_targets_speed.ForEach(t => t.ShowSensitivityRing(show));
                return;
            case TestType.Peripheral:
                m_target_peripheral.ShowSensitivityRing(show);
                return;
            case TestType.Sequence:
                m_targets_memory.ForEach(t => t.ShowSensitivityRing(show));
                return;
            case TestType.Reaction:
                m_targets_reaction.ForEach(t => t.ShowSensitivityRing(show));
                return;
            case TestType.Balance:
                //no changes here for balance..
                return;
            case TestType.Flash:
                m_targets_flash.ForEach(t => t.ShowSensitivityRing(show));
                return;
            case TestType.GoNoGo:
                m_targets_goNoGo.ForEach(t => t.ShowSensitivityRing(show));
                return;
            case TestType.Contrast:
                m_target_contrast.ShowSensitivityRing(show);
                break;
            case TestType.Rhythm:
                m_target_rhythm.ShowSensitivityRing(show);
                break;
            case TestType.Multi:
                m_targets_multi.ForEach(t => t.ShowSensitivityRing(show));
                break;
            case TestType.All:
            case TestType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     Deactivate all targets for the given target set, Type.All will deactivate all targets
    /// </summary>
    public void ClearTargets(TestType type = TestType.All)
    {
        switch (type)
        {
            case TestType.Speed:
                m_targets_speed.ForEach(t => t.Deactivate());
                break;
            case TestType.Peripheral:
                m_target_peripheral.Deactivate();
                break;
            case TestType.Reaction:
                m_targets_reaction.ForEach(t => t.Deactivate());
                break;
            case TestType.Sequence:
                m_targets_memory.ForEach(t => t.Deactivate());
                break;
            case TestType.GoNoGo:
                m_targets_goNoGo.ForEach(t => t.Deactivate());
                break;
            case TestType.Flash:
                m_targets_flash.ForEach(t => t.Deactivate());
                break;
            case TestType.Rhythm:
                m_target_rhythm.Deactivate();
                break;
            case TestType.Multi:
                m_targets_multi.ForEach(t => t.Deactivate());
                break;
            case TestType.Contrast:
                m_target_contrast.Deactivate(false);
                break;
            case TestType.All:
                m_targets_speed.ForEach(t => t.Deactivate());
                m_target_peripheral.Deactivate();
                m_targets_memory.ForEach(t => t.Deactivate());
                m_targets_reaction.ForEach(t => t.Deactivate());
                m_targets_goNoGo.ForEach(t => t.Deactivate());
                m_targets_flash.ForEach(t => t.Deactivate());
                m_target_rhythm.Deactivate();
                m_targets_multi.ForEach(t => t.Deactivate());
                m_target_contrast.Deactivate(false);
                break;
            case TestType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    /// <summary>
    ///     this will activate a random target on the speed grid that does not have the same index of the given value
    ///     this stops the same targhet from being activated twice in a row
    /// </summary>
    /// <returns>the index of the target we activated</returns>
    public int Speed_ActivateRandomTarget(int lastActiveTarget = -1)
    {
        //first clear all of our existing targets
        ClearTargets(TestType.Speed);

        //todo theoretical infinite loop..
        //loop and find an index that doesn't equal the last index we used when returning
        //a target, this could theoretically loop infinitely...but we're probably definitely maybe fine
        var randomIndex = -100;
        do
        {
            //first pick a ring
            var r = Random.Range(0, 3);
            switch (r)
            {
                case 0: //inner
                    randomIndex = Random.Range(0, 9);
                    break;
                case 1: //middle
                    randomIndex = Random.Range(9, 17);
                    break;
                case 2: //outer
                    randomIndex = Random.Range(17, 33);
                    break;
                default:
                    Debug.Log("Error in ActivateRandomTarget_Speed()");
                    break;
            }
        } while (randomIndex == lastActiveTarget);

        //now that we have a valid index, activate the target
        var target = m_targets_speed.Find(t => t.m_index == randomIndex);
        if (target == null)
        {
            Debug.Log("Error assign active index in ActivateRandomTarget_Speed()");
            return -100;
        }

        target.Activate();

        return randomIndex;
    }

    /// <summary>
    ///     This will move the peripheral target to a new random position using the whole sceen
    /// </summary>
    public void Peripheral_MoveTarget_FullScreen()
    {
        //get a random position for x and y that respect the edge padding of the screen
        float xPos = 0, yPos = 0;
        do //todo theoretical infinite loop..
        {
            int coin;
            do //x
            {
                coin = Random.Range(0, 2);
                xPos = coin == 0
                    ? Random.Range(0, m_screenBounds.x / 2)
                    : Random.Range(m_screenBounds.x / 2 * -1, 0);
            } while (!PositionWithinEdgePadding(xPos, m_screenBounds.x / 2));

            do //y
            {
                coin = Random.Range(0, 2);
                yPos = coin == 0
                    ? Random.Range(0, m_screenBounds.y / 2)
                    : Random.Range(m_screenBounds.y / 2 * -1, 0);
            } while (!PositionWithinEdgePadding(yPos, m_screenBounds.y / 2));

            //getting here is a valid screen position based on the bounds, we also need to make sure it isn't underneath
            //the start/stop button or the info display
        } while (Position_ObstructedByUi(new Vector2(xPos, yPos), m_target_peripheral.rectTransform.sizeDelta.x / 2));

        //this is a valid position based on the screen padding and the rects of the things that could possibly obstruct 
        //the target
        m_target_peripheral.rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }

    /// <summary>
    ///     move the peripheral target within the focused zone
    /// </summary>
    public void Peripheral_MoveTarget_FocusedZone()
    {
        //todo should have a counter in all do while loop and break out with a default value to prevent the program from hanging
        var radius = Test_Peripheral.Instance.m_radius_focusZone;
        var origin = Test_Peripheral.Instance.m_rectTransform_focusZone.anchoredPosition;
        var distribution = Test_Peripheral.Instance.m_options.m_option_focusZoneDistribution.value;
        bool inCircle;
        float xPos = 0, yPos = 0;
        Vector2 pos;
        float minRadius, maxRadius;
        //distribution is the chance that the target should appear inside the target
        //if the distribution is a absolute, no need to roll
        if (distribution <= 0)
        {
            inCircle = false;
        }
        else if (distribution >= 1)
        {
            inCircle = true;
        }
        else
        {
            var chance = Random.Range(0, 1f);
            inCircle = chance <= distribution;
        }

        if (inCircle)
        {
            minRadius = 0;
            maxRadius = radius;
        }
        else
        {
            minRadius = radius;
            maxRadius = Screen.width;
        }

        do //todo theoretical infinite loop..
        {
            //first, get a magnitude up to our radius
            var magnitude = Random.Range(minRadius, maxRadius);
            //now get a random angle to apply the radius value 
            var angle = Random.Range(0f, 6.28f);
            //that will give us two points
            xPos = Mathf.Cos(angle) * magnitude;
            yPos = Mathf.Sin(angle) * magnitude;
            //we need to make sure these are in the screen and not obstructed like the when moving the target fullscreen
            //we also have to offset the position by the focus zone
            pos = new Vector2(origin.x + xPos, origin.y + yPos);
        } while (!PositionOnScreen(pos) ||
                 !PositionWithinEdgePadding(pos.x, m_screenBounds.x / 2) ||
                 !PositionWithinEdgePadding(pos.y, m_screenBounds.y / 2) ||
                 Position_ObstructedByUi(pos, m_target_peripheral.rectTransform.sizeDelta.x / 2));


        //this is a valid position based on the screen padding and the rects of the things that could possibly obstruct 
        m_target_peripheral.rectTransform.anchoredPosition = pos;

        //todo still seems possible for a target to appear off screen if the circle bounds are off screen..
    }

    /// <summary>
    ///     See if the given position is contained within either rect of the start/stop button
    ///     or the info display
    /// </summary>
    private bool Position_ObstructedByUi(Vector2 pos, float targetRadius)
    {
        //the peri bounds are points with a safe padding included that the targets should not be below or outside of
        //so with the target radius we can say that the position given should be at least a radius away from these
        //bounds positions..
        var posMod = new Vector2(pos.x - targetRadius, pos.y - targetRadius);
        //info panel
        if (posMod.x < ConstantDefinitions.Instance.peri_bounds_infoDisplay.x &&
            posMod.y < ConstantDefinitions.Instance.peri_bounds_infoDisplay.y)
            return true;

        //start/stop button
        posMod = new Vector2(pos.x + targetRadius, pos.y - targetRadius);
        if (posMod.x > ConstantDefinitions.Instance.peri_bounds_stop.x &&
            posMod.y < ConstantDefinitions.Instance.peri_bounds_stop.y)
            return true;

        //todo we'll want to make sure a target isn't obstructed by the patient name plate

        return false;
    }

    /// <summary>
    ///     check if the given coordinate is within the constant defined % distance away from the given max
    /// </summary>
    private bool PositionWithinEdgePadding(float coord, float max)
    {
        var validRange = max - max * ConstantDefinitions.Instance.peri_edgePadding;

        return Math.Abs(coord) <= validRange;
    }

    /// <summary>
    ///     check if the given coordinate is within the bounds of the visible screen
    /// </summary>
    private bool PositionOnScreen(Vector2 pos)
    {
        var maxX = m_screenBounds.x / 2;
        var maxY = m_screenBounds.y / 2;

        if (m_showDebugCoordinates)
        {
            Debug.Log(pos.x + " / " + maxX + ", " + pos.y + " / " + maxY);
            Debug.Log(Math.Abs(pos.x) <= maxX && Math.Abs(pos.y) <= maxY);
        }

        return Math.Abs(pos.x) <= maxX && Math.Abs(pos.y) <= maxY;
    }

    /// <summary>
    ///     show or hide the rings of targets depending on the given difficulty of the memory test
    /// </summary>
    public void Memory_ShowTargetsByDifficulty()
    {
        //show everything initially
        m_targets_memory.ForEach(t => t.gameObject.SetActive(true));

        var maxIndex = -100;
        //then hide everything we don't need
        switch (Test_Memory.Instance.m_options.m_option_patternDifficulty.value)
        {
            case TestOption_Difficulty.Easy:
                maxIndex = 9;
                break;
            case TestOption_Difficulty.Medium:
                maxIndex = 17;
                break;
            case TestOption_Difficulty.Hard:
                maxIndex = 33;
                break;
        }

        foreach (var t in m_targets_memory)
            if (t.m_index >= maxIndex)
                t.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Activate a random target on the memory targets
    /// </summary>
    /// <param name="difficulty">The difficulty of this memory test, controls which rings can be activated</param>
    /// <param name="lastActiveIndex">
    ///     the index of the last activeted target to prevent the same target
    ///     from being activated twice in a row
    /// </param>
    /// <returns></returns>
    public int Memory_ActivateRandomTarget(TestOption_Difficulty difficulty, int lastActiveIndex)
    {
        ClearTargets(TestType.Sequence);
        var randomIndex = -1000;
        do
        {
            var indexMax = 0;
            switch (difficulty)
            {
                case TestOption_Difficulty.Easy:
                    indexMax = 9;
                    break;
                case TestOption_Difficulty.Medium:
                    indexMax = 17;
                    break;
                case TestOption_Difficulty.Hard:
                    indexMax = 33;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("difficulty", difficulty, null);
            }

            //get a random index in the given range
            randomIndex = Random.Range(0, indexMax);
        } while (randomIndex == lastActiveIndex);

        var target = m_targets_memory.Find(t => t.m_index == randomIndex);
        if (target == null)
        {
            Debug.Log("Error getting target in Memory_ActivateRandomTarget()");
            return -1000;
        }

        target.Activate();
        return target.m_index;
    }

    /// <summary>
    ///     Get the index of a target that is different than the lastActiveIndex
    /// </summary>
    public int Memory_GetRandomIndex(TestOption_Difficulty difficulty, int lastActiveIndex)
    {
        var randomIndex = -1000;
        do
        {
            var indexMax = 0;
            switch (difficulty)
            {
                case TestOption_Difficulty.Easy:
                    indexMax = 9;
                    break;
                case TestOption_Difficulty.Medium:
                    indexMax = 17;
                    break;
                case TestOption_Difficulty.Hard:
                    indexMax = 33;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("difficulty", difficulty, null);
            }

            //get a random index in the given range
            randomIndex = Random.Range(0, indexMax);
        } while (randomIndex == lastActiveIndex);

        return randomIndex;
    }

    /// <summary>
    ///     Activate the target with the given index, this will also clear all other memory targets
    /// </summary>
    public void Memory_ActivateTargetByIndex(int index)
    {
        ClearTargets(TestType.Sequence);

        m_targets_memory.Find(t => t.m_index == index).Activate();
    }

    /// <summary>
    ///     Show the given set of targets for the reaction test
    /// </summary>
    public void Reaction_ShowTargetSet(TestOption_OrientationDir orientation)
    {
        //hide them all initially
        m_targets_reaction.ForEach(t => t.gameObject.SetActive(false));

        //the indices for the reaction targets are E|S|W|N - 0|1|2|3

        //then show the ones we want
        if (orientation == TestOption_OrientationDir.LtoR || orientation == TestOption_OrientationDir.RtoL)
        {
            m_targets_reaction[0].gameObject.SetActive(true);
            m_targets_reaction[2].gameObject.SetActive(true);
        }
        else
        {
            m_targets_reaction[1].gameObject.SetActive(true);
            m_targets_reaction[3].gameObject.SetActive(true);
        }
    }

    /// <summary>
    ///     get the target object in the given orientation
    /// </summary>
    public Target Reaction_GetTarget(Direction dir)
    {
        switch (dir)
        {
            case Direction.N:
                return m_targets_reaction[1];
            case Direction.S:
                return m_targets_reaction[3];
            case Direction.E:
                return m_targets_reaction[0];
            case Direction.W:
                return m_targets_reaction[2];
            default:
                return null;
        }
    }

    /// <summary>
    ///     move the hover target to a random place on the screen, with respect to the usable space
    /// </summary>
    /// <param name="minDistance">optionally, force a minimum distance the new target will be from the old one</param>
    public void Balance_MoveTarget(float minDistance = 0)
    {
        var targetOk = false;
        var newPos = Vector2.zero;

        const float extraEdgePadding = 65f;
        var bpTargetRectT = transform.Find("Target_BpHover").GetComponent<RectTransform>();
        var targetRadius = bpTargetRectT.sizeDelta.x / 2;
        var safeSize = targetRadius + extraEdgePadding;
        while (!targetOk)
        {
            //get a new random screen position based on the usage screen, and the current size of the target
            var posX = Random.Range(
                BalancePlate.Instance.m_usableScreen_x / -2 + safeSize,
                BalancePlate.Instance.m_usableScreen_x / 2 - safeSize);
            var posY = Random.Range(
                BalancePlate.Instance.m_usableScreen_y / -2 + safeSize,
                BalancePlate.Instance.m_usableScreen_y / 2 - safeSize);

            newPos = new Vector2(posX, posY);

            if (Vector2.Distance(bpTargetRectT.anchoredPosition, newPos) > minDistance)
                targetOk = true;
        }

        transform.Find("Target_BpHover").GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    public void Balance_SetTargetPosition(Vector2 pos)
    {
        transform.Find("Target_BpHover").GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void Balance_SetTargetSize(float newSize)
    {
        m_bpHoverTarget.ChangeSize(newSize);
    }

    public void Balance_SetTargetInMotion(float bearing, float speed)
    {
        m_bpHoverTarget.StartMoving(speed, bearing);
    }

    /// <summary>
    ///     this will activate a random target in the go/nogo grid with a green/red target depending on how the
    ///     'go' bool is set, will return the index of the target we activated, this is functionally identical to the speed
    ///     activateRandomTarget, but we set a special target graphic here...
    /// </summary>
    public int GoNoGo_ActivateRandomTarget(bool go, int lastActiveTarget = -1)
    {
        //first clear all of our existing targets
        ClearTargets(TestType.GoNoGo);

        //loop and find an index that doesn't equal the last index we used when returning
        //a target, this could theoretically loop infinitely...but we're probably fine
        var randomIndex = -100;
        do
        {
            //first pick a ring
            var r = Random.Range(0, 3);
            switch (r)
            {
                case 0: //inner
                    randomIndex = Random.Range(0, 9);
                    break;
                case 1: //middle
                    randomIndex = Random.Range(9, 17);
                    break;
                case 2: //outer
                    randomIndex = Random.Range(17, 33);
                    break;
                default:
                    Debug.Log("Error in ActivateRandomTarget_Speed()");
                    break;
            }
        } while (randomIndex == lastActiveTarget);

        //now that we have a valid index, activate the target
        var target = m_targets_goNoGo.Find(t => t.m_index == randomIndex);
        if (target == null)
        {
            Debug.Log("Error assign active index in ActivateRandomTarget_Speed()");
            return -100;
        }

        target.Activate_SpecialSprite(
            go ? Test_GoNoGo.Instance.image_target_go : Test_GoNoGo.Instance.image_target_noGo);

        return randomIndex;
    }

    /// <summary>
    ///     Activate X unique targets on the flash target grid
    /// </summary>
    public List<int> Flash_Activate_X_RandomTargets(int targetsToActivate)
    {
        //first, we make a new list consisting of all the available indeces from the main target list
        var availableIndeces = new List<int>();
        for (var x = 0; x < m_targets_flash.Count; x++)
            availableIndeces.Add(x);

        //now make a new list with the indeces we will choose to activate
        var indecesToUse = new List<int>(targetsToActivate);

        for (var x = 0; x < targetsToActivate; x++)
        {
            //get a random index from the size of the availabel index list
            var randomIndex = Random.Range(0, availableIndeces.Count);

            //add this to our indeces we're going to activate
            indecesToUse.Add(availableIndeces[randomIndex]);

            //then remove this from the available ones
            availableIndeces.RemoveAt(randomIndex);
        }

        //now activate the selected targets
        foreach (var x in indecesToUse)
            m_targets_flash.Find(t => t.m_index == x).Activate();

        return indecesToUse;
    }

    /// <summary>
    ///     Activate the correct targets based on the given List-int and show the target with the given incorrect Index
    ///     as red.  This should be used when a user pushes a wrong target in the flash sequence
    /// </summary>
    public void Flash_ShowSolution(List<int> correctIndeces, int incorrectIndex)
    {
        foreach (var x in correctIndeces)
            m_targets_flash[x].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_go);

        m_targets_flash[incorrectIndex].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_noGo);
    }

    /// <summary>
    ///     Activate a rhythm target on eithe rthe left or right side of the screen according to the given bool
    /// </summary>
    public void Rhythm_ActivateTarget(bool leftSide)
    {
        //todo, this could probably be done once at a better place..
        //get our usable screen  
        var screenX = GetComponent<RectTransform>().rect.width;
        var halfScreenX = screenX / 2;
        var halfScreenY = GetComponent<RectTransform>().rect.height / 2;
        Vector2 pos;
        //get a random location with respect to our offsets
        do //todo theoretical infinite loop..
        {
            var x = Random.Range(ConstantDefinitions.Instance.rhythm_offset_center,
                halfScreenX - ConstantDefinitions.Instance.rhythm_offset_x);
            var y = Random.Range(-halfScreenY + ConstantDefinitions.Instance.rhythm_offset_y,
                halfScreenY - ConstantDefinitions.Instance.rhythm_offset_y);

            if (leftSide) x *= -1;
            pos = new Vector2(x, y);
        } while (Position_ObstructedByUi(pos,
            m_target_rhythm.rectTransform.sizeDelta.x * m_target_rhythm.transform.localScale.x / 2));

        m_target_rhythm.rectTransform.anchoredPosition = pos;
        m_target_rhythm.Activate();
    }

    /// <summary>
    ///     Change the visibility of the multi target sets
    /// </summary>
    /// <param name="sets">Number of sets to show</param>
    public void Multi_ChangeTargetSets(int sets)
    {
        //hide everything intially
        m_trans_target_multi[0].gameObject.SetActive(false);
        m_trans_target_multi[1].gameObject.SetActive(false);
        m_trans_target_multi[2].gameObject.SetActive(false);
        m_trans_target_multi[3].gameObject.SetActive(false);
        switch (sets)
        {
            case 1:
                m_trans_target_multi[0].gameObject.SetActive(true);
                m_trans_target_multi[0].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_1Set_origin;
                break;
            case 2:
                m_trans_target_multi[0].gameObject.SetActive(true);
                m_trans_target_multi[1].gameObject.SetActive(true);
                m_trans_target_multi[0].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_2Sets_origins[0];
                m_trans_target_multi[1].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_2Sets_origins[1];
                break;
            case 3:
                m_trans_target_multi[0].gameObject.SetActive(true);
                m_trans_target_multi[1].gameObject.SetActive(true);
                m_trans_target_multi[2].gameObject.SetActive(true);
                m_trans_target_multi[0].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_3Sets_origins[0];
                m_trans_target_multi[1].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_3Sets_origins[1];
                m_trans_target_multi[2].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_3Sets_origins[2];
                break;
            case 4:
                m_trans_target_multi[0].gameObject.SetActive(true);
                m_trans_target_multi[1].gameObject.SetActive(true);
                m_trans_target_multi[2].gameObject.SetActive(true);
                m_trans_target_multi[3].gameObject.SetActive(true);
                m_trans_target_multi[0].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_4Sets_origins[0];
                m_trans_target_multi[1].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_4Sets_origins[1];
                m_trans_target_multi[2].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_4Sets_origins[2];
                m_trans_target_multi[3].GetComponent<RectTransform>().anchoredPosition =
                    ConstantDefinitions.Instance.multi_4Sets_origins[3];
                break;
        }
    }

    /// <summary>
    ///     Change the color or each target in the set to indicate if they were go or no go, this basically shows the 'answer'
    ///     for the set
    /// </summary>
    public void Multi_ActivateTargetsFromSet(int set)
    {
        switch (set)
        {
            case 0:
                m_targets_multi[0].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_go);
                m_targets_multi[1].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_noGo);
                break;
            case 1:
                m_targets_multi[2].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_go);
                m_targets_multi[3].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_noGo);
                break;
            case 2:
                m_targets_multi[4].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_go);
                m_targets_multi[5].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_noGo);
                break;
            case 3:
                m_targets_multi[6].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_go);
                m_targets_multi[7].Activate_SpecialSprite(Test_GoNoGo.Instance.image_target_noGo);
                break;
        }
    }

    /// <summary>
    ///     this will change the targets back to white.  This should be done before we indicate any targets with the arrows..
    /// </summary>
    public void Multi_VisuallyResetAllTargets()
    {
        foreach (var t in m_targets_multi)
            t.Deactivate();
    }

    /// <summary>
    ///     moves the contrast target to a random position, we return the position given to then be sent as last position.
    ///     This was originally intended to allow the user to set a magnitude limit of one target to another so they
    ///     didn't appeat too close together, but never got implemented
    /// </summary>
    public Vector2 Contrast_MoveTargetToRandomPosition(Vector2 lastPosition)
    {
        if (Test_Contrast.Instance.m_options.m_option_useFocusedZone.value)
        {
            Contrast_MoveTarget_FocusedZone();
            return Vector2.zero;
        }

        float x, y;
        do //todo theoretical infinite loop..
        {
            //x
            do
            {
                x = Random.Range(0, m_screenBounds.x / 2);
                x *= Random.Range(0, 2) == 0 ? -1 : 1; //coin flip
            } while (!PositionWithinEdgePadding(x, m_screenBounds.x / 2));

            //y
            do
            {
                y = Random.Range(0, m_screenBounds.y / 2);
                y *= Random.Range(0, 2) == 0 ? -1 : 1; //coin flip
            } while (!PositionWithinEdgePadding(y, m_screenBounds.y / 2));

            //getting here means we have both valid x/y as far as edge padding, now we have one last check
            //to make sure they aren't obstructed by anything in the UI
        } while (Position_ObstructedByUi(new Vector2(x, y),
            m_target_contrast.rectTransform.sizeDelta.x * m_target_contrast.rectTransform.localScale.x / 2));

        //todo, we would check the magnitude from the last position here and loop again if it is too close..

        var pos = new Vector2(x, y);
        m_target_contrast.rectTransform.anchoredPosition = pos;

        return pos;
    }

    public void Contrast_MoveTarget_FocusedZone()
    {
        var radius = Test_Contrast.Instance.m_radius_focusZone;
        var origin = Test_Contrast.Instance.m_rectTransform_focusZone.anchoredPosition;
        var distribution = Test_Contrast.Instance.m_options.m_option_focusZoneDistribution.value;
        bool inCircle;

        //distribution is the chance that the target should appear inside the target
        //if the distribution is a absolute, no need to roll
        if (distribution <= 0)
        {
            inCircle = false;
        }
        else if (distribution >= 1)
        {
            inCircle = true;
        }
        else
        {
            var chance = Random.Range(0, 1f);
            inCircle = chance <= distribution;
        }

        var targetOk = false;
        while (targetOk == false)
        {
            //pick a completely random position within the bounds of the screen
            var xPos = Random.Range(-Screen.width / 2, Screen.width / 2);
            var yPos = Random.Range(-Screen.height / 2, Screen.height / 2);
            var newPosition = new Vector2(xPos, yPos);

            //make sure this is within edge padding and not obscured by the ui
            if (!PositionWithinEdgePadding(xPos, Screen.width / 2) ||
                !PositionWithinEdgePadding(yPos, Screen.height / 2) ||
                Position_ObstructedByUi(newPosition,
                    m_target_contrast.rectTransform.sizeDelta.x * m_target_contrast.rectTransform.localScale.x /
                    2)) continue;

            //now make sure it is within with circle, or not depending on our options
            if (inCircle)
            {
                if (Vector2.Distance(origin, newPosition) > radius)
                    continue;
            }
            else if (Vector2.Distance(origin, newPosition) < radius)
            {
                continue;
            }

            //this is a good position, so move the target
            m_target_contrast.rectTransform.anchoredPosition = newPosition;
            break;
        }
    }

    /// <summary>
    ///     Reset the contrast target to its initial state
    /// </summary>
    public void Contrast_ResetTarget()
    {
        m_target_contrast.rectTransform.anchoredPosition = Vector2.zero;
        m_target_contrast.m_image_targetSprite.color = Color.white;
    }
}