using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSummary_Reaction : TrainingSummary_Base
{
    public float m_arrowMoveSpeed, m_arrow_finalWidth, m_arrow_initialWidth;
    public Graph_HitTimes m_graph_hitTimes;
    public Image m_image_arrow;
    public Image m_image_needle;
    public Image m_image_targetRing;
    public float m_needleMoveSpeed;
    public float m_ringSpeed, m_finalRingSize;
    private float m_speedFactor, m_finalNeedleAngle;
    public Transform m_tableContent;
    private List<GameObject> m_tableEntries;
    public TextMeshProUGUI m_text_inputMode, m_text_direction, m_text_trials;
    public TextMeshProUGUI m_text_responseTime, m_text_reactionTime, m_text_speedFactor;
    private Vector2 m_vec2_arrow_initialPosition;
    public GameObject pfb_reactionTableEntry;

    public override void Start()
    {
        base.Start();

        m_vec2_arrow_initialPosition = m_image_arrow.rectTransform.anchoredPosition;
        m_arrow_initialWidth = m_image_arrow.rectTransform.sizeDelta.x;
        m_tableEntries = new List<GameObject>();
    }

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        //input mode
        m_text_inputMode.text = e.m_inputMode;

        //we neeed to reset the arrow..
        m_image_arrow.rectTransform.anchoredPosition = m_vec2_arrow_initialPosition;
        m_image_arrow.rectTransform.sizeDelta = new Vector2(m_arrow_initialWidth,
            m_image_arrow.rectTransform.sizeDelta.y);

        //response time
        StartCoroutine(MoveArrow());
        m_text_responseTime.text = e.m_responseTime.ToString("0.00") + " s";

        //reaction time
        StartCoroutine(ExpandRing());
        m_text_reactionTime.text = e.m_reaction_autoPace ? e.m_reactionTime.ToString("0.00") + " s" : "N/A";

        //speedometer
        //determine the speed factor
        m_speedFactor = (e.m_reaction_autoPace ? 10 : 5) - (e.m_reactionTime + e.m_responseTime);
        if (m_speedFactor < 0) m_speedFactor = 0;
        m_text_speedFactor.text = m_speedFactor.ToString("0.00");
        //find the angle change for the needle
        m_finalNeedleAngle = 180 - m_speedFactor / 10 * 180;
        StartCoroutine(MoveSpeedometer());

        //set the text for the direction of the test
        var dir = "";
        switch (e.m_reaction_orientation)
        {
            case TestOption_OrientationDir.RtoL:
                dir = "Right to Left";
                break;
            case TestOption_OrientationDir.LtoR:
                dir = "Left to Right";
                break;
            case TestOption_OrientationDir.TtoB:
                dir = "Top to Bottom";
                break;
            case TestOption_OrientationDir.BtoT:
                dir = "Bottom to Top";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        m_text_direction.text = "Direction: " + dir;
        m_text_trials.text = "Trials: " + e.m_numberOfTrials;

        m_graph_hitTimes.Show(e.m_averageHit, e.m_fastestHit, e.m_slowestHit);

        //fill the table with the individual times
        //nuke the current table if there is one
        m_tableEntries.ForEach(Destroy);
        m_tableEntries = new List<GameObject>();

        for (var x = 0; x < e.m_numberOfTrials; x++)
        {
            var entry = Instantiate(pfb_reactionTableEntry, m_tableContent);
            entry.transform.Find("Text_Trial").GetComponent<TextMeshProUGUI>().text = (x + 1).ToString();
            var times = e.m_reaction_trialTimes[x];
            entry.transform.Find("Text_Time").GetComponent<TextMeshProUGUI>().text =
                times.x.ToString("0.00") + " / " + times.y.ToString("0.00");
            m_tableEntries.Add(entry);
        }
    }


    private IEnumerator MoveArrow()
    {
        var initialWidth = m_image_arrow.rectTransform.sizeDelta.x;
        float i = 0;

        while (i < 1)
        {
            i += Time.deltaTime * m_arrowMoveSpeed;
            m_image_arrow.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(initialWidth, m_arrow_finalWidth, i),
                m_image_arrow.rectTransform.sizeDelta.y);

            //we need to keep the position at it's initial position, otherwise increasing the width does weird things to the rectTransform position
            m_image_arrow.rectTransform.anchoredPosition =
                new Vector2(
                    m_vec2_arrow_initialPosition.x + (m_image_arrow.rectTransform.sizeDelta.x - initialWidth) / 2,
                    m_vec2_arrow_initialPosition.y);

            yield return null;
        }
    }

    private IEnumerator ExpandRing()
    {
        var initialSize = m_image_targetRing.rectTransform.sizeDelta.x;
        float i = 0;

        while (i < 1)
        {
            var newSize = Mathf.Lerp(initialSize, m_finalRingSize, i);
            m_image_targetRing.rectTransform.sizeDelta = new Vector2(newSize, newSize);
            i += Time.deltaTime * m_ringSpeed;
            yield return null;
        }
    }

    private IEnumerator MoveSpeedometer()
    {
        float i = 0, initialNeedleAngle = 180;

        while (i < 1)
        {
            var newAngle = Mathf.Lerp(initialNeedleAngle, m_finalNeedleAngle, i);
            m_image_needle.rectTransform.rotation = Quaternion.Euler(0, 0, newAngle);
            i += Time.deltaTime * m_needleMoveSpeed;
            yield return null;
        }
    }
}