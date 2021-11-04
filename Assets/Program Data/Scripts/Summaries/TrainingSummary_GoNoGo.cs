using TMPro;
using UnityEngine;

public class TrainingSummary_GoNoGo : TrainingSummary_Base
{
    public WMG_Axis_Graph m_barGraph_go_presentedVsHits, m_barGraph_nogo_presentedVsHits;

    public WMG_Series m_series_go_presented,
        m_series_go_totalHits,
        m_series_hitTimes,
        m_series_nogo_presented,
        m_series_nogo_totalHits;

    public Transform m_trans_hitTimes_dataLabelParent,
        m_trans_go_totalHits_dataLabelParent,
        m_trans_go_presented_dataLabelParent,
        m_trans_noGo_totalHits_dataLabelParent,
        m_trans_noGo_presented_dataLabelParent;


    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        //go
        //make the presented vs. hit bar graph
        m_barGraph_go_presentedVsHits.xAxis.AxisMaxValue = e.m_presented_go;
        m_series_go_presented.pointValues[0] = new Vector2(1, e.m_presented_go);
        m_series_go_totalHits.pointValues[0] = new Vector2(1, e.m_totalHits);
        m_trans_go_presented_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text =
            e.m_presented_go.ToString("0");
        m_trans_go_totalHits_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text =
            e.m_totalHits.ToString("0");

        //populate the hit times bar graph
        m_series_hitTimes.pointValues[0] = new Vector2(1, e.m_slowestHit);
        m_series_hitTimes.pointValues[1] = new Vector2(1, e.m_fastestHit);
        m_series_hitTimes.pointValues[2] = new Vector2(1, e.m_averageHit);
        //and the labels
        var dataLabels = m_trans_hitTimes_dataLabelParent.GetComponentsInChildren<TextMeshProUGUI>();
        dataLabels[0].text = e.m_slowestHit.ToString("0.00") + " s";
        dataLabels[1].text = e.m_fastestHit.ToString("0.00") + " s";
        dataLabels[2].text = e.m_averageHit.ToString("0.00") + " s";


        //nogo
        //make the presented vs. hit bar graph
        m_barGraph_nogo_presentedVsHits.xAxis.AxisMaxValue = e.m_presented_nogo;
        m_series_nogo_presented.pointValues[0] = new Vector2(1, e.m_presented_nogo);
        m_series_nogo_totalHits.pointValues[0] = new Vector2(1, e.m_faults);
        m_trans_noGo_presented_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text =
            e.m_presented_nogo.ToString("0");
        m_trans_noGo_totalHits_dataLabelParent.GetComponentInChildren<TextMeshProUGUI>().text =
            e.m_faults.ToString("0");
    }
}