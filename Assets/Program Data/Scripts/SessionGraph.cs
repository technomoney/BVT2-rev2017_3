using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TMPro;
using Twity.DataModels.Core;
using UnityEngine;
using UnityEngine.UI;

public class SessionGraph : MonoBehaviour
{
    private RectTransform m_graphArea;
    public Transform m_keyEntryParent;
    public List<TextMeshProUGUI> m_list_yLabels, m_list_xLabels;
    public TextMeshProUGUI m_text_title;
    public Transform m_xLabelParent;
    public SessionGraphKeyEntry pfb_keyEntry;
    public LineSegment pfb_lineSegment;
    public GameObject pfb_smallBall;
    public TextMeshProUGUI pfb_xLabel;


    public void Initialize()
    {
        m_graphArea = transform.Find("Graph Area").GetComponent<RectTransform>();
    }

    private string GetTrainingName(int index)
    {
        switch (index)
        {
            case 0: return "Speed";
            case 1: return "Peripheral";
            case 2: return "Sequence";
            case 3: return "Reaction";
            case 4: return "Balance";
            case 5: return "Go/No-Go";
            case 6: return "Flash";
            case 7: return "Rhythm";
            case 8: return "Contrast";
            case 9: return "Multi";
            default: return "Test Name";
        }
    }

    public void PlotConfigs(List<List<PatientTrainingSummary>> configs, int trainingIndex)
    {
        m_text_title.text = GetTrainingName(trainingIndex);
        //we are given a list of lists, the top element is a config, and each cell is a summary in that config
        //[config][individualSummary]
        //well want to average and plot all summaries in a given config together

        //note when graphing the scores we'll deal with the shortScore element from the training summary
        //short score is a string cast from an int, with the following exceptions:
        //reaction/multi/contrast are cast from floats
        //balance only in dynamic mode is cast from a float

        //we need to determine which config has the most entries, this will tell us how many unique elements the
        //x axis will have (at most), this also allows everything to line up even if they don't have the same number
        //of entries
        var largestConfigCount = configs[0].Count;
        configs.ForEach(c => largestConfigCount = c.Count > largestConfigCount ? c.Count : largestConfigCount);

        //finally, we need to determine the highest score among all entries in all configs, this gives us our max Y axis
        //limit for getting the location of each data point
        //this is slightly annoying since the data type may change based on the test type being used..
        //we'll use a float here for the score, even though the data type may actually be an int.. for now..
        var highestScore = -1f;
        foreach (var config in configs)
        {
            var val = GetHighestScore(config);
            if (val > highestScore) highestScore = val;
        }

        var lowestScore = 1000f;
        foreach (var config in configs)
        {
            var val = GetLowestScore(config);
            if (val < lowestScore) lowestScore = val;
        }

        if (lowestScore >= highestScore)
            Debug.LogError("Problem determining high/low scores..");


        //now lets set our y axis labels
        //todo going to assume the lowest is always zero, but if we're ever going to have a negative short score we'll have to handle that here
        //axis label [0] is at the bottom and for now we'll set that to zero
        //m_list_yLabels[0].text = "0";
        //m_list_yLabels[1].text = Mathf.Lerp(0, highestScore, .25f).ToString("0.0");
        //m_list_yLabels[2].text = Mathf.Lerp(0, highestScore, .5f).ToString("0.0");
        //m_list_yLabels[3].text = Mathf.Lerp(0, highestScore, .75f).ToString("0.0");
        //m_list_yLabels[4].text = highestScore.ToString("0.0");
        var values = GetYaxisValues(ref highestScore, ref lowestScore);
        for (int x = 0; x < 5; x++)
            m_list_yLabels[x].text = values[x];

        //testing here..
        if (highestScore <= 0)
            Debug.LogError("Something went wrong getting highest score");

        var colorIndex = 0;
        //NOTE: this is all assuming a single day of reports, we'll have to do some other stuff with multi day...
        foreach (var config in configs) //this main loop goes through the configs
        {
            //this keeps track of which element of the list we're on for lerping the x position of the labels
            var elementIndex = 0;
            //each config will use a unique color
            var color = Window_SessionReport.Inst.m_colors[colorIndex];

            //this is needed for the line segments below, we have to keep track of all positions..
            var balls = new List<GameObject>();

            foreach (var summary in config) //this nested loop goes through each summary in the config
            {
                //todo making the x labels here will result in some duplicates, should do it before looping in the configs
                //like we do below in multi-day
                //first make an x axis label for this element
                var xLabel = Instantiate(pfb_xLabel, m_xLabelParent);

                //find the x position for this label
                //we do the half width thing here so the text lines up correctly with the ball we'll make and still
                //behaves with the left/center alignment of the transform
                var halfWidth = xLabel.GetComponent<RectTransform>().sizeDelta.x / 2;
                var posX = Mathf.Lerp(0 + halfWidth,
                    m_xLabelParent.GetComponent<RectTransform>().sizeDelta.x - halfWidth,
                    elementIndex / (float) largestConfigCount);

                //now actually set the position of the x label based on the above junk
                //the y position is set by the prefab and should not change!
                var rectT = xLabel.GetComponent<RectTransform>();
                rectT.anchoredPosition = new Vector2(posX, rectT.anchoredPosition.y);

                //set the x label text here which is just the number of the trial
                var count = elementIndex + 1;
                xLabel.text = count.ToString();

                //parse our score here from the summary short score, this try/catch may be redundant since we 
                //do this in GetHighestScore, but who knows...
                var score = -1f;
                try
                {
                    score = ConvertScore(summary.m_shortScore);
                }
                catch (FormatException)
                {
                    Debug.LogError("Problem parsing score to float...");
                }

                //the y position of our data point will be a lerp based on the score min/max
                //var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, score / highestScore);
                //when we lerp on the y axis we have to convert the calc to 0 - highest score using the modified high/low scores
                var modifiedHighest = highestScore - lowestScore;
                var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, (score - lowestScore) / modifiedHighest);


                //now make a data point ball at the above y location and the same x location as the x label
                var ball = Instantiate(pfb_smallBall, m_graphArea);
                ball.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                //set the color of the ball to the same as the line we're drawing now
                ball.GetComponent<Image>().color = color;
                balls.Add(ball);

                //make a new line segment if we need to
                //we'll make a connection between each ball and the previous one, so just skip element index 0
                if (elementIndex != 0)
                {
                    //we'll make a segment between the ball we just made and the previous one
                    var seg = Instantiate(pfb_lineSegment, m_graphArea);
                    var r1 = balls[elementIndex - 1].GetComponent<RectTransform>();
                    var r2 = balls[elementIndex].GetComponent<RectTransform>();
                    seg.Set(r1, r2, color);
                }

                //increment the working index 
                elementIndex++;
            }

            //every time we plot a config we have to make a new config entry with the matching color
            //then when this is clicked it will show the popup with the config details
            //it doesn't matter which summary in the config we get since they'll all be the same anyway..
            var entry = Instantiate(pfb_keyEntry, m_keyEntryParent);
            entry.Initialize("Config " + (colorIndex + 1), Window_SessionReport.Inst.m_colors[colorIndex], config[0]);

            colorIndex++;
        }
    }

    /// <summary>
    /// This plots all of the given tests across the given days on the same graph
    /// See PlotDays3 for plotting individual tests on their own graph across multiple days
    /// </summary>
    /// <param name="sortedResults"></param>
    public void PlotDays2(List<List<PatientTrainingSummary>>[] sortedResults)
    {
        //set the title
        m_text_title.text = "Multi Day Report";
        //sorted results is the full array of trainings grouped by type and config, but it doesn't consider the day
        //of training at all, so we'll have to handle that here

        //when we go through all of this we will more than one training type, we'll need to determine the dates
        //with data across all trainings, so make sure that's in the highest level of the sorted results loop 
        var trainingDates = new List<DateTime>();
        foreach (var training in sortedResults)
        {
            //we don't need to do anything with this training if there are no configs i.e. no data
            if (training.Count <= 0) continue;

            //first thing, we need to go through these results and determine every day for which we have a training, these will
            //be our x-axis data points
            foreach (var config in training)
            foreach (var summary in config)
            {
                //get the date of this training
                var date = summary.m_dateTime;
                var isUnique = true;
                //now add this to the list as long as this date isn't already in the list
                foreach (var d in trainingDates)
                {
                    if (d.Month != date.Month || d.Day != date.Day) continue;

                    isUnique = false;
                    break;
                }

                if (isUnique) trainingDates.Add(date);
            }
        }

        //make sure training dates are actually sorted by date...
        trainingDates = trainingDates.OrderBy(x => x.Date).ToList();

        //training dates now has a date entry for every date on which a training occurs across all trainings in the 
        //sorted results list

        //we'll now make the x-axis labels, this is a little different than with a single day graph which makes them in
        //the low loop but since we're going to iterate over the length of the x axis multiple times, we need to do
        //it here so it only occurs once
        var xAxisLabels = new List<TextMeshProUGUI>();
        var dateIndex = 0;
        foreach (var date in trainingDates)
        {
            //NOTE this code is identical to above when plotting a single day
            var xLabel = Instantiate(pfb_xLabel, m_xLabelParent);
            //find the x position for this label
            //we do the half width thing here so the text lines up correctly with the ball we'll make and still
            //behaves with the left/center alignment of the transform
            var halfWidth = xLabel.GetComponent<RectTransform>().sizeDelta.x / 2;
            var posX = Mathf.Lerp(0 + halfWidth,
                m_xLabelParent.GetComponent<RectTransform>().sizeDelta.x - halfWidth,
                dateIndex / (float) trainingDates.Count);

            //now actually set the position of the x label based on the above junk
            //the y position is set by the prefab and should not change!
            var rectT = xLabel.GetComponent<RectTransform>();
            rectT.anchoredPosition = new Vector2(posX, rectT.anchoredPosition.y);
            //now actually set the label
            xLabel.text = date.Month + "/" + date.Day;


            dateIndex++;
        }


        Debug.Log("Have " + trainingDates.Count + " unique training days");

        //NOTE this is identical to the code above for a single day
        //finally, we need to determine the highest score among all entries in all configs, this gives us our max Y axis
        //limit for getting the location of each data point
        //this is slightly annoying since the data type may change based on the test type being used..
        //we'll use a float here for the score, even though the data type may actually be an int.. for now..
        var highestScore = -1f;
        foreach (var training in sortedResults)
        foreach (var config in training)
        {
            var val = GetHighestScore(config);
            if (val > highestScore) highestScore = val;
        }

        //now lets set our y axis labels
        //todo going to assume the lowest is always zero, but if we're ever going to have a negative short score we'll have to handle that here
        //axis label [0] is at the bottom and for now we'll set that to zero
        m_list_yLabels[0].text = "0";
        m_list_yLabels[1].text = Mathf.Lerp(0, highestScore, .25f).ToString("0.0");
        m_list_yLabels[2].text = Mathf.Lerp(0, highestScore, .5f).ToString("0.0");
        m_list_yLabels[3].text = Mathf.Lerp(0, highestScore, .75f).ToString("0.0");
        m_list_yLabels[4].text = highestScore.ToString("0.0");


        //when we plot like this we don't actually care about the configs, all we care about is the day on which the 
        //training is done, and if multiples are done on the same day, we average them, so we'll rearrange the sortedResults
        //list to reflect this
        var dataByDate = new List<TrainingDate>();

        //so let's go through every training/config/summary and crunch it all down
        foreach (var training in sortedResults)
        foreach (var config in training)
        foreach (var summary in config)
        {
            //we want to add this score to the sum of the trainingDate object
            //see if we already have an entry with this training on this date
            var entry = dataByDate.Find(date => date.date.Day == summary.m_dateTime.Day &&
                                                date.date.Month == summary.m_dateTime.Month &&
                                                date.testType == summary.m_testType);
            if (entry != null)
            {
                entry.sum.Add(ConvertScore(summary.m_shortScore));
            }
            else
            {
                //we have to make a new entry in the list
                entry = new TrainingDate
                {
                    testType = summary.m_testType,
                    date = summary.m_dateTime
                };
                entry.sum.Add(ConvertScore(summary.m_shortScore));
                dataByDate.Add(entry);
            }
        }

        //we should now have a list of training date objects that have a series of scores per day, per test
        //ignoring configs and such, so we'll average these out
        dataByDate.ForEach(t => t.AverageScores());

        //finally we'll break these up into individual lists so we have a separate entry for each training
        var sortedDataByDate = new List<TrainingDate>[10];
        for (var x = 0; x < 10; x++)
            sortedDataByDate[x] = new List<TrainingDate>();
        foreach (var data in dataByDate)
            //we can cheat here and just use the int index of the test type enum to plop the data in the 
            //array which will automatically sort it by test type, yay
            sortedDataByDate[(int) data.testType].Add(data);

        //now we'll go through each training in the sortedData list, and plot everything we have
        for (var x = 0; x < 10; x++)
        {
            //each training will need it's own line to connect the data with
            //each config will use a unique color
            var color = Window_SessionReport.Inst.m_colors[x];

            var linePosIndex = 0;
            var balls = new List<GameObject>();
            foreach (var date in trainingDates)
            {
                var xIndex = trainingDates.IndexOf(date);
                var plotVal = 0f;
                //we'll plot the score for this date if we have one, if not, we do not want to plot it
                //lets see if the training we're working through now has an entry for this date
                var entry = sortedDataByDate[x].Find(t => t.IsSameDayMonth(date));
                if (entry == null) continue;

                plotVal = entry.score;

                //now plot this on the graph under the current day
                //the y position of our data point will be a lerp based on the score min/max
                var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, plotVal / highestScore);
                //now make a data point ball at the above y location and the same x location as the x label
                var ball = Instantiate(pfb_smallBall, m_graphArea);
                var posX = xAxisLabels[xIndex].rectTransform.anchoredPosition.x;
                ball.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                //set the color of the ball to the same as the line we're drawing now
                ball.GetComponent<Image>().color = color;
                balls.Add(ball);

                //make our line segments
                if (linePosIndex != 0)
                {
                    //we'll make a segment between the ball we just made and the previous one
                    var seg = Instantiate(pfb_lineSegment, m_graphArea);
                    var r1 = balls[linePosIndex - 1].GetComponent<RectTransform>();
                    var r2 = balls[linePosIndex].GetComponent<RectTransform>();
                    seg.Set(r1, r2, color);
                }

                linePosIndex++;
            }
        }

        //finally, we'll make a key entry for each training that has at least one entry on the graph, but unlike single day plotting
        //clicking on the key won't do anything so the Pushed() event can stay null
        var trainingIndex = 0;
        foreach (var training in sortedResults)
        {
            if (training != null && training.Count > 0)
            {
                var key = Instantiate(pfb_keyEntry, m_keyEntryParent);
                key.Initialize(GetTrainingName(trainingIndex), Window_SessionReport.Inst.m_colors[trainingIndex], null);
            }

            trainingIndex++;
        }
    }

    public void PlotDays3(List<List<PatientTrainingSummary>> training, int trainingIndex)
    {
        //set the title
        m_text_title.text = GetTrainingName(trainingIndex) + " Multi Day Report";

        //if we have no data for this training, then just continue to the next one
        if (training.Count <= 0) return;

        //let's get the dates we have data for in this test
        var trainingDates = new List<DateTime>();

        //first thing, we need to go through these results and determine every day for which we have a training, these will
        //be our x-axis data points
        foreach (var config in training)
        foreach (var summary in config)
        {
            //get the date of this training
            var date = summary.m_dateTime;
            var isUnique = true;
            //now add this date to the list as long as this date isn't already in the list
            foreach (var d in trainingDates)
            {
                if (d.Month != date.Month || d.Day != date.Day) continue;

                isUnique = false;
                break;
            }

            if (isUnique) trainingDates.Add(date);
        }

        //make sure training dates are actually sorted by date...
        trainingDates = trainingDates.OrderBy(x => x.Date).ToList();

        //training dates now has a date for every date on which a training occurs across the current test

        //we'll now make the x axis labels for this chart, each of which is a date
        var xAxisLabels = new List<TextMeshProUGUI>();
        var dateIndex = 0;
        foreach (var date in trainingDates)
        {
            //NOTE this code is identical to above when plotting a single day
            var xLabel = Instantiate(pfb_xLabel, m_xLabelParent);
            //find the x position for this label
            //we do the half width thing here so the text lines up correctly with the ball we'll make and still
            //behaves with the left/center alignment of the transform
            var halfWidth = xLabel.GetComponent<RectTransform>().sizeDelta.x / 2;
            var posX = Mathf.Lerp(0 + halfWidth,
                m_xLabelParent.GetComponent<RectTransform>().sizeDelta.x - halfWidth,
                dateIndex / (float) trainingDates.Count);

            //now actually set the position of the x label based on the above junk
            //the y position is set by the prefab and should not change!
            var rectT = xLabel.GetComponent<RectTransform>();
            rectT.anchoredPosition = new Vector2(posX, rectT.anchoredPosition.y);
            //now set the label
            xLabel.text = date.Month + "/" + date.Day;

            xAxisLabels.Add(xLabel);
            dateIndex++;
        }

        //we need to determine the highest score among entries in all configs for this test, this gives us
        //our max Y axis limit for getting the location of each data point
        //this is slightly annoying since the data type may change based on the test type being used..
        //we'll use a float here for the score, even though the data type may actually be an int.. for now..
        var highestScore = -1f;
        foreach (var config in training)
        {
            var val = GetHighestScore(config);
            if (val > highestScore) highestScore = val;
        }

        //if (highestScore <= 0)
            //Debug.LogWarning("Something may have gone wrong getting highest score...");
            var lowestScore = 1000f;
            foreach (var config in training)
            {
                var val = GetLowestScore(config);
                if (val < lowestScore) lowestScore = val;
            }

            if (lowestScore >= highestScore)
                Debug.LogError("Problem determining high/low scores..");

        //now lets set our y axis labels
        //todo going to assume the lowest is always zero, but if we're ever going to have a negative short score we'll have to handle that here
        //axis label [0] is at the bottom and for now we'll set that to zero
        //m_list_yLabels[0].text = "0";
        //m_list_yLabels[1].text = Mathf.Lerp(0, highestScore, .25f).ToString("0.0");
        //m_list_yLabels[2].text = Mathf.Lerp(0, highestScore, .5f).ToString("0.0");
        //m_list_yLabels[3].text = Mathf.Lerp(0, highestScore, .75f).ToString("0.0");
        //m_list_yLabels[4].text = highestScore.ToString("0.0");
        
        var values = GetYaxisValues(ref highestScore, ref lowestScore);
        for (int x = 0; x < 5; x++)
            m_list_yLabels[x].text = values[x];

        //testing here..
        if (highestScore <= 0)
            Debug.LogError("Something went wrong getting highest score");
        

        //when we plot like this we don't actually care about the configs, all we care about is the day on which the 
        //training is done, and if multiple tests are done on the same day, we'll average them,
        //so we'll rearrange the sortedResults list to reflect this
        var dataByDate = new List<TrainingDate>();

        //so let's go through every config/summary and crunch it all down
        foreach (var config in training)
        foreach (var summary in config)
        {
            //we want to add this score to the sum of the trainingDate object
            //see if we already have an entry with this training on this date
            var entry = dataByDate.Find(date => date.date.Day == summary.m_dateTime.Day &&
                                                date.date.Month == summary.m_dateTime.Month &&
                                                date.testType == summary.m_testType);
            if (entry != null)
            {
                entry.sum.Add(ConvertScore(summary.m_shortScore));
            }
            else
            {
                //we have to make a new entry in the list
                entry = new TrainingDate
                {
                    testType = summary.m_testType,
                    date = summary.m_dateTime
                };
                entry.sum.Add(ConvertScore(summary.m_shortScore));
                dataByDate.Add(entry);
            }
        }

        //we should now have a list of training date objects that have a series of scores per day
        //ignoring configs and such, so we'll average these out
        dataByDate.ForEach(t => t.AverageScores());

        //all we have to do now is plot the data by date
        var linePosIndex = 0;
        var balls = new List<GameObject>();

        //for now we'll just use default color for all tests...
        var color = Window_SessionReport.Inst.m_colors[0];
        foreach (var date in trainingDates)
        {
            var xIndex = trainingDates.IndexOf(date);
            var plotVal = 0f;
            //we'll plot the score for this date if we have one, if not, we do not want to plot it
            //lets see if the training we're working through now has an entry for this date
            var entry = dataByDate.Find(t => t.IsSameDayMonth(date));
            if (entry == null) continue;

            plotVal = entry.score;

            //now plot this on the graph under the current day
            //the y position of our data point will be a lerp based on the score min/max
            //var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, plotVal / highestScore);
            
            //the y position of our data point will be a lerp based on the score min/max
            //var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, score / highestScore);
            //when we lerp on the y axis we have to convert the calc to 0 - highest score using the modified high/low scores
            var modifiedHighest = highestScore - lowestScore;
            var posY = Mathf.Lerp(0, m_graphArea.sizeDelta.y, (plotVal - lowestScore) / modifiedHighest);
            
            //now make a data point ball at the above y location and the same x location as the x label
            var ball = Instantiate(pfb_smallBall, m_graphArea);
            var posX = xAxisLabels[xIndex].rectTransform.anchoredPosition.x;
            ball.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            //set the color of the ball to the same as the line we're drawing now
            ball.GetComponent<Image>().color = color;
            balls.Add(ball);

            //make our line segments
            if (linePosIndex != 0)
            {
                //we'll make a segment between the ball we just made and the previous one
                var seg = Instantiate(pfb_lineSegment, m_graphArea);
                var r1 = balls[linePosIndex - 1].GetComponent<RectTransform>();
                var r2 = balls[linePosIndex].GetComponent<RectTransform>();
                seg.Set(r1, r2, color);
            }

            linePosIndex++;
        }
    }


    /// <summary>
    ///     Get the highest value of short score, parsed as a float, among all of the summaries in the given list
    /// </summary>
    /// <returns>float of the highest value, -1 if format exception was thrown...</returns>
    private float GetHighestScore(List<PatientTrainingSummary> config)
    {
        var highest = 0.0f;

        foreach (var summary in config)
        {
            //cast our short score to a float, regardless of the actual original data type an int will cast to a float fine
            var val = ConvertScore(summary.m_shortScore);
            if (val > highest) highest = val;
        }

        return highest;
    }

    private float GetLowestScore(List<PatientTrainingSummary> config)
    {
        var lowest = 1000f;

        foreach (var summary in config)
        {
            //cast our short score to a float, regardless of the actual original data type an int will cast to a float fine
            var val = ConvertScore(summary.m_shortScore);
            if (val < lowest) lowest = val;
        }

        return lowest;
    }

    /// <summary>
    ///     get the number of unique dates present for all summaries in this config
    ///     This is functionally identical to getting the dates present in all configs when evaluating a
    ///     training for plotting, but here we only care about the number of distinct dates, not the dates
    ///     themselves
    /// </summary>
    private int GetUniqueDateCountForConfig(List<PatientTrainingSummary> config)
    {
        var dates = new List<DateTime>();
        foreach (var summary in config)
        {
            //get the date of this training
            var date = summary.m_dateTime;
            var isUnique = true;
            //now add this to the list as long as this date isn't already in the list
            foreach (var d in dates)
            {
                if (d.Month != date.Month || d.Day != date.Day) continue;

                isUnique = false;
                break;
            }

            if (isUnique) dates.Add(date);
        }

        return dates.Count;
    }

    /// <summary>
    ///     Get the number of days that a training occurs for this list
    /// </summary>
    private int GetNumberOfTrainingDays(List<List<PatientTrainingSummary>> training)
    {
        var dates = new List<DateTime>();
        foreach (var config in training)
        foreach (var summary in config)
        {
            //get the date of this training
            var date = summary.m_dateTime;
            var isUnique = true;
            //now add this to the list as long as this date isn't already in the list
            foreach (var d in dates)
            {
                if (d.Month != date.Month || d.Day != date.Day) continue;

                isUnique = false;
                break;
            }

            if (isUnique) dates.Add(date);
        }

        return dates.Count;
    }

    private float ConvertScore(string shortScore)
    {
        float val;
        try
        {
            if (shortScore.Contains('%'))
                //this should take care of the contrast score having the % in it..
                val = float.Parse(shortScore.Substring(0, shortScore.Length - 1));
            else val = float.Parse(shortScore);
        }
        catch (FormatException)
        {
            Debug.LogError("Problem converting short score");
            return 0;
        }

        return val;
    }

    /// <summary>
    /// determine the values for the y axis so everything looks nice with whole numbers
    /// </summary>
    private string[] GetYaxisValues(ref float highest, ref float lowest)
    {
        //get the range
        var range = highest - lowest;
        string[] values = {"", "", "", "", ""};
        var step = 0;
        var rng = 0;
        int high = -1000;
        int low = 1000;
        //the range must be even to evenly label the y axis assuming we have 5 axis elements
        //there are a couple of situations here depending on what the range it

        if (rng == 5)
        {
            //if the range is exactly 5 then we're in luck, we can just label each axis element as steps along the range
            step = 1;
        }
        else if (rng < 8)
        {
            //so we have to handle ranges between 1 - 7 (excluding 5) here
            //8 is the weird cutoff where anything less that isn't exactly 5 will be an edge case so we can handle that here
            //the numbers are also too small to really be able to add anything to the high/low to even them out so we'll just
            //do this the old way
            values[0] = lowest.ToString();
            values[1] = Mathf.Lerp(lowest, highest, .25f).ToString("0.0");
            values[2] = Mathf.Lerp(lowest, highest, .5f).ToString("0.0");
            values[3] = Mathf.Lerp(lowest, highest, .75f).ToString("0.0");
            values[4] = highest.ToString();
            return values;
        }
        else
        {
            //if we have a range of at least 8 then we can handle our high/low being converted to whole numbers if they aren't already
            var dec = highest - (int) highest;
            //if we have any decimal component left over, just round to the next whole number
            high = dec > 0 ? (int) highest + 1 : (int) highest;
            dec = lowest - (int) lowest;
            low = dec > 0 ? (int) lowest - 1 : (int) lowest;
            //now get a new range with these numbers
            rng = high - low;
            
            //this is the normal range were range must be even and we might have to adjust it a bit to make everything fit nicely
            if (rng % 2 != 0)
                //if range isn't even then we need to modify the highest to extend the range so it is even
                //this may not make a whole lot of sense if the values are skewed to the lower value, but todo
            {
                high++;
                rng = high - low;
            }


            //we now have an even range and we can find our step value to evenly space things on the axis
            //to have even steps on an axis of 5 elements, the range must be divisible by 4
            var up = true;
            while (rng % 4 != 0)
            {
                if (up)
                    high++;
                else low--;

                up = !up;
                rng = high - low;
            }

            //we should now have everything we need, so divide our range by 4 to get our step
            step = rng / 4;
            highest = high;
            lowest = low;
        }

        //and return our answer
        values[0] = low.ToString();
        values[1] = (low + step).ToString();
        values[2] = (low + step * 2).ToString();
        values[3] = (low + step * 3).ToString();
        values[4] = high.ToString();
        return values;
    }
}

internal class TrainingDate
{
    public DateTime date;
    public float score;
    public List<float> sum;
    public TestType testType;

    public TrainingDate()
    {
        sum = new List<float>();
    }

    public void AverageScores()
    {
        score = sum.Average();
    }

    public bool IsSameDayMonth(DateTime d)
    {
        return d.Day == date.Day && d.Month == date.Month;
    }
}