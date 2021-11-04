using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Graph_Xaxis_Bar : SerializedMonoBehaviour
{
    public float m_barWidth, m_labelPadding_upper, m_labelPadding_lower;
    public List<float> m_data_x, m_data_y;

    private float m_data_y_max, m_xBounds_min, m_xBounds_max, m_yBounds_min, m_yBounds_max; //m_data_x_max
    private List<DataPoint> m_dataPoints;

    public float m_drawTime;
    public DataPoint m_pfb_dataPoint;
    public RectTransform m_trans_dataPoints;


    private void Update()
    {
        //testing
        //if(Input.GetKeyDown(KeyCode.Space))
        //MakeGraph();
    }

    public void MakeGraph(bool truncateXaxisLabels = false)
    {
        if (m_data_x.Count != m_data_y.Count)
        {
            Debug.Log("Graph data mismatch");
            return;
        }

        m_dataPoints = new List<DataPoint>();

        m_xBounds_min = m_data_x.Min();
        m_xBounds_max = m_trans_dataPoints.sizeDelta.x;
        m_yBounds_min = 0;
        m_yBounds_max = m_trans_dataPoints.sizeDelta.y;

        //find the mighest magnitude items from each axis
        //m_data_x_max = m_data_x.Max();
        m_data_y_max = m_data_y.Max();

        //now make a datapoint at each coordinate
        var counter = 0;
        var showYaxisLabel = true;
        for (var index = 0; index < m_data_x.Count; index++)
        {
            var point = Instantiate(m_pfb_dataPoint, m_trans_dataPoints);
            point.m_data_x = m_data_x[index];
            point.m_data_y = m_data_y[index];
            m_dataPoints.Add(point);

            //should we make an x axis label?
            bool makeLabel;
            if (index == 0)
            {
                makeLabel = true;
            }
            else
            {
                if (counter > 3) //this will make a label every 3..
                {
                    counter = 0;
                    makeLabel = true;
                }
                else
                {
                    counter++;
                    makeLabel = false;
                }
            }

            if (index == 0 || index == m_data_x.Count - 1) showYaxisLabel = true;
            else showYaxisLabel = !showYaxisLabel;

            StartCoroutine(DrawBar(point, index, m_data_x.Count, makeLabel, showYaxisLabel, truncateXaxisLabels));
        }
    }

    public void DestroyDataPoints()
    {
        for (var x = 0; x < m_dataPoints.Count; x++)
            Destroy(m_dataPoints[x].gameObject);

        m_dataPoints.Clear();
    }

    private IEnumerator DrawBar(DataPoint point, int pointIndex, int pointCount, bool showXaxisLabel,
        bool showYaxisLabel, bool trunctaeXlabels)
    {
        float currentTime = 0;
        //set the position of the bar point along the x axis
        point.m_rectTransform.anchoredPosition =
            new Vector2(Mathf.Lerp(m_xBounds_min, m_xBounds_max, (float) pointIndex / pointCount), 0);

        var finalSize = Mathf.Lerp(0, m_yBounds_max, point.m_data_y / m_data_y_max);
        var sizeDelta = new Vector2(m_barWidth, 0);

        //set the label for the xaxis
        point.m_text_label_lower.text = showXaxisLabel
            ? trunctaeXlabels ? point.m_data_x.ToString("0") : point.m_data_x.ToString("0.000")
            : "";


        while (currentTime < m_drawTime)
        {
            sizeDelta.y = Mathf.Lerp(m_yBounds_min, finalSize, currentTime / m_drawTime);
            point.m_rectTransform.sizeDelta = sizeDelta;

            //update the labels
            point.m_text_label_upper.text = showYaxisLabel
                ? Mathf.Lerp(0, point.m_data_y, currentTime / m_drawTime).ToString("0.0")
                : "";

            currentTime += Time.deltaTime;
            yield return null;
        }

        //before we finish update the label one last time to the actual result to account for timing/rounding errors..
        point.m_text_label_upper.text = showYaxisLabel ? point.m_data_y.ToString("0.0") : "";
    }
}