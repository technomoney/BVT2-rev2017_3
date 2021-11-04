using UnityEngine.UI;

public class TestOption_RecordData : TestOption
{
    public Toggle m_toggle_recordData;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_toggle_recordData.onValueChanged.AddListener(delegate { TogglePushed(m_toggle_recordData); });
    }

    private void TogglePushed(Toggle t)
    {
        //m_test.m_options.m_option_recordBpData.Change(t.isOn);
    }

    protected override void TestSelected()
    {
        base.TestSelected();

        //m_toggle_recordData.isOn = m_test.m_options.m_option_recordBpData.value;
    }
}