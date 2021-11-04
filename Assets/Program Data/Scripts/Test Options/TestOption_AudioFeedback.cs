using TMPro;
using UnityEngine.UI;

public class TestOption_AudioFeedback : TestOption
{
    public Button m_button_next, m_button_previous, m_button_sound;
    public Toggle m_toggle;

    public override void Initialize(Test test)
    {
        base.Initialize(test);

        m_toggle.onValueChanged.AddListener(delegate { TogglePushed(m_toggle); });
        m_button_next.onClick.AddListener(delegate { CycleSounds(true); });
        m_button_previous.onClick.AddListener(delegate { CycleSounds(false); });
        m_button_sound.onClick.AddListener(ButtonPushed_Sound);
    }

    private void CycleSounds(bool next)
    {
        var nextIndex =
            ConstantDefinitions.Instance.m_list_feedbackClips.IndexOf(m_test.m_options.m_option_audioClip.value);
        nextIndex += next ? 1 : -1;
        if (nextIndex >= ConstantDefinitions.Instance.m_list_feedbackClips.Count) nextIndex = 0;
        else if (nextIndex < 0) nextIndex = ConstantDefinitions.Instance.m_list_feedbackClips.Count - 1;

        m_test.m_options.m_option_audioClip.Change(ConstantDefinitions.Instance.m_list_feedbackClips[nextIndex]);
        m_button_sound.GetComponentInChildren<TextMeshProUGUI>().text = m_test.m_options.m_option_audioClip.value.name;

        PlaySelectedSound();
    }

    private void ButtonPushed_Sound()
    {
        PlaySelectedSound();
    }

    private void TogglePushed(Toggle t)
    {
        m_test.m_options.m_option_audioOnhit.Change(t.isOn);
    }

    protected override void TestSelected()
    {
        m_toggle.isOn = m_test.m_options.m_option_audioOnhit.value;
        m_button_sound.GetComponentInChildren<TextMeshProUGUI>().text = m_test.m_options.m_option_audioClip.value.name;
    }

    private void PlaySelectedSound()
    {
        Manager_Audio.PlaySound(m_test.m_options.m_option_audioClip.value);
    }
}