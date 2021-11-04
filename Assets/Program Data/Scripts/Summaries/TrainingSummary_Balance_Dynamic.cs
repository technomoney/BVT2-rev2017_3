using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSummary_Balance_Dynamic : TrainingSummary_Base
{
    public Image m_image_overTarget, m_image_backing;

    public TextMeshProUGUI m_text_onTarget_percentage,
        m_text_onTarget_time,
        m_text_offTarget_percentage,
        m_text_offTarget_time,
        m_text_deviations;


    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        m_text_deviations.text = "Target Deviations: " + e.m_balance_targetDeviations;

        StartCoroutine(FillBar(e.m_balance_percentageOverTarget, e.m_balance_dynamic_totalTimeOverTarget,
            e.m_testDuration));
    }

    private IEnumerator FillBar(float percentage, float timeOverTarget, float duration)
    {
        var timeToFill = 1f;
        float currentTime = 0;
        var finalSize = m_image_backing.rectTransform.sizeDelta.x * percentage / 100;
        var height = m_image_overTarget.rectTransform.sizeDelta.y;

        while (currentTime <= timeToFill)
        {
            m_image_overTarget.rectTransform.sizeDelta =
                new Vector2(Mathf.Lerp(0, finalSize, currentTime / timeToFill), height);

            m_text_onTarget_percentage.text = Mathf.Lerp(0, percentage, currentTime / timeToFill).ToString("0.0") + "%";
            m_text_onTarget_time.text =
                Mathf.Lerp(0, timeOverTarget, currentTime / timeToFill).ToString("0.0") + " sec";

            m_text_offTarget_percentage.text =
                Mathf.Lerp(0, 100 - percentage, currentTime / timeToFill).ToString("0.0") + "%";
            m_text_offTarget_time.text =
                Mathf.Lerp(0, duration - timeOverTarget, currentTime / timeToFill).ToString("0.0") + " sec";

            currentTime += Time.deltaTime;
            yield return 0;
        }

        m_text_onTarget_percentage.text = percentage.ToString("0.0") + "%";
        m_text_onTarget_time.text = timeOverTarget.ToString("0.0") + " sec";

        m_text_offTarget_percentage.text = (100 - percentage).ToString("0.0") + "%";
        m_text_offTarget_time.text = (duration - timeOverTarget).ToString("0.0") + " sec";
    }
}