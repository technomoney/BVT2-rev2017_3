using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bp_Grapher : SerializedMonoBehaviour
{
    //this will hold our samples of data per second
    private List<float>[] ds2DataX;
    private List<float>[] ds2DataY;

    //this will hold our full data, by second
    private List<float>[] fullDataX;
    private List<float>[] fullDataY;
    private List<float>[] m_downsampledData_x, m_downsampledData_y;
    public string m_file_path;

    private List<float>[] m_fullData;
    public int m_samplesPerSecond;
    public float m_samplingRate;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ReadInData();

        if (Input.GetKey(KeyCode.D)) DownsampleData();

        if (Input.GetKeyDown(KeyCode.P)) Downsample2();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReadInData();
            DownsampleData();
        }
    }

    private void ReadInData()
    {
        m_fullData = new List<float>[3];
        m_fullData[0] = new List<float>();
        m_fullData[1] = new List<float>();
        m_fullData[2] = new List<float>();

        var sr = File.OpenText(m_file_path);

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
    }

    private void DownsampleData()
    {
        //get the number of whole seconds in the data
        var dataSec = (int) (m_fullData[0].Count / 1000f);
        //this should round up...
        var leftovers = m_fullData[0].Count - dataSec * 1000;

        //now make a new array with that many entries that will store all data for a given second
        var m_dataSeconds_x = new List<float>[dataSec];
        var m_dataSeconds_y = new List<float>[dataSec];
        for (var second = 0; second < dataSec; second++)
        {
            m_dataSeconds_x[second] = new List<float>();
            m_dataSeconds_y[second] = new List<float>();

            if (second == 0)
            {
                m_dataSeconds_x[second].AddRange(m_fullData[1].GetRange(0, 1000));
                m_dataSeconds_y[second].AddRange(m_fullData[2].GetRange(0, 1000));
                continue;
            }

            if (second == dataSec - 1)
            {
                m_dataSeconds_x[second].AddRange(m_fullData[1].GetRange(dataSec * 1000, leftovers));
                m_dataSeconds_y[second].AddRange(m_fullData[2].GetRange(dataSec * 1000, leftovers));
                break;
            }

            m_dataSeconds_x[second].AddRange(m_fullData[1].GetRange(second * 1000, 1000));
            m_dataSeconds_y[second].AddRange(m_fullData[2].GetRange(second * 1000, 1000));
        }

        //now we have the data per second, so finally average each second out based on our point per second
        m_downsampledData_x = new List<float>[dataSec];
        m_downsampledData_y = new List<float>[dataSec];
        float total_x = 0, total_y = 0;
        var totalEntries = 1000f / m_samplesPerSecond;
        for (var second = 0; second < dataSec; second++)
        {
            m_downsampledData_x[second] = new List<float>();
            m_downsampledData_y[second] = new List<float>();

            for (var segment = 0; segment < m_samplesPerSecond; segment++)
            {
                if (second == 0)
                {
                    total_x = total_y = 0;
                    for (var x = 0; x < (int) totalEntries; x++)
                    {
                        total_x += m_dataSeconds_x[second][x];
                        total_y += m_dataSeconds_y[second][x];
                    }

                    m_downsampledData_x[second].Add(total_x / totalEntries);
                    m_downsampledData_y[second].Add(total_y / totalEntries);
                    continue;
                }

                if (second == dataSec - 1)
                {
                    total_x = total_y = 0;
                    for (var x = 0; x < leftovers; x++)
                    {
                        total_x += m_dataSeconds_x[second][x];
                        total_y += m_dataSeconds_y[second][x];
                    }

                    m_downsampledData_x[second].Add(total_x / leftovers);
                    m_downsampledData_y[second].Add(total_y / leftovers);
                    break;
                }

                total_x = total_y = 0;
                for (var x = (int) totalEntries * segment; x < (int) totalEntries; x++)
                {
                    total_x += m_dataSeconds_x[second][x];
                    total_y += m_dataSeconds_y[second][x];
                }

                m_downsampledData_x[second].Add(total_x / totalEntries);
                m_downsampledData_y[second].Add(total_y / totalEntries);
            }
        }

        Debug.Log("Done down sampling data");
    }

    public Vector2 GetMaxVals()
    {
        var maxX = new float[m_downsampledData_x.Length];
        var maxY = new float[m_downsampledData_x.Length];
        for (var index = 0; index < m_downsampledData_x.Length; index++)
        {
            maxX[index] = m_downsampledData_x[index].Max();
            maxY[index] = m_downsampledData_y[index].Max();
        }

        return new Vector2(maxX.Max(), maxY.Max());
    }

    private void Downsample2()
    {
        //A lot of this depends on an initial sampling rate of 1000...

        //this give us how many full seconds of data we have
        var secondsOfData = m_fullData[0].Count / 1000;
        //for our graphing here, we'll clip any leftovers..
        secondsOfData -= 1;

        fullDataX = new List<float>[secondsOfData];
        fullDataY = new List<float>[secondsOfData];
        ds2DataX = new List<float>[secondsOfData];
        ds2DataY = new List<float>[secondsOfData];

        //populate our data by second
        for (var second = 0; second < secondsOfData; second++)
        {
            fullDataX[second] = m_fullData[1].GetRange(second * 1000, 1000);
            fullDataY[second] = m_fullData[2].GetRange(second * 1000, 1000);
        }

        //now populate the downsampled lists
        for (var second = 0; second < secondsOfData; second++)
        {
            ds2DataX[second] = new List<float>();
            ds2DataY[second] = new List<float>();
            for (var segment = 0; segment < m_samplesPerSecond; segment++)
            {
                //x
                var seg = fullDataX[second]
                    .GetRange(segment * 1000 / m_samplesPerSecond, 1000 / m_samplesPerSecond);
                float total = 0, avg = 0;
                seg.ForEach(f => total += f);
                avg = total / seg.Count;
                ds2DataX[second].Add(avg);
                //y
                seg = fullDataX[second]
                    .GetRange(segment * 1000 / m_samplesPerSecond, 1000 / m_samplesPerSecond);
                total = 0;
                avg = 0;
                seg.ForEach(f => total += f);
                avg = total / seg.Count;
                ds2DataY[second].Add(avg);
            }
        }

        Debug.Log("Downsample2() done");
    }

    public List<float> Ds2ToMagnitude()
    {
        //convert our downsampled 2 data in to cop magnitudes
        var magnitude = new List<float>();

        for (var second = 0; second < ds2DataX.Length; second++)
        for (var sample = 0; sample < ds2DataX[second].Count; sample++)
            magnitude.Add(Mathf.Abs(ds2DataX[second][sample]) + Mathf.Abs(ds2DataY[second][sample]));

        return magnitude;
    }

    public Vector2 GetMaxVals2()
    {
        var maxX = new float[ds2DataX.Length];
        var maxY = new float[ds2DataY.Length];
        for (var index = 0; index < ds2DataX.Length; index++)
        {
            maxX[index] = ds2DataX[index].Max();
            maxY[index] = ds2DataY[index].Max();
        }

        return new Vector2(maxX.Max(), maxY.Max());
    }
}