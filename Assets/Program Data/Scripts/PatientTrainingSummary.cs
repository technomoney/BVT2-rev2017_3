using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     this is a generic container to hold all test data for a training that can be used to create the training summary
///     and stored
///     per patient
/// </summary>
[Serializable]
public class PatientTrainingSummary
{
    public float m_autopaceInterval;
    public string m_background, m_target, m_sensitivity, m_gridSize, m_targetSize, m_audio, m_countdown;
    public float m_balance_dynamic_totalTimeOverTarget;
    public float m_balance_hoverTime;
    public float m_balance_percentageOverTarget;
    public int m_balance_targetDeviations;
    public string m_balance_targetSize, m_balance_targetSpacing, m_balance_progressDecay;
    public TargetTrackingDirection m_balance_trackingDirection;
    public TargetTrackingMode m_balance_trackingMode;
    public float m_balance_trackingSpeed;
    public TestOption_Difficulty m_contrast_blendSpeed;
    public float m_contrast_lowestAchieved;
    public string m_contrast_oversizeTargets;
    public float m_contrast_paceTime;
    public string m_dataLocation;
    public DateTime m_dateTime;
    public float m_decayTime;
    public float m_fastestHit, m_slowestHit, m_averageHit, m_responseTime, m_reactionTime, m_testDuration;
    public string m_fileName;
    public float m_flash_displayTime;
    public int m_flash_startingLevel;
    public float m_goNogo_displayTime;
    public string m_goNogo_frequency;

    public int m_highestlevel,
        m_startingLevel,
        m_hitsOnHighestLevel,
        m_levelsCompleted,
        m_faults,
        m_presented_go,
        m_presented_nogo,
        m_numberOfTrials,
        m_targetSets;

    public string m_inputMode;

    public List<float> m_list_leveltimes,
        m_list_levelTimes,
        m_list_contrastSteps,
        m_list_touchTimes,
        m_hitTimes,
        m_hitTimes_left,
        m_hitTimes_right;

    public string m_multi_rotationSpeed;
    public float m_multi_rotationTime;
    public float m_multi_score;
    public int m_multi_trials, m_multi_targetSets;
    public string m_operatorAccountName;
    public string m_paceMode;
    public int m_presented_left, m_presented_right;
    public float m_reaciton_targetSpacing;
    public bool m_reaction_autoPace;
    public TestOption_OrientationDir m_reaction_orientation;
    public Vector2[] m_reaction_trialTimes;
    public TestOption_Difficulty m_sequence_difficulty;
    public float m_sequence_displayTime;
    public string m_shortScore, m_score_firstHalf, m_score_secondHalf, m_scoreFatigue;

    public string m_targetArea;

    //all fields for training info
    public string m_testName;
    public TestType m_testType;
    public int m_totalHits, m_totalMisses, m_misses_left, m_misses_right, m_streak;
    public bool m_touchSensitivity_keepVisible, m_focusZone_staysVisible;
    public bool[][] m_trialResults;
    public float m_zoneSize, m_zoneDist;

    public PatientTrainingSummary()
    {
    }

    /// <summary>
    /// We have to manually assign all fields here since we'll be reconstituting this object from xml
    /// when a summary is loaded and we don't want anything to end up null or undefined
    /// </summary>
    /// <param name="summary"></param>
    public PatientTrainingSummary(PatientTrainingSummary summary)
    {
        m_testName = summary.m_testName;
        m_testType = summary.m_testType;
        m_dateTime = summary.m_dateTime;
        m_shortScore = summary.m_shortScore;
        m_score_firstHalf = summary.m_score_firstHalf;
        m_score_secondHalf = summary.m_score_secondHalf;
        m_scoreFatigue = summary.m_scoreFatigue;
        m_totalHits = summary.m_totalHits;
        m_totalMisses = summary.m_totalMisses;
        m_streak = summary.m_streak;
        m_fastestHit = summary.m_fastestHit;
        m_slowestHit = summary.m_slowestHit;
        m_averageHit = summary.m_averageHit;
        m_responseTime = summary.m_responseTime;
        m_reactionTime = summary.m_reactionTime;
        m_highestlevel = summary.m_highestlevel;
        m_hitsOnHighestLevel = summary.m_hitsOnHighestLevel;
        m_faults = summary.m_faults;
        m_presented_go = summary.m_presented_go;
        m_presented_nogo = summary.m_presented_nogo;
        m_list_leveltimes = new List<float>(summary.m_list_leveltimes); //todo check for null?
        m_reaction_autoPace = summary.m_reaction_autoPace;
        m_dataLocation = summary.m_dataLocation;
        m_misses_left = summary.m_misses_left;
        m_misses_right = summary.m_misses_right;
        m_hitTimes_left = summary.m_hitTimes_left;
        m_hitTimes_right = summary.m_hitTimes_right;
        m_numberOfTrials = summary.m_numberOfTrials;
        m_trialResults = summary.m_trialResults;
        m_targetSets = summary.m_targetSets;
        m_startingLevel = summary.m_startingLevel;
        m_levelsCompleted = summary.m_levelsCompleted;
        m_testDuration = summary.m_testDuration;
        m_fileName = summary.m_fileName;
        m_balance_trackingMode = summary.m_balance_trackingMode;
        m_balance_dynamic_totalTimeOverTarget = summary.m_balance_dynamic_totalTimeOverTarget;
        m_balance_percentageOverTarget = summary.m_balance_percentageOverTarget;
        m_balance_targetDeviations = summary.m_balance_targetDeviations;
        m_operatorAccountName = summary.m_operatorAccountName;
        m_reaction_trialTimes = summary.m_reaction_trialTimes;
        m_sensitivity = summary.m_sensitivity;
        m_touchSensitivity_keepVisible = summary.m_touchSensitivity_keepVisible;
        m_presented_left = summary.m_presented_left;
        m_presented_right = summary.m_presented_right;
    }

    /// <summary>
    ///     Check if the given test has a 'config' that matches this one.  This is a function of Session graphing, so check
    ///     SessionReportMaker for specific usage.
    /// </summary>
    /// <param name="other">The summary to check, the test type MUST match the type of this summary..</param>
    public bool CheckForMatchingConfig(PatientTrainingSummary other)
    {
        if (m_testType != other.m_testType)
        {
            Debug.LogError("Test types do not match for CheckForMatchingConfig()...");
            return false;
        }

        //we'll use an abs comparison for any float to avoid rounding shenanigans 
        const float TOLERANCE = .01f;

        switch (m_testType)
        {
            case TestType.Speed:
                if (m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_gridSize != other.m_gridSize ||
                    m_targetSize != other.m_targetSize ||
                    Math.Abs(m_testDuration - other.m_testDuration) > TOLERANCE ||
                    m_paceMode != other.m_paceMode ||
                    Math.Abs(m_autopaceInterval - other.m_autopaceInterval) > TOLERANCE ||
                    m_inputMode != other.m_inputMode ||
                    Math.Abs(m_balance_hoverTime - other.m_balance_hoverTime) > TOLERANCE ||
                    m_balance_progressDecay != other.m_balance_progressDecay) return false;
                break;
            case TestType.Peripheral:
                if (Math.Abs(m_testDuration - other.m_testDuration) > TOLERANCE ||
                    Math.Abs(m_zoneDist - other.m_zoneDist) > TOLERANCE ||
                    Math.Abs(m_zoneSize - other.m_zoneSize) > TOLERANCE ||
                    m_focusZone_staysVisible != other.m_focusZone_staysVisible ||
                    m_targetArea != other.m_targetArea ||
                    m_paceMode != other.m_paceMode ||
                    Math.Abs(m_autopaceInterval - other.m_autopaceInterval) > TOLERANCE ||
                    m_inputMode != other.m_inputMode ||
                    Math.Abs(m_balance_hoverTime - other.m_balance_hoverTime) > TOLERANCE ||
                    m_balance_progressDecay != other.m_balance_progressDecay ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.Sequence:
                if (Math.Abs(m_sequence_displayTime - other.m_sequence_displayTime) > TOLERANCE ||
                    m_sequence_difficulty != other.m_sequence_difficulty ||
                    m_startingLevel != other.m_startingLevel ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_gridSize != other.m_gridSize ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.Reaction:
                if (m_reaction_orientation != other.m_reaction_orientation ||
                    m_paceMode != other.m_paceMode ||
                    Math.Abs(m_autopaceInterval - other.m_autopaceInterval) > TOLERANCE ||
                    m_inputMode != other.m_inputMode ||
                    Math.Abs(m_balance_hoverTime - other.m_balance_hoverTime) > TOLERANCE ||
                    m_balance_progressDecay != other.m_balance_progressDecay ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_numberOfTrials != other.m_numberOfTrials) return false;
                break;
            case TestType.Balance:
                if (Math.Abs(m_testDuration - other.m_testDuration) > TOLERANCE ||
                    m_balance_targetSize != other.m_balance_targetSize ||
                    Math.Abs(m_balance_hoverTime - other.m_balance_hoverTime) > TOLERANCE ||
                    m_balance_progressDecay != other.m_balance_progressDecay ||
                    m_balance_trackingDirection != other.m_balance_trackingDirection ||
                    m_balance_trackingMode != other.m_balance_trackingMode ||
                    Math.Abs(m_balance_trackingSpeed - other.m_balance_trackingSpeed) > TOLERANCE) return false;
                break;
            case TestType.Flash:
                if (Math.Abs(m_flash_displayTime - other.m_flash_displayTime) > TOLERANCE ||
                    m_flash_startingLevel != other.m_startingLevel ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_gridSize != other.m_gridSize ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.GoNoGo:
                if (Math.Abs(m_testDuration - other.m_testDuration) > TOLERANCE ||
                    m_goNogo_frequency != other.m_goNogo_frequency ||
                    Math.Abs(m_goNogo_displayTime - other.m_goNogo_displayTime) > TOLERANCE ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_gridSize != other.m_gridSize ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.Contrast:
                if (m_contrast_blendSpeed != other.m_contrast_blendSpeed ||
                    m_contrast_oversizeTargets != other.m_contrast_oversizeTargets ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    Math.Abs(m_contrast_paceTime - other.m_contrast_paceTime) > TOLERANCE ||
                    Math.Abs(m_zoneDist - other.m_zoneDist) > TOLERANCE ||
                    Math.Abs(m_zoneSize - other.m_zoneSize) > TOLERANCE ||
                    m_focusZone_staysVisible != other.m_focusZone_staysVisible ||
                    m_targetArea != other.m_targetArea ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.Rhythm:
                if (m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_targetSize != other.m_targetSize ||
                    Math.Abs(m_testDuration - other.m_testDuration) > TOLERANCE ||
                    m_paceMode != other.m_paceMode ||
                    Math.Abs(m_autopaceInterval - other.m_autopaceInterval) > TOLERANCE ||
                    m_inputMode != other.m_inputMode ||
                    Math.Abs(m_balance_hoverTime - other.m_balance_hoverTime) > TOLERANCE ||
                    m_balance_progressDecay != other.m_balance_progressDecay) return false;
                break;
            case TestType.Multi:
                if (m_multi_targetSets != other.m_targetSets ||
                    m_multi_rotationSpeed != other.m_multi_rotationSpeed ||
                    Math.Abs(m_multi_rotationTime - other.m_multi_rotationTime) > TOLERANCE ||
                    m_numberOfTrials != other.m_numberOfTrials ||
                    m_sensitivity != other.m_sensitivity ||
                    m_touchSensitivity_keepVisible != other.m_touchSensitivity_keepVisible ||
                    m_gridSize != other.m_gridSize ||
                    m_targetSize != other.m_targetSize) return false;
                break;
            case TestType.All:
            case TestType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }
}