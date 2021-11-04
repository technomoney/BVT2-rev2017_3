using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class TrainingSummary_Balance : TrainingSummary_Base
{
	/// <summary>
	///     An array of lists for the cop X data. Each array element will be 1 second, and the list itself will be a number
	///     of samples dictated by m_downsampleRate
	/// </summary>
	private List<float>[] dsDataX;

	/// <summary>
	///     An array of lists for the cop Y data. Each array element will be 1 second, and the list itself will be a number
	///     of samples dictated by m_downsampleRate
	/// </summary>
	private List<float>[] dsDataY;

	/// <summary>
	///     Array to hold all of the cop X data
	/// </summary>
	private List<float>[] fullDataX;

	/// <summary>
	///     Array to hold all of the cop Y data
	/// </summary>
	private List<float>[] fullDataY;

	/// <summary>
	///     the parent object that will hold our data points
	/// </summary>
	public RectTransform m_dataObject;

    private List<float>[] m_downsampledData_x, m_downsampledData_y;

    /// <summary>
    ///     the frequency of how we'll downsample the fp data when we're graphing it here
    /// </summary>
    public int m_downsampleRate;

    /// <summary>
    ///     when hard coding a path, use this..
    /// </summary>
    public string m_filePath;

    /// <summary>
    ///     An array of lists to hold all the data from the file, it will be time/copX/copY
    /// </summary>
    private List<float>[] m_fullData;

    /// <summary>
    ///     The physcial bounds of the graphing area
    /// </summary>
    private Vector2 m_graphBounds;

    /// <summary>
    ///     List to hold our instantiated objects on the graph
    /// </summary>
    private List<GameObject> m_graphObjects;

    /// <summary>
    ///     the physcial component used to determine the bounds
    /// </summary>
    public RectTransform m_graphTransform;

    public Transform m_hashMarks_x_parent;

    /// <summary>
    ///     the pfb we'll use to plot the down sampled data
    /// </summary>
    public GameObject pfb_dataPoint;

    public RectTransform pfb_hashLabel_x;

    public RectTransform pfb_hashMark_x_full, pfb_hashMark_x_half;

    /// <summary>
    /// List to hold our touch lines
    /// </summary>
    //private List<RectTransform> m_touchLines;
    /// <summary>
    ///     prefab to draw the lines on the graph
    /// </summary>
    public LineRenderer pfb_lineRenderer;

    /// <summary>
    ///     the pfb for the line indicating a touch
    /// </summary>
    public RectTransform pfb_touchLine;

    public override void Show(PatientTrainingSummary e)
    {
        //testing
        base.Show(e);

        //we need to open our data file
        //todo will probably have to have some kind of smart file get based on the passed training summary
        //for now, we'll just use a generic location

        ReadInData();
        DownsampleData();

        //now graph the data
        m_graphObjects = new List<GameObject>();
        var data = GetDataMagnitudes();
        m_graphBounds = m_graphTransform.sizeDelta;
        //we need the max y value so we can interpolate the correct location on the y axis
        var maxValY = data.Max();

        var dataLength = data.Count;

        var pos = Vector2.zero;

        for (var index = 0; index < dataLength; index++)
        {
            pos.x = Mathf.Lerp(0, m_graphBounds.x, (float) index / dataLength);
            pos.y = Mathf.Lerp(0, m_graphBounds.y, data[index] / maxValY);
            var point = Instantiate(pfb_dataPoint, m_dataObject);
            point.GetComponent<RectTransform>().anchoredPosition = pos;
            m_graphObjects.Add(point);
        }

        Debug.Log("Graphed " + dataLength + " objects");

        //draw the lines
        var line = Instantiate(pfb_lineRenderer, m_dataObject);
        line.positionCount = dataLength;
        for (var x = 0; x < dataLength; x++)
            line.SetPosition(x, m_graphObjects[x].GetComponent<RectTransform>().anchoredPosition);

        //add the touches
        //for now we'll make a fake like of time stamps with touches..
        var touches = new List<float> {1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10, 11, 12};

        //we need to cut down on the touches to match the number of seconds of fp data we have
        if (touches.Count > dsDataX.Length) touches = touches.GetRange(0, dsDataX.Length);

        //add the touch lines
        //m_touchLines = new List<RectTransform>();
        foreach (var f in touches)
        {
            //todo add the touch lines....
            //RectTransform t = Instantiate(pfb_touchLine, m_dataObject);
            //t.sizeDelta = new Vector2(t.sizeDelta.x, m_graphBounds.y);
            //t.anchoredPosition = new Vector2(Mathf.Lerp(0, m_graphBounds.x, f / touches.Count), 0);
            //m_touchLines.Add(t);
        }

        //add the hash marks and labels
        //x
        //full size at 25/50/70/100
        //halfsize at 12.5, 37.5, 62.5, 87.5
        var hash = .25f;
        for (var x = 0; x < 4; x++)
        {
            var h = Instantiate(pfb_hashMark_x_full, m_hashMarks_x_parent);
            h.anchoredPosition = new Vector2(Mathf.Lerp(0, m_graphBounds.x, hash), h.anchoredPosition.y);
            var text = Instantiate(pfb_hashLabel_x, m_hashMarks_x_parent);
            text.anchoredPosition = new Vector2(h.anchoredPosition.x, text.anchoredPosition.y);
            text.GetComponent<TextMeshProUGUI>().text = (hash * (dsDataX.Count() + 1)).ToString("0.0");

            h = Instantiate(pfb_hashMark_x_half, m_hashMarks_x_parent);
            h.anchoredPosition = new Vector2(Mathf.Lerp(0, m_graphBounds.x, hash / 2), h.anchoredPosition.y);

            hash += .25f;
        }
    }

    private void Update()
    {
        //testing
        //if (Input.GetKeyDown(KeyCode.J))
        //	Show(null);
    }

    /// <summary>
    ///     Populate m_fulldata with each column of data from our file
    /// </summary>
    private void ReadInData()
    {
        m_fullData = new List<float>[3];
        m_fullData[0] = new List<float>();
        m_fullData[1] = new List<float>();
        m_fullData[2] = new List<float>();

        //todo this is testing only for now, need to point to the actual data...
        //StreamReader sr = File.OpenText(m_filePath);
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
    }


    private void DownsampleData()
    {
        //A lot of this depends on an initial sampling rate of 1000...

        //this give us how many full seconds of data we have
        var secondsOfData = m_fullData[0].Count / 1000;
        //for our graphing here, we'll clip any leftovers..
        secondsOfData -= 1;

        fullDataX = new List<float>[secondsOfData];
        fullDataY = new List<float>[secondsOfData];
        dsDataX = new List<float>[secondsOfData];
        dsDataY = new List<float>[secondsOfData];

        //populate our data by second
        for (var second = 0; second < secondsOfData; second++)
        {
            fullDataX[second] = m_fullData[1].GetRange(second * 1000, 1000);
            fullDataY[second] = m_fullData[2].GetRange(second * 1000, 1000);
        }

        //now populate the downsampled lists
        for (var second = 0; second < secondsOfData; second++)
        {
            dsDataX[second] = new List<float>();
            dsDataY[second] = new List<float>();
            for (var segment = 0; segment < m_downsampleRate; segment++)
            {
                //x
                var seg = fullDataX[second]
                    .GetRange(segment * 1000 / m_downsampleRate, 1000 / m_downsampleRate);
                float total = 0, avg = 0;
                seg.ForEach(f => total += f);
                avg = total / seg.Count;
                dsDataX[second].Add(avg);
                //y
                seg = fullDataX[second]
                    .GetRange(segment * 1000 / m_downsampleRate, 1000 / m_downsampleRate);
                total = 0;
                avg = 0;
                seg.ForEach(f => total += f);
                avg = total / seg.Count;
                dsDataY[second].Add(avg);
            }
        }

        Debug.Log("Finished downsampling data");
    }

    private List<float> GetDataMagnitudes()
    {
        //convert our downsampled 2 data in to cop magnitudes
        var magnitude = new List<float>();

        for (var second = 0; second < dsDataX.Length; second++)
        for (var sample = 0; sample < dsDataX[second].Count; sample++)
            magnitude.Add(Mathf.Abs(dsDataX[second][sample]) + Mathf.Abs(dsDataY[second][sample]));

        return magnitude;
    }
}