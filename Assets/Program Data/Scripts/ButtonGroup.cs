using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     A button group will toggle between a series of buttons, highlighting the currently selected one
/// </summary>
public class ButtonGroup : MonoBehaviour
{
    public delegate void ButtonGroupEvent(int buttonIndex);

    public int defaultSelected;

    /// <summary>
    ///     this event fires whenever a button in a button group is pushed, this will pass the index of the button in the group
    ///     as a parameter
    /// </summary>
    public ButtonGroupEvent event_buttonPushed;

    public Color m_color_active, m_color_inactive;
    public List<Button> m_list_buttons;

    private void Awake()
    {
        //populate the button list, we can search for buttons only if the list is null on Start()
        if (m_list_buttons == null || m_list_buttons.Count <= 0)
        {
            m_list_buttons = new List<Button>();
            GetComponentsInChildren<Button>().ToList().ForEach(b => m_list_buttons.Add(b));
        }

        if (m_list_buttons == null || m_list_buttons.Count <= 0)
        {
            Debug.LogWarning("ButtonGroup has null/zero entries in list..");
            return;
        }

        //hook the click events
        m_list_buttons.ForEach(b => b.onClick.AddListener(delegate { ButtonPushed(b); }));
    }

    private void ButtonPushed(Button button)
    {
        //initially, make all of the buttons the inactive color
        m_list_buttons.ForEach(b => b.GetComponent<Image>().color = m_color_inactive);

        //then set the one we just clicked to the active color
        button.GetComponent<Image>().color = m_color_active;

        if (event_buttonPushed == null)
            Debug.Log("ButtonGroupEvent is null for button: " + button.name);
        else event_buttonPushed(m_list_buttons.IndexOf(button));
    }

    public void SetNames(string[] names)
    {
        for (var x = 0; x < names.Length; x++)
            m_list_buttons[x].GetComponentInChildren<TextMeshProUGUI>().text = names[x];
    }

    public void SetNames(int[] names)
    {
        var newNames = new string[names.Length];
        for (var x = 0; x < names.Length; x++)
            newNames[x] = names[x].ToString();

        SetNames(newNames);
    }

    public void SetNames(float[] names)
    {
        var newNames = new string[names.Length];
        for (var x = 0; x < names.Length; x++)
            newNames[x] = names[x].ToString();

        SetNames(newNames);
    }

    public string GetName(int buttonIndex)
    {
        return m_list_buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public void InvokeDefault()
    {
        m_list_buttons[defaultSelected].onClick.Invoke();
    }
}