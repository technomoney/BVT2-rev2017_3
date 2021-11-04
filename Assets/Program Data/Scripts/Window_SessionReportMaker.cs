using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class Window_SessionReportMaker : BVT_Window
{
    public static Window_SessionReportMaker Inst;
    public DatePicker_DateRange m_date_range;
    public DatePicker m_date_single;
    private List<DateTime> m_dateTime_multi;

    private DateTime m_dateTime_single;
    public TMP_InputField[] m_input_singleDay, m_input_multiDay_start, m_input_multiDay_end;
    private Patient m_showingPatient;
    public Toggle m_toggle_single, m_toggle_multi;

    public List<Toggle> m_toggles_trainings;
    private List<string> m_trainingTypes;
    public bool testing_doNotClearFields;


    private void Awake()
    {
        Inst = this;
    }

    public override void Hide()
    {
        m_date_single.Hide();
        m_date_range.Hide();
        base.Hide();
    }

    public override void Start()
    {
        base.Start();

        transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(Hide);
        transform.Find("Button_Generate").GetComponent<Button>().onClick.AddListener(MakeReport);

        transform.Find("Trainings").Find("Button_All").GetComponent<Button>().onClick.AddListener(SetAllToggles);
        transform.Find("Trainings").Find("Button_None").GetComponent<Button>().onClick.AddListener(ClearAllToggles);

        m_date_single.Config.Events.OnDaySelected.AddListener(SingleDayChosen);
        m_date_range.Config.Events.OnDaySelected.AddListener(MultiDateChosen);

        //select our default option on the date buttons
        //this is a placeholder in case we want to use our quick date buttons again
        //var buttons = GetComponent<ButtonGroup>();
        //buttons.event_buttonPushed += DateButtonPushed;
        //buttons.InvokeDefault();
    }

    public void Show(Patient showingPatient)
    {
        base.Show();
        m_showingPatient = showingPatient;

        //clear all of our fields and reset everything
        if (!testing_doNotClearFields)
            for (var x = 0; x < 3; x++)
            {
                m_input_singleDay[x].text = string.Empty;
                m_input_multiDay_start[x].text = string.Empty;
                m_input_multiDay_end[x].text = string.Empty;
            }

        m_toggle_single.isOn = true;
        ClearAllToggles();
        m_dateTime_single = new DateTime(1900, 1, 1);
        m_dateTime_multi = new List<DateTime>();

        //set the single day to today by default
        m_date_single.SelectedDate = DateTime.Today;
        SingleDayChosen(DateTime.Today);
        
        //set the multi to three months ago and then today
        m_date_range.Ref_DatePicker_To.SelectedDate = DateTime.Today;
        MultiDateChosen(DateTime.Today);
        m_date_range.Ref_DatePicker_From.SelectedDate = DateTime.Today.AddDays(-90);
        MultiDateChosen(DateTime.Today.AddDays(-90));
    }

    private void ClearAllToggles()
    {
        m_toggles_trainings.ForEach(t => t.isOn = false);
    }

    private void SetAllToggles()
    {
        m_toggles_trainings.ForEach(t => t.isOn = true);
    }

    private void DateButtonPushed(int index)
    {
    }

    private void MakeReport()
    {
        //make sure we have a valid date for the report we're generating
        var singleDay = m_toggle_single.isOn;

        if (singleDay)
        {
            if (!VerifyDateEntry(m_dateTime_single))
            {
                Window_GenericMessage.Inst.Show("Date Entry Error", "Please enter a valid date");
                return;
            }
        }

        //so getting here we can assume we have a reasonable date, now make sure we have at least one training selected
        //populate our list of trainings here
        m_trainingTypes = new List<string>();
        //we need to check each one..
        if (m_toggles_trainings[0].isOn) m_trainingTypes.Add("Speed");
        if (m_toggles_trainings[1].isOn) m_trainingTypes.Add("Peripheral");
        if (m_toggles_trainings[2].isOn) m_trainingTypes.Add("Sequence");
        if (m_toggles_trainings[3].isOn) m_trainingTypes.Add("Reaction");
        if (m_toggles_trainings[4].isOn) m_trainingTypes.Add("Balance");
        if (m_toggles_trainings[5].isOn) m_trainingTypes.Add("GoNoGo");
        if (m_toggles_trainings[6].isOn) m_trainingTypes.Add("Flash");
        if (m_toggles_trainings[7].isOn) m_trainingTypes.Add("Rhythm");
        if (m_toggles_trainings[8].isOn) m_trainingTypes.Add("Contrast");
        if (m_toggles_trainings[9].isOn) m_trainingTypes.Add("Multi");

        if (m_trainingTypes == null || m_trainingTypes.Count <= 0)
        {
            Window_GenericMessage.Inst.Show("Training Selection Error", "Please select at least one training");
            return;
        }

        //make sure we have the patient from the patient detail window
        if (m_showingPatient == null)
        {
            Debug.LogError("showing patient is null for report maker...");
            return;
        }

        //so we have valid selections, now we need to search through this patients reports and see if any match
        //the formatting for files name is TRAININGNAME_DAY_MONTH_YEAR_HOUR_MIN_SECOND
        var dirInfo = new DirectoryInfo(Manager_Patient.Instance.GetPatientDirectory(m_showingPatient));
        //get all files
        var files = dirInfo.GetFiles();

        var matchingTrainings = new List<FileInfo>();

        for (var x = 0; x < files.Length; x++)
        {
            //we don't want to try and read the patient info, so we'll skip that one
            if (files[x].Name.Equals("patientInfo.xml")) continue;
            //we don't want to try and read the data files either, so skip those
            if (files[x].Name.ToLower().Contains("data")) continue;

            //so here we presumably have a training summary report
            //split it by '_' to get individual components of the filename
            var parts = files[x].Name.Split('_');
            if (parts.Length <= 0)
            {
                Debug.LogError("0/null parts after splitting file name for report maker..");
                return;
            }

            //element 0 is the training type so see if it matches anything in our training types list
            if (m_trainingTypes.Contains(parts[0]))
                matchingTrainings.Add(files[x]); //this is a match for the training type, so add it to the list
        }

        if (matchingTrainings.Count <= 0)
        {
            NoMatchesFound();
            return;
        }

        //now we have a list of files that match all training types from our selected types
        //sort it by type(name)
        // ReSharper disable once StringCompareToIsCultureSpecific
        matchingTrainings.Sort((x, y) => x.Name.CompareTo(y.Name));
        Debug.Log("Got " + matchingTrainings.Count + " matches by training type");
        //now we can go through this list and see what matches by the date or date range

        var month = 0;
        var day = 0;
        var year = 0;
        var monthEnd = 0;
        var dayEnd = 0;
        var yearEnd = 0;

        //if we're doing multi we'll arrange the list so that element 0 is the earlier date
        if (!singleDay)
        {
            //m_dateTime_multi = m_dateTime_multi.OrderBy(x => x.Date).ToList();
            try
            {
                month = m_date_range.Ref_DatePicker_From.SelectedDate.Date.Month; //m_dateTime_multi[0].Month;
                monthEnd = m_date_range.Ref_DatePicker_To.SelectedDate.Date.Month; //m_dateTime_multi[1].Month;
                day = m_date_range.Ref_DatePicker_From.SelectedDate.Date.Day; //m_dateTime_multi[0].Day;
                dayEnd = m_date_range.Ref_DatePicker_To.SelectedDate.Date.Day; //m_dateTime_multi[1].Day;
                year = m_date_range.Ref_DatePicker_From.SelectedDate.Date.Year; //m_dateTime_multi[0].Year;
                yearEnd = m_date_range.Ref_DatePicker_To.SelectedDate.Date.Year; //m_dateTime_multi[1].Year;
            }
            catch (FormatException)
            {
                Window_GenericMessage.Inst.Show("Date Entry Error", "Please enter a valid date");
                return;
            }
        }
        else
        {
            month = m_dateTime_single.Month;
            day = m_dateTime_single.Day;
            year = m_dateTime_single.Year;
        }

        var matchingFiles = new List<FileInfo>();
        //todo may want to be smarter how we're parsing the text to int here, could probably be done earlier so it only needs to be done once...
        foreach (var t in matchingTrainings)
        {
            var parts = t.Name.Split('_');
            //we'll want to parse the file components to ints so we can compare them below
            var fileDay = 0;
            var fileMonth = 0;
            var fileYear = 0;
            try
            {
                //elements 1/2/3 = day/month/year
                fileDay = int.Parse(parts[1]);
                fileMonth = int.Parse(parts[2]);
                fileYear = int.Parse(parts[3]);
            }
            catch (FormatException)
            {
                Debug.LogError("Problem parsing file dates to int");
                return;
            }

            if (singleDay)
            {
                //single day must match exactly
                if (!fileMonth.Equals(month) && !("0" + fileMonth).Equals(month)) continue;
                if (!fileDay.Equals(day) && !("0" + fileDay).Equals(day)) continue;
                if (!fileYear.Equals(year)) continue;
                matchingFiles.Add(t);
            }
            else
            {
                //multi day just has to be in the range
                //this is slightly more annoying since we can have the end of one month
                //and the beginning of another being 1 day apart but 30 digits apart (31-1 = 30)
                //so we have to convert everything to a date time and use the built in comparators 
                DateTime start, end, fileDate;
                try
                {
                    //we also have to add 2000 to the ints we're parsing below so the dates match
                    fileDate = new DateTime(fileYear, fileMonth, fileDay);
                    start = new DateTime(year, month, day);
                    end = new DateTime(yearEnd, monthEnd, dayEnd);
                }
                catch (Exception)
                {
                    Debug.LogError("Problem parsing dates to date time type..");
                    return;
                }

                //todo we should check if we have a valid range selected before we get this far...
                //now get our range from start to finish
                var range = end.Subtract(start);
                //now get the difference from the date we're checking to the end date
                var diff = end.Subtract(fileDate);

                //Debug.Log("Range: " + range.Days + "  Diff: " + diff.Days);

                //finally compare them 
                if (diff.Days < 0 || diff.Days > range.Days) continue;

                matchingFiles.Add(t);
            }
        }

        if (matchingFiles.Count <= 0)
        {
            NoMatchesFound();
            return;
        }

        Debug.Log("Found " + matchingFiles.Count + " reports matching date and trainings");

        //todo for a given day/set we need at least two trials to have any meaningful graph..

        //now that we have our matches we can compile them into lists for the session graph to handle
        //we're going to have x matching files that are of the multiple test types on different days and with different
        //settings, so a given 'config' for a test type will have common unique setting for that test
        //Things that don't modify a config:
        //background types within a given subset of backgrounds (video/picture/solid are the three subsets)
        //Target type/audio settings/countdown settings do not modify a config
        //other than those settings, any changes will place that test into a unique 'config'
        //currently we will only show 4 unique configs per graph, but that would change

        //we'll have an array of lists, one element in the array for each training type, in menu order,
        //and the lists themselves for the files we're deserializing
        var summaries = new List<PatientTrainingSummary>[10];
        for (var x = 0; x < 10; x++)
            summaries[x] = new List<PatientTrainingSummary>();

        //the matching files list we have is FileInfo, so we'll need to parse those into patient training objects
        //this code is identical to that in Window_PatientDetail
        foreach (var match in matchingFiles)
        {
            //we'll try to deserialize every file in to a patientSummary
            PatientTrainingSummary summary = null;

            var reader = new XmlSerializer(typeof(PatientTrainingSummary));
            var file = new StreamReader(match.FullName);
            summary = (PatientTrainingSummary) reader.Deserialize(file);
            file.Close();
            summary.m_fileName = match.Name;

            //todo should have some kind of check here to make sure this succeeded

            var summaryIndex = -1;
            //now plop this summary into the correct list in the summary array
            switch (summary.m_testType)
            {
                case TestType.Speed:
                    summaryIndex = 0;
                    break;
                case TestType.Peripheral:
                    summaryIndex = 1;
                    break;
                case TestType.Sequence:
                    summaryIndex = 2;
                    break;
                case TestType.Reaction:
                    summaryIndex = 3;
                    break;
                case TestType.Balance:
                    summaryIndex = 4;
                    break;
                case TestType.GoNoGo:
                    summaryIndex = 5;
                    break;
                case TestType.Flash:
                    summaryIndex = 6;
                    break;
                case TestType.Rhythm:
                    summaryIndex = 7;
                    break;
                case TestType.Contrast:
                    summaryIndex = 8;
                    break;
                case TestType.Multi:
                    summaryIndex = 9;
                    break;
                case TestType.All:
                    summaryIndex = -1;
                    break;
                case TestType.None:
                    summaryIndex = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (summaryIndex == -1)
            {
                Debug.LogError("Something went wrong getting test type from SessionReportMaker");
                return;
            }

            //add the summary we just deserialized to the correct test cell of the array as determined above
            summaries[summaryIndex].Add(summary);
        }

        //so now we have out array of lists separated by test type
        //within each list we have to arrange those tests by config (see above)

        //sorted results is an array of 10 elements, one for each test type, that contains a list of lists,
        //each list will be a different config for the given test type
        var sortedResults = new List<List<PatientTrainingSummary>>[10];
        for (var x = 0; x < 10; x++)
            sortedResults[x] = new List<List<PatientTrainingSummary>>();

        //sorted results looks like this: [test type][config number][summary]
        //SortedResults
        //[0]
        //   List of configs
        //			[0] config 0
        //					summary0
        //					summary1
        //			[1] config 1
        //					summary0
        //			[2] config 2
        //					summary0
        //[1]
        //...... and so on
        //we'll go through our deserialized summaries one test type at a time, determine the configs and add them
        //to our final sorted results list

        for (var testType = 0; testType < 10; testType++)
        {
            //don't need to do anything if there aren't any summaries for a given test type
            if (summaries[testType].Count <= 0) continue;

            //we'll put the first element in its own config automatically since there won't be any to compare to yet..
            sortedResults[testType].Add(new List<PatientTrainingSummary> {summaries[testType][0]});

            //now go through the rest of the summaries in this test type and see if the match an existing config,
            //or if they need to be put in to a new one
            for (var summaryNumber = 1; summaryNumber < summaries[testType].Count; summaryNumber++)
            {
                var checkingSummary = summaries[testType][summaryNumber];

                //see if this summary matches a config we already have for this test type
                var foundMatchingConfig = false;
                foreach (var config in sortedResults[testType])
                {
                    //we only have to compare this summary to the first element in each config since, presumably,
                    //all summaries in a given config should be the same...
                    if (!config[0].CheckForMatchingConfig(checkingSummary)) continue;

                    foundMatchingConfig = true;
                    config.Add(checkingSummary);
                    break;
                }

                //if we've already added this to a config, then continue to the next summary
                if (foundMatchingConfig) continue;

                //getting here means we didn't find a config, so we have to make a new one
                sortedResults[testType].Add(new List<PatientTrainingSummary> {checkingSummary});
            }
        }

        //now we should have a really complicated structure with each summary sorted by test type and paired with like configs
        //so we can send each test type over to a graph

        //testing
        var s = "Configs per Training (summaries per config)\n";
        foreach (var t in sortedResults)
        {
            s += t.Count + " ( ";
            foreach (var config in t) s += config.Count + "/";

            s += ")   ";
        }

        Debug.Log(s);

        //testing
        s = "Scores per config\n";
        //for peripheral only....
        foreach (var c in sortedResults[1])
        {
            foreach (var score in c)
                s += score.m_shortScore + "/";
            s += "\n";
        }

        Debug.Log(s);

        //so lets make a session graph for each training that has configs
        if (singleDay)
        {
            Window_SessionReport.Inst.Show();
            Window_SessionReport.Inst.MakeGraph_SingleDay(sortedResults);
        }
        else
        {
            Window_SessionReport.Inst.Show();
            Window_SessionReport.Inst.MakeGraph_MultiDay(sortedResults);
        }
        
        //then after we make the report we'll close this window
        Hide();
    }

    private void SingleDayChosen(DateTime date)
    {
        //when we select a date we'll manually check the box
        m_toggle_single.isOn = true;
        //Debug.Log("Single day chosen:" + date);
        m_dateTime_single = date;
    }

    private void MultiDateChosen(DateTime date)
    {
        //when we select a date we'll manually check the box
        m_toggle_multi.isOn = true;
        //Debug.Log("Multi To chosen:" + date);
        m_dateTime_multi.Add(date);
    }


    private void NoMatchesFound()
    {
        //for now..
        Window_GenericMessage.Inst.Show("No Results Found",
            "Unable to locate any training reports.  Please check the search criteria and try again.");
    }

    private bool VerifyDateEntry(DateTime date)
    {
        //this is our default, unused year, so if this is 1900 we know we haven't selected a date yet..
        return date.Year != 1900;
    }

    #region obsolete date verification methods
    
    private bool VerifyDateEntry(List<DateTime> dates)
    {
        if (dates.Count != 2) return false;

        //make sure they aren't somehow the same day
        var day1 = dates[0].Day;
        var month1 = dates[0].Month;
        var year1 = dates[0].Year;
        var day2 = dates[1].Day;
        var month2 = dates[1].Month;
        var year2 = dates[1].Year;
        if (day1 == day2 && month1 == month2 && year1 == year2) return false;

        return true;
    }

    private bool VerifyDateEntry(TMP_InputField[] dateArray)
    {
        var day = 0;
        var month = 0;
        var year = 0;

        //first make sure we can parse everything to an int
        try
        {
            month = int.Parse(dateArray[0].text);
            day = int.Parse(dateArray[1].text);
            year = int.Parse(dateArray[2].text);
        }
        catch (Exception)
        {
            return false;
        }

        //everything is an int, so now check the ranges
        if (month < 1 || month > 12) return false;
        if (day < 1 || day > 31) return false;

        //we can accept a year in XX or XXXX format
        //this will get us to within 2010-2050, seems reasonable..
        if (year < 100)
        {
            if (year < 10 || year > 50) return false;
        }
        else
        {
            if (year < 2010 || year > 2050) return false;
        }

        //so getting here we should have a month 1-12, day 1-31, and year 10-50..
        return true;
    }
    
    #endregion
}