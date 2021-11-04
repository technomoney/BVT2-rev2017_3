using UnityEngine;

/// <summary>
///     This is a middle man for the target class, we have to do it this way because we want the collider on the
///     sensitivity
///     ring, not the primary target object, and the trigger events won't be detected there.  So when we get the normal
///     unity
///     event here we can just call the event on the parent target
/// </summary>
public class Target_BPCursorHandler : MonoBehaviour
{
    private Target m_parentTarget;

    public void Initialize(Target t)
    {
        m_parentTarget = t;
        if (m_parentTarget == null)
            Debug.Log("Problem assigning parent target in Target_BPCursorHandler");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_parentTarget.event_triggerEnter != null)
            m_parentTarget.event_triggerEnter(m_parentTarget);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (m_parentTarget.event_triggerExit != null)
            m_parentTarget.event_triggerExit(m_parentTarget);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (m_parentTarget.event_triggerStay != null)
            m_parentTarget.event_triggerStay(m_parentTarget);
    }
}