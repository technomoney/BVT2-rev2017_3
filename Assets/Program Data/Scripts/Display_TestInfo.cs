using TMPro;
using UnityEngine;

public class Display_TestInfo : MonoBehaviour
{
    public static Display_TestInfo Instance;
    public TextMeshProUGUI m_text_label;

    // Use this for initialization
    private void Start()
    {
        Instance = this;

        m_text_label.text = "";
        //this should initially be hidden
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Manager_Test.Instance.m_mode == TestMode.Idle) return;

        //update the timer
        if (Manager_Test.Instance.m_mode == TestMode.Running)
            UpdateLabel();
    }

    /// <summary>
    ///     Should only be called by the test manager when a test starts
    /// </summary>
    public void Show()
    {
        //we don't need to do anything if the active test is reaction
        if (Manager_Test.Instance.m_selectedTestType == TestType.Reaction) return;


        gameObject.SetActive(true);
        //we can manually update the label here so the correct info for the given test will show during a countdown
        UpdateLabel();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateLabel()
    {
        m_text_label.text = Manager_Test.Instance.m_selectedTest.GetInfoString();
    }

    public void Clear()
    {
        m_text_label.text = "";
    }
}