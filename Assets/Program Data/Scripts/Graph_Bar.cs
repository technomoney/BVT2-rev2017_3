using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Graph_Bar : SerializedMonoBehaviour
{
    public float m_entrySpacing;
    public Vector2 m_initialPosition;
    public List<Color> m_list_colors;
    public List<string> m_list_titles;

    public List<float> m_list_values;
    public float m_maxValue;
    private Vector2 m_nextPosition;

    public Graph_Bar_Entry pfb_barEntry;

    private void Awake()
    {
        if (m_list_values == null)
            m_list_values = new List<float>();

        m_nextPosition = m_initialPosition;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ShowGraph();
    }

    public void ShowGraph()
    {
        foreach (var v in m_list_values)
        {
            var b = Instantiate(pfb_barEntry, transform);
            b.m_rectTransform.anchoredPosition = m_nextPosition;
            m_nextPosition.y -= m_entrySpacing;
            b.SetValue(v, m_maxValue, m_list_colors[m_list_values.IndexOf(v)], m_list_titles[m_list_values.IndexOf(v)]);
        }
    }
}