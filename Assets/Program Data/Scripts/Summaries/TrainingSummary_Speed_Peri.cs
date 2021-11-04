using TMPro;
using UnityEngine;

public class TrainingSummary_Speed_Peri : TrainingSummary_Base
{
    public WMG_Axis_Graph m_barGraph_streakVsHits;
    public WMG_Pie_Graph m_pie_hitsMisses;
    public WMG_Series m_series_streak, m_series_totalHits, m_series_hitTimes;
    public TextMeshProUGUI m_text_inputMode;
    public GameObject m_totalHitsOnly;
    public Transform m_trans_hitsMisses_pieSliceParent;

    public Transform m_trans_hitTimes_dataLabelParent,
        m_trans_streak_dataLabelParent,
        m_trans_totalHits_dataLabelParent;


    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        //input mode
        m_text_inputMode.text = e.m_inputMode;

        //make the total hits pie chart
        m_pie_hitsMisses.sliceValues[0] = e.m_totalHits;
        m_pie_hitsMisses.sliceValues[1] = e.m_totalMisses;
        //set the labels in the slices
        var t = m_trans_hitsMisses_pieSliceParent.Find("Hits").Find("Text");
        var tmp = t.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = e.m_totalHits.ToString();

        t = m_trans_hitsMisses_pieSliceParent.Find("Misses").Find("Text");
        if (t != null) //this can be null when there are 0 misses..
        {
            tmp = t.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = e.m_totalMisses.ToString();
        }

        //make the hit/streak bar graph as long as we aren't in balance mode
        if (e.m_inputMode.Equals("Touch"))
        {
            //hide the hits only object
            m_totalHitsOnly.gameObject.SetActive(false);
            m_barGraph_streakVsHits.gameObject.SetActive(true);

            m_barGraph_streakVsHits.xAxis.AxisMaxValue = e.m_totalHits;
            m_series_streak.pointValues[0] = new Vector2(1, e.m_streak);
            m_series_totalHits.pointValues[0] = new Vector2(1, e.m_totalHits);
            m_trans_totalHits_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text = e.m_totalHits.ToString();
            m_trans_streak_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text = e.m_streak.ToString();
        }
        else
        {
            //show the hits only object
            m_totalHitsOnly.gameObject.SetActive(true);
            m_barGraph_streakVsHits.gameObject.SetActive(false);

            m_totalHitsOnly.GetComponentInChildren<TextMeshProUGUI>().text = "Total Hits: " + e.m_totalHits;
        }

        //make the fast/slow/average bar graph
        m_series_hitTimes.pointValues[0] = new Vector2(1, e.m_slowestHit);
        m_series_hitTimes.pointValues[1] = new Vector2(1, e.m_fastestHit);
        m_series_hitTimes.pointValues[2] = new Vector2(1, e.m_averageHit);

        //now we need to place and set the data labels for the hit times
        var dataLabels = m_trans_hitTimes_dataLabelParent.GetComponentsInChildren<TextMeshProUGUI>();
        dataLabels[0].text = e.m_slowestHit.ToString("0.00") + " s";
        dataLabels[1].text = e.m_fastestHit.ToString("0.00") + " s";
        dataLabels[2].text = e.m_averageHit.ToString("0.00") + " s";
    }
}