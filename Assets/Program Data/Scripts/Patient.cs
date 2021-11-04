using System;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Patient
{
    public string m_name_first { get; private set; }
    public string m_name_last { get; private set; }
    public string m_gender { get; private set; }
    public string m_dob_month { get; private set; }
    public string m_dob_day { get; private set; }
    public string m_dob_year { get; private set; }
    public string m_height_feet { get; private set; }
    public string m_height_inches { get; private set; }
    public string m_weight { get; private set; }
    public string m_notes { get; private set; }
    public string m_id { get; private set; }

    public void SetName(string first, string last)
    {
        m_name_first = first;
        m_name_last = last;
    }

    public void SetGender(string gender)
    {
        m_gender = gender;
    }

    public void SetDoB(string month, string day, string year)
    {
        m_dob_month = month;
        m_dob_day = day;
        m_dob_year = year;
    }

    public void SetHeight(string feet, string inches)
    {
        m_height_feet = feet;
        m_height_inches = inches;
    }

    public void SetWeight(string weight)
    {
        m_weight = weight;
    }

    public void SetNotes(string notes)
    {
        m_notes = notes;
    }

    public void SetId(string id)
    {
        m_id = id;
    }

    public void AddTrainingSummary(PatientTrainingSummary newSummary)
    {
        var dateTime = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour +
                       "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;

        //since we're adding a new summary, we'll need to make a training summary file in our directory
        //if we have a data path, we want to rename it to match this file..
        if (newSummary.m_dataLocation != null && newSummary.m_dataLocation.Length > 5)
        {
            //rename the file
            File.Move(newSummary.m_dataLocation, newSummary.m_dataLocation + dateTime);
            //then store the new location
            newSummary.m_dataLocation = newSummary.m_dataLocation + dateTime;
        }

        var path = Manager_Patient.Instance.GetSelectedPatientDirectory() + "/" + newSummary.m_testType + "_" +
                   dateTime;
        newSummary.m_fileName = newSummary.m_testType + "_" + dateTime;

        //serialize the summary and add it to a file in our directory
        var writer = new XmlSerializer(typeof(PatientTrainingSummary));
        var file = File.Create(path);
        writer.Serialize(file, newSummary);
        file.Close();
    }
}