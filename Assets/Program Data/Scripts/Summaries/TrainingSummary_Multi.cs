using TMPro;
using UnityEngine;

public class TrainingSummary_Multi : TrainingSummary_Base
{
    public TextMeshProUGUI[] m_resultLabels;
    public GameObject[] m_results;
    public TextMeshProUGUI m_text_score;
    public TextMeshProUGUI m_text_trialLabels;

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        //always show the score
        m_text_score.text = "Score: " + e.m_shortScore;

        //hide all results initially
        m_results[0].gameObject.SetActive(false);
        m_results[1].gameObject.SetActive(false);
        m_results[2].gameObject.SetActive(false);
        m_results[3].gameObject.SetActive(false);
        m_resultLabels[0].text = "";
        m_resultLabels[1].text = "";
        m_resultLabels[2].text = "";
        m_resultLabels[3].text = "";

        m_text_trialLabels.text = "";

        switch (e.m_multi_trials)
        {
            case 3:
                m_text_trialLabels.text = "1\n2\n3";
                break;
            case 5:
                m_text_trialLabels.text = "1\n2\n3\n4\n5";
                break;
            case 7:
                m_text_trialLabels.text = "1\n2\n3\n4\n5\n6\n7";
                break;
        }

        switch (e.m_multi_targetSets)
        {
            case 1:
                m_results[0].gameObject.SetActive(true);
                break;
            case 2:
                m_results[0].gameObject.SetActive(true);
                m_results[1].gameObject.SetActive(true);
                break;
            case 3:
                m_results[0].gameObject.SetActive(true);
                m_results[1].gameObject.SetActive(true);
                m_results[2].gameObject.SetActive(true);
                break;
            case 4:
                m_results[0].gameObject.SetActive(true);
                m_results[1].gameObject.SetActive(true);
                m_results[2].gameObject.SetActive(true);
                m_results[3].gameObject.SetActive(true);
                break;
        }

        for (var trial = 0; trial < e.m_multi_trials; trial++)
        for (var targetSet = 0; targetSet < e.m_multi_targetSets; targetSet++)
            m_resultLabels[targetSet].text += (e.m_trialResults[trial][targetSet] ? "Correct" : "Incorrect") + "\n";
    }
}