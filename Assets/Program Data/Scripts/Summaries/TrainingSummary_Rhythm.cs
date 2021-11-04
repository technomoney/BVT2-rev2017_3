using System.Linq;
using TMPro;
using UnityEngine;

public class TrainingSummary_Rhythm : TrainingSummary_Base
{
    public Transform m_hitTimes_left_dataParent, m_hitTimes_right_dataParent;
    public WMG_Series m_series_hitTimes_left, m_series_hitTimes_right;

    public TextMeshProUGUI m_text_duration, m_text_hits;

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);
        TextMeshProUGUI[] dataLabels;
        //left
        if (e.m_hitTimes_left.Count > 0)
        {
            m_series_hitTimes_left.pointValues[0] = new Vector2(1, e.m_hitTimes_left.Max());
            m_series_hitTimes_left.pointValues[1] = new Vector2(1, e.m_hitTimes_left.Min());
            m_series_hitTimes_left.pointValues[2] = new Vector2(1, e.m_hitTimes_left.Average());
            //labels
            dataLabels = m_hitTimes_left_dataParent.GetComponentsInChildren<TextMeshProUGUI>();
            dataLabels[0].text = e.m_hitTimes_left.Max().ToString("0.00") + " s";
            dataLabels[1].text = e.m_hitTimes_left.Min().ToString("0.00") + " s";
            dataLabels[2].text = e.m_hitTimes_left.Average().ToString("0.00") + " s";
        }
        else
        {
            m_series_hitTimes_left.pointValues[0] = new Vector2(1, 0);
            m_series_hitTimes_left.pointValues[1] = new Vector2(1, 0);
            m_series_hitTimes_left.pointValues[2] = new Vector2(1, 0);
            //labels
            dataLabels = m_hitTimes_left_dataParent.GetComponentsInChildren<TextMeshProUGUI>();
            dataLabels[0].text = "No data available";
            dataLabels[1].text = "No data available";
            dataLabels[2].text = "No data available";
        }

        //right
        if (e.m_hitTimes_right.Count > 0)
        {
            m_series_hitTimes_right.pointValues[0] = new Vector2(1, e.m_hitTimes_right.Max());
            m_series_hitTimes_right.pointValues[1] = new Vector2(1, e.m_hitTimes_right.Min());
            m_series_hitTimes_right.pointValues[2] = new Vector2(1, e.m_hitTimes_right.Average());
            //labels
            dataLabels = m_hitTimes_right_dataParent.GetComponentsInChildren<TextMeshProUGUI>();
            dataLabels[0].text = e.m_hitTimes_right.Max().ToString("0.00") + " s";
            dataLabels[1].text = e.m_hitTimes_right.Min().ToString("0.00") + " s";
            dataLabels[2].text = e.m_hitTimes_right.Average().ToString("0.00") + " s";
        }
        else
        {
            m_series_hitTimes_right.pointValues[0] = new Vector2(1, 0);
            m_series_hitTimes_right.pointValues[1] = new Vector2(1, 0);
            m_series_hitTimes_right.pointValues[2] = new Vector2(1, 0);
            //labels
            dataLabels = m_hitTimes_right_dataParent.GetComponentsInChildren<TextMeshProUGUI>();
            dataLabels[0].text = "No data available";
            dataLabels[1].text = "No data available";
            dataLabels[2].text = "No data available";
        }

        m_text_duration.text = "Duration: " + e.m_testDuration + " s";
        var totalPresented = e.m_presented_left + e.m_presented_right;
        //m_text_hits.text = "Total Hits: " + e.m_totalHits + "/" + (e.m_totalHits + e.m_totalMisses);
        m_text_hits.text = "Total Hits: " + e.m_totalHits + "/" + totalPresented;
    }
}