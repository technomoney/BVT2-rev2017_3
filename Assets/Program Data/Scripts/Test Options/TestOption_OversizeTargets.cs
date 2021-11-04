using UnityEngine.UI;

public class TestOption_OversizeTargets : TestOption
{
    public Toggle m_toggle_useOversize;

    public override void Initialize(Test test)
    {
        base.Initialize(test);
        m_toggle_useOversize.onValueChanged.AddListener(delegate { ToggleChanged(m_toggle_useOversize); });
    }

    private void ToggleChanged(Toggle t)
    {
        m_test.m_options.m_option_useOversizeTargets.Change(t.isOn);
        Manager_Test.Instance.m_selectedTest.HandleTargetScaling();
    }

    protected override void TestSelected()
    {
        base.TestSelected();
        m_toggle_useOversize.isOn = m_test.m_options.m_option_useOversizeTargets.value;
        Manager_Test.Instance.m_selectedTest.HandleTargetScaling();
    }
}