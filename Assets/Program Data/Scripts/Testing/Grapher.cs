using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Grapher : SerializedMonoBehaviour
{
    public float drawTime, labelPadding, x_axisLabelPadding;
    private float maxVal_x, maxVal_y;
    public GameObject pfb_axisLabel;
    public Testing_DataPoint pfb_dataPoint;
    public List<float> x_axis, y_axis;
    private float xBounds_min, xBounds_max, yBounds_min, yBounds_max;

    // Use this for initialization
    private void Start()
    {
        xBounds_min = 0;
        yBounds_min = 0;
        xBounds_max = GetComponent<RectTransform>().sizeDelta.x;
        yBounds_max = GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) MakeDataPoints();
    }

    private void MakeDataPoints()
    {
        if (x_axis.Count != y_axis.Count)
        {
            Debug.Log("Graph data mismatch");
            return;
        }

        //find the highest and lowest values in each axis
        maxVal_x = x_axis.Max();
        maxVal_y = y_axis.Max();

        //now make a point at each coordinate
        for (var index = 0; index < x_axis.Count; index++)
        {
            var point = Instantiate(pfb_dataPoint, transform);
            point.x = x_axis[index];
            point.y = y_axis[index];
            StartCoroutine(DrawLine(point));

            //make the x axis label
            var label = Instantiate(pfb_axisLabel, transform);
            label.GetComponent<TextMeshProUGUI>().text = point.x.ToString("0.0");
            label.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Mathf.Lerp(xBounds_min, xBounds_max, point.x / maxVal_x), -x_axisLabelPadding);
        }
    }

    private IEnumerator DrawLine(Testing_DataPoint point)
    {
        float currentTime = 0;

        //x position does not change
        var xPos = Mathf.Lerp(xBounds_min, xBounds_max, point.x / maxVal_x);
        float initialSize = 0;
        var finalSize = Mathf.Lerp(yBounds_min, yBounds_max, point.y / maxVal_y);
        var sizeDelta = new Vector2(point.rectTransform.sizeDelta.x, 0);
        var pos = new Vector2(xPos, 0);

        while (currentTime < drawTime)
        {
            sizeDelta.y = Mathf.Lerp(initialSize, finalSize, currentTime / drawTime);
            pos.y = sizeDelta.y / 2;

            point.rectTransform.sizeDelta = sizeDelta;
            point.rectTransform.anchoredPosition = pos;

            //update the label
            point.text.text = Mathf.Lerp(0, point.y, currentTime / drawTime).ToString("0.0");
            point.text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, sizeDelta.y / 2 + labelPadding);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}