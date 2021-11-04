using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum TestType
{
    Speed,
    Peripheral,
    Sequence,
    Reaction,
    Balance,
    GoNoGo,
    Flash,
    Rhythm,
    Contrast,
    Multi,
    All,
    None
}

public enum InputMode
{
    Touch,
    Balance,
    Ehb
}

public enum GlobalTestOption
{
    TouchSensitivity_Size,
    TouchSensitivity_KeepVisible,
    TargetLayout_GridSize,
    TargetLayout_TargetSize,
    Audio,
    Countdown,
    RecordData
}

public enum TestOption_Pacing
{
    Self,
    Auto,
    Auto_Audio,
    Random,
    None
}

public enum TestOption_Difficulty
{
    Easy,
    Medium,
    Hard
}

public enum TestOption_OrientationDir
{
    RtoL,
    LtoR,
    TtoB,
    BtoT
}

public enum TargetLocation
{
    Inner,
    Middle,
    Outer,
    Peripheral,
    Reation,
    Rhythm,
    Multi1,
    Multi2,
    Multi3,
    Multi4,
    Contrast,
    None
}

public enum MemoryMode
{
    Playback,
    Input,
    ShowingSolution
}

public enum FlashMode
{
    Display,
    Input,
    Cooldown,
    Stopped,
    ShowingSolution
}

public enum MultiMode
{
    Showing,
    Spinning,
    Waiting,
    Cooldown
}

public enum Direction
{
    N,
    S,
    E,
    W,
    F,
    R,
    None
}

public enum TargetTrackingMode
{
    Static,
    Dynamic
}

public enum TargetTrackingDirection
{
    Horizontal,
    Vertical,
    Diagonal,
    Random
}

public class ConstantDefinitions : SerializedMonoBehaviour
{
    public static ConstantDefinitions Instance;

    /// <summary>
    ///     "When the countdown is set to 0, this is the amount of time that will pass, but not be displayed
    ///     "so that the test doesn't start too abruptly."
    /// </summary>
    [HideInInspector] public float countdown_invisibleTime = 1;

    [HideInInspector] public float flash_solutionShowTime = 4;

    public List<AudioClip> m_list_feedbackClips;


    [HideInInspector] public float memory_showSolutionDelay = 1;

    [HideInInspector] public Vector2 multi_1Set_origin = new Vector2(0, 0);
    [HideInInspector] public Vector2[] multi_2Sets_origins = {new Vector2(-450, 0), new Vector2(450, 0)};

    [HideInInspector] public Vector2[] multi_3Sets_origins =
        {new Vector2(-400, 150), new Vector2(400, 150), new Vector2(0, 0)};

    [HideInInspector] public Vector2[] multi_4Sets_origins =
        {new Vector2(-400, 0), new Vector2(400, 0), new Vector2(0, -200), new Vector2(0, 200)};

    [HideInInspector] public float multi_cooldownTime = 1.5f;

    [HideInInspector] public float multi_showtime = 2;

    [HideInInspector] public Vector2 peri_bounds_infoDisplay = new Vector2(-730, -400);

    /// <summary>
    ///     The positional limits of the star/stop button and info boxes.  These bounds
    ///     define where the peripheral target can not spawn, so it doesn't get hidden behind something..
    /// </summary>
    [HideInInspector] public Vector2 peri_bounds_stop = new Vector2(700, -430);


    /// <summary>
    ///     "Determines how close to the edge of the screen that the random target can spawn, as a percentage of the total
    ///     width/height of the screen"
    /// </summary>
    [HideInInspector] public float peri_edgePadding = .1f;


    [HideInInspector] public float rhythm_offset_center = 50;
    [HideInInspector] public float rhythm_offset_x = 50;
    [HideInInspector] public float rhythm_offset_y = 50;

    private void Awake()
    {
        Instance = this;
    }
}