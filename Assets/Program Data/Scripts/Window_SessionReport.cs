using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_SessionReport : BVT_Window
{
    public static Window_SessionReport Inst;
    public List<Color> m_colors;
    public Transform m_graphParent;
    private List<SessionGraph> m_graphs;
    public SessionGraph pfb_sessionGraph;

    public override void Start()
    {
        base.Start();
        Inst = this;
        transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(Hide);
        m_graphs = new List<SessionGraph>();
    }

    public void MakeGraph_SingleDay(List<List<PatientTrainingSummary>>[] sortedResults)
    {
        var index = 0;
        foreach (var training in sortedResults)
        {
            if (training.Count <= 0) continue;
            var graph = Instantiate(pfb_sessionGraph, m_graphParent); 
            m_graphs.Add(graph);
            graph.Initialize();
            graph.PlotConfigs(training, index);
            index++;
        }
    }

    public void MakeGraph_MultiDay(List<List<PatientTrainingSummary>>[] sortedResults)
    {
        var index = 0;
        foreach (var training in sortedResults)
        {
            if (training.Count <= 0) continue;
            var graph = Instantiate(pfb_sessionGraph, m_graphParent); 
            m_graphs.Add(graph);
            graph.Initialize();
            graph.PlotDays3(training, index);
            index++;
        }
    }

    public override void Hide()
    {
        //when we close a session report window we need to nuke any graphs that are currently there
        m_graphs.ForEach(g => Destroy(g.gameObject));
        m_graphs.Clear();
        m_graphs = new List<SessionGraph>();
        base.Hide();
    }
}