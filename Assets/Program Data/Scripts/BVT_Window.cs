using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
///     generic base class for any window that shows/hides
/// </summary>
public class BVT_Window : SerializedMonoBehaviour
{
    private Vector2 m_hidePosition, m_showPosition;
    [HideInEditorMode] public bool m_isShowing;
    private RectTransform m_rectTransform;

    public virtual void Start()
    {
        m_isShowing = false;

        //unless otherwise overridden, the show position should always be 0,0
        m_showPosition = Vector2.zero;
        m_rectTransform = GetComponent<RectTransform>();

        if (m_rectTransform == null)
        {
            Debug.LogError("RectTransform is null for BVT Window...");
            return;
        }

        m_hidePosition = m_rectTransform.anchoredPosition;
    }

    public virtual void Show()
    {
        m_isShowing = true;
        m_rectTransform.anchoredPosition = m_showPosition;
    }

    public virtual void Hide()
    {
        m_isShowing = false;
        m_rectTransform.anchoredPosition = m_hidePosition;
    }

    public virtual void ToggleVisibility()
    {
        m_isShowing = !m_isShowing;
        if (m_isShowing) Show();
        else Hide();
    }

    public RectTransform GetRect()
    {
        return m_rectTransform;
    }
}