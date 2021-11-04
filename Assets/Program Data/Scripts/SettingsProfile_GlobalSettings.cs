using System;
using UnityEngine;

[Serializable]
public class SettingsProfile_GlobalSettings
{
    [HideInInspector] public int m_last_touchSensitivity,
        m_last_targetLayout_size,
        m_last_targetLayout_targetSize,
        m_last_countdown;

    [HideInInspector] public bool m_last_touchSensitivity_keepVisible, m_last_recordData, m_last_audio_playOnHit;

    public int m_touchSensitivity,
        m_targetLayout_size,
        m_targetLayout_targetSize,
        m_countdown;

    public bool m_touchSensitivity_keepVisible, recordData, m_audio_playOnHit;

    public void Initialize()
    {
        m_last_countdown = m_countdown;
        m_last_recordData = recordData;
        m_last_targetLayout_size = m_targetLayout_size;
        m_last_targetLayout_targetSize = m_targetLayout_targetSize;
        m_last_touchSensitivity = m_touchSensitivity;
        m_last_touchSensitivity_keepVisible = m_touchSensitivity_keepVisible;
        m_last_audio_playOnHit = m_audio_playOnHit;
    }

    public void SetLastOption(GlobalTestOption option, int index)
    {
        switch (option)
        {
            case GlobalTestOption.TouchSensitivity_Size:
                m_last_touchSensitivity = index;
                break;
            case GlobalTestOption.TargetLayout_GridSize:
                m_last_targetLayout_size = index;
                break;
            case GlobalTestOption.TargetLayout_TargetSize:
                m_last_targetLayout_targetSize = index;
                break;
            case GlobalTestOption.Countdown:
                m_last_countdown = index;
                break;
            default:
                throw new ArgumentOutOfRangeException("option", option, null);
        }
    }

    public void SetLastOption(GlobalTestOption option, bool val)
    {
        switch (option)
        {
            case GlobalTestOption.TouchSensitivity_KeepVisible:
                m_last_touchSensitivity_keepVisible = val;
                break;
            case GlobalTestOption.RecordData:
                m_last_recordData = val;
                break;
            case GlobalTestOption.Audio:
                m_last_audio_playOnHit = val;
                break;
            default:
                throw new ArgumentOutOfRangeException("option", option, null);
        }
    }
}