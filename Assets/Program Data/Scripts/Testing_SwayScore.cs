using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Testing_SwayScore : MonoBehaviour
{
    private List<float>[] m_fullData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            CalculateSwayScore();
    }

    private void CalculateSwayScore()
    {
        m_fullData = new List<float>[3];
        m_fullData[0] = new List<float>();
        m_fullData[1] = new List<float>();
        m_fullData[2] = new List<float>();

        var sr = File.OpenText(Application.dataPath + "\\fulldata.txt");

        var contents = sr.ReadToEnd();

        char[] entries = {' ', '\n', '\r'};
        var splitContents = contents.Split(entries, StringSplitOptions.RemoveEmptyEntries);

        for (var x = 0; x <= splitContents.Length - 3; x += 3)
        {
            m_fullData[0].Add(float.Parse(splitContents[x]));
            m_fullData[1].Add(float.Parse(splitContents[x + 1]));
            m_fullData[2].Add(float.Parse(splitContents[x + 2]));
        }

        Debug.Log("Done loading data from file");

        /*
        D_ML=zeros(1,length(COPX));
        D_AP=zeros(1,length(COPX));
        D=zeros(1,length(COPX));

        for i=1:length(COPX)-1
        D_ML(i)=abs(COPX(i+1)-COPX(i));
        D_AP(i)=abs(COPY(i+1)-COPY(i));
        D(i)=((COPX(i+1)-COPX(i))^2+(COPY(i+1)-(COPY(i)))^2)^0.5;
        end*/

        var D_ML = new float[m_fullData[1].Count];
        var D_AP = new float[m_fullData[1].Count];
        var D = new float[m_fullData[1].Count];

        for (var i = 0; i < m_fullData[1].Count - 1; i++)
        {
            D_ML[i] = Mathf.Abs(m_fullData[1][i + 1] - m_fullData[1][i]);
            D_AP[i] = Mathf.Abs(m_fullData[2][i + 1] - m_fullData[2][i]);
            D[i] = Mathf.Pow(Mathf.Pow(D_ML[i], 2) + Mathf.Pow(D_AP[i], 2), .5f);
        }

        var d_ml_avg = D_ML.ToList().Average();
        var d_ap_avg = D_AP.ToList().Average();
        var d_avg = D.ToList().Average();

        Debug.Log(d_ml_avg + " / " + d_ap_avg + " / " + d_avg);
    }
}