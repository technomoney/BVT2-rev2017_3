using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Target : SerializedMonoBehaviour
{
    public delegate void TargetEvent(Target t);

    public TargetEvent
        event_targetDown,
        event_targetUp,
        event_triggerEnter,
        event_triggerExit,
        event_triggerStay;

    public Target_BPCursorHandler m_bpCursorHandler;
    private Color m_color_sensitivityVisible, m_color_sensitivityHidden;
    public Sprite m_defaultImage;
    public EventTrigger m_eventTrigger;

    public Image m_image_targetSprite, m_image_sensitivityRing;
    public int m_index;

    [PropertyTooltip("Show the debug data for target up/down/click")]
    public bool m_showDebugData;

    public TargetLocation m_targetLocation;
    private EventTrigger.Entry m_trigger_down, m_trigger_up;
    public RectTransform rectTransform;

    /// <summary>
    ///     has this target been activated by the target manager and showing the target graphic?
    /// </summary>
    public bool m_isActive { get; private set; }

    public void Initialize(int index, TargetLocation location)
    {
        //since we need to hook to target up/down/clicked we have to use an event trigger, can't use a basic button
        //we have to use a custom event trigger and handle each event we want to subscribe to manually
        //m_trigger_clicked = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
        m_trigger_up = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};
        m_trigger_down = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};

        //m_trigger_clicked.callback.AddListener(TargetClicked);
        m_trigger_up.callback.AddListener(TargetUp);
        m_trigger_down.callback.AddListener(TargetDown);

        //m_eventTrigger.triggers.Add(m_trigger_clicked);
        m_eventTrigger.triggers.Add(m_trigger_up);
        m_eventTrigger.triggers.Add(m_trigger_down);

        m_targetLocation = location;
        rectTransform = GetComponent<RectTransform>();
        m_index = index;
        m_isActive = false;

        m_color_sensitivityVisible = m_image_sensitivityRing.color;
        m_color_sensitivityHidden = m_color_sensitivityVisible;
        m_color_sensitivityHidden.a = 0;

        //this is set in the inspector, but can be null if on anything other than a standard target (contrast target)
        //so unless we need to use the bp in contrast, this is fine..
        if (m_bpCursorHandler != null) m_bpCursorHandler.Initialize(this);
    }

    public void ChangeOption_TargetScale(float newScale)
    {
        rectTransform.localScale = new Vector2(newScale, newScale);
    }

    public void ChangeOption_TouchSensitivity(float newScale)
    {
        m_image_sensitivityRing.rectTransform.localScale = new Vector3(newScale, newScale);
    }

    public void TargetDown(BaseEventData d)
    {
        //this function can only ever be called by a touch (or mouse) input, so doing this check here is 'generally' 
        //a safe bet, but could have implications at some point...
        //the 'hit' for a BP hover is handled in Test().base so we know any hits originating from there must be from 
        //balance, and any from here must be from touch
        if (Manager_Test.Instance.m_testInputMode == InputMode.Balance) return;

        if (m_showDebugData) Debug.Log(name + " down");

        if (!RaycastToSensitivityRing()) return;

        if (event_targetDown == null)
        {
            if (m_showDebugData) Debug.Log("event_TargetDown is null for target: " + name);
            return;
        }

        if (m_isActive && Manager_Test.Instance.m_selectedTest.m_options.m_option_audioOnhit.value)
            Manager_Audio.PlaySound(Manager_Test.Instance.m_selectedTest.m_options.m_option_audioClip.value);

        //set the flag for checking the streak
        if (m_isActive)
            //Debug.Log("Active target hit this frame");
            Manager_Test.Instance.m_selectedTest.m_activeTargetHitThisFrame = true;

        event_targetDown(this);
    }

    private bool RaycastToSensitivityRing()
    {
        var hits =
            Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hits.Length > 0)
            foreach (var h in hits)
            {
                if (!h.collider.gameObject.name.Equals("Image_SensitivityRing"))
                    continue;
                return true;
            }

        return false;
    }

    private void TargetUp(BaseEventData d)
    {
        if (m_showDebugData) Debug.Log(name + " up");
        if (event_targetUp == null)
        {
            if (m_showDebugData) Debug.Log("event_TargetUp is null for target: " + name);
            return;
        }

        event_targetUp(this);
    }

    /// <summary>
    ///     activate this target and change the graphic to that of the chosen global target graphic
    /// </summary>
    public void Activate()
    {
        m_isActive = true;
        m_image_targetSprite.sprite = Manager_Targets.Instance.currentTargetGraphic;
        m_image_targetSprite.color = Manager_Targets.Instance.m_currentTarget_color;
        //have to change the sibling order to account for targets overlapping with large sensitivity
        //last sibling is top when using the UI canvas
        transform.SetAsLastSibling();
    }

    /// <summary>
    ///     Activate this target normally, but then set the target sprite to the given sprite
    /// </summary>
    public void Activate_SpecialSprite(Sprite targetSprite)
    {
        Activate();
        m_image_targetSprite.sprite = targetSprite;
    }

    /// <summary>
    ///     sets isactive=false, and changes the target back to the default sprite
    /// </summary>
    public void Deactivate(bool resetSpriteToDefault = true)
    {
        m_isActive = false;
        if (resetSpriteToDefault)
            m_image_targetSprite.sprite = m_defaultImage;
    }

    /// <summary>
    ///     show or hide the sensitivity ring
    /// </summary>
    public void ShowSensitivityRing(bool show)
    {
        m_image_sensitivityRing.color = show ? m_color_sensitivityVisible : m_color_sensitivityHidden;
    }
}