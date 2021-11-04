using System.Linq;
using TMPro;

public class TrainingSummary_Contrast : TrainingSummary_Base
{
    public Graph_Xaxis_Bar m_graph_contrastVsTime;
    public Graph_HitTimes m_graph_hitTimes;
    public TextMeshProUGUI m_text_lowestContrast;

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);
        m_graph_hitTimes.Show(e.m_averageHit, e.m_fastestHit, e.m_slowestHit);
        m_graph_contrastVsTime.m_data_x = e.m_list_contrastSteps;
        m_graph_contrastVsTime.m_data_y = e.m_hitTimes;
        m_graph_contrastVsTime.MakeGraph();
        m_text_lowestContrast.text =
            "Lowest Contrast Identified: " + (e.m_list_contrastSteps.Last() * 100).ToString("0.0") + "%";
    }

    public override void Close()
    {
        m_graph_contrastVsTime.DestroyDataPoints();
        base.Close();
    }
}