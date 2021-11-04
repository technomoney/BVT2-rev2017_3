using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSummary_Memory : TrainingSummary_Base
{
    public Color m_color_targetHit, m_color_targetMissed;
    public Image[] m_dotImages;

    /// <summary>
    ///     Time, in seconds, to finish animating the dots
    /// </summary>
    public float m_dotRevealTime;

    public Image m_image_x;

    //starting level elements needed only for flash
    public GameObject m_obj_startingLevel;
    public WMG_Series m_series_levelTimes;
    public TextMeshProUGUI m_text_hitsAtHighestLevel;

    public TextMeshProUGUI m_text_level, m_text_hits;

    public TextMeshProUGUI[] m_text_levelLabels;
    public TextMeshProUGUI m_text_startingLevel;
    public Transform m_trans_levelTimes_dataLabelParent;

    /// <summary>
    ///     The positions to place the X for any given level that failed
    /// </summary>
    public Vector2[] m_vec2_levelXPositions;

    private int showingIndex, hitsAtHighestLevel, highestLevel;
    private float waitTime;

    public override void Show(PatientTrainingSummary e)
    {
        base.Show(e);

        //initially, hide all of the bubbles
        foreach (var t in m_dotImages)
            t.gameObject.SetActive(false);

        showingIndex = 0;
        waitTime = m_dotRevealTime / e.m_highestlevel;
        highestLevel = e.m_highestlevel;
        hitsAtHighestLevel = e.m_hitsOnHighestLevel;

        //only show the starting level if this is flash
        m_obj_startingLevel.SetActive(e.m_testType == TestType.Flash);
        if (e.m_testType == TestType.Flash) m_text_startingLevel.text = e.m_startingLevel.ToString();

        m_text_hits.text = e.m_totalHits.ToString();
        m_text_level.text = highestLevel.ToString();

        StartCoroutine(ShowDots());


        //make level times graph
        //what we do here depends on how many entries we have, we have 6 entries max, meaning that's the highest level we'll
        //show, so if we have more than that in the list, we need to chop it down
        if (e.m_list_leveltimes.Count > 6)
        {
            var newList = e.m_list_leveltimes.GetRange(e.m_list_leveltimes.Count - 6, 6);
            e.m_list_leveltimes = newList;
        }

        //now set all of the bars to zero, and set their point color to invisible in case they aren't used
        for (var x = 0; x < 6; x++)
        {
            m_series_levelTimes.pointValues[x] = new Vector2(0, 0);
            m_series_levelTimes.pointColors[x] = new Color(m_series_levelTimes.pointColors[x].r,
                m_series_levelTimes.pointColors[x].g, m_series_levelTimes.pointColors[x].b, 0);
        }

        //now we'll want to fill the bars, they fill backwards, so bar 6 (index 5) is actually element 0 of the list
        //also, make the color visible
        var index = 0;
        foreach (var f in e.m_list_leveltimes)
        {
            m_series_levelTimes.pointValues[5 - index] = new Vector2(1, f);
            m_series_levelTimes.pointColors[5 - index] = new Color(m_series_levelTimes.pointColors[5 - index].r,
                m_series_levelTimes.pointColors[5 - index].g, m_series_levelTimes.pointColors[5 - index].b, 1);
            index++;
        }

        //initially make all of the level labels invisible
        for (var x = 0; x < 6; x++)
            m_text_levelLabels[x].color = new Color(m_text_levelLabels[x].color.r, m_text_levelLabels[x].color.g,
                m_text_levelLabels[x].color.b, 0);

        //now set and make visible the required labels
        index = 0;
        var level = e.m_levelsCompleted <= 6 ? e.m_startingLevel : e.m_highestlevel - 6;
        foreach (var f in e.m_list_leveltimes)
        {
            m_text_levelLabels[index].color = new Color(m_text_levelLabels[index].color.r,
                m_text_levelLabels[index].color.g,
                m_text_levelLabels[index].color.b, 1);
            m_text_levelLabels[index].text = level.ToString("0");
            level++;
            index++;
        }


        //set the data labels for each entry
        //initially just set each label to "" in case any of them end up being unused
        var dataLabels = m_trans_levelTimes_dataLabelParent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var t in dataLabels)
            t.text = "";

        index = 0;
        foreach (var f in e.m_list_leveltimes)
        {
            dataLabels[5 - index].text = f.ToString("0.00") + " s";
            index++;
        }

        //place the X indicating the level that was failed
        m_image_x.GetComponent<RectTransform>().anchoredPosition =
            m_vec2_levelXPositions[e.m_list_leveltimes.Count - 1];

        //write the string for hits at highest level
        m_text_hitsAtHighestLevel.text = "Hits at Highest Level: " + e.m_hitsOnHighestLevel + "/" + e.m_highestlevel;
    }

    private IEnumerator ShowDots()
    {
        while (showingIndex < highestLevel)
        {
            yield return new WaitForSeconds(waitTime);
            m_dotImages[showingIndex].gameObject.SetActive(true);
            //by default, show them as not filled
            m_dotImages[showingIndex].color = m_color_targetMissed;
            showingIndex++;
        }

        showingIndex = 0;
        StartCoroutine(FillDots());
    }

    private IEnumerator FillDots()
    {
        while (showingIndex < hitsAtHighestLevel)
        {
            yield return new WaitForSeconds(waitTime);

            m_dotImages[showingIndex].color = m_color_targetHit;
            showingIndex++;
        }
    }
}