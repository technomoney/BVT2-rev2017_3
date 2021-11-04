using System.Collections.Generic;
using TMPro;

public class TrainingSummary_Balance_Static : TrainingSummary_Base
{
    public Graph_HitTimes m_graph_hitTimes;
    public Graph_Xaxis_Bar m_graph_timeToHit;
    public TextMeshProUGUI m_text_totalHits;

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        m_text_totalHits.text = "Total Hits: " + e.m_totalHits;

        m_graph_hitTimes.Show(e.m_averageHit, e.m_fastestHit, e.m_slowestHit);

        var hits = new List<float>();
        var x = 1;
        for (var index = 0; index < e.m_hitTimes.Count; index++)
        {
            hits.Add(x);
            x++;
        }

        m_graph_timeToHit.m_data_x = hits;
        m_graph_timeToHit.m_data_y = e.m_hitTimes;
        m_graph_timeToHit.MakeGraph(true);
    }

    public override void Close()
    {
        m_graph_timeToHit.DestroyDataPoints();
        base.Close();
    }
}