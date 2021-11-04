using System;
using System.Collections;
using System.IO;
using sharpPDF;
using sharpPDF.Enumerators;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Entry_PatientTestSummary : SerializedMonoBehaviour
{
    public int image_yPos, m_reportSpacing;
    public Button m_button_main, m_button_export, m_button_trash;
    private int m_currentTextPos;
    public Image m_image_textIcon;
    private Patient m_patient;
    private string m_pdf_fileName;
    public TextMeshProUGUI m_text_name, m_text_date, m_text_score;
    public PatientTrainingSummary m_trainingSummary;
    private string path;

    private void Awake()
    {
        m_button_main.onClick.AddListener(Clicked_Main);
        m_button_trash.onClick.AddListener(Clicked_Trash);
        m_button_export.onClick.AddListener(Clicked_Export);
    }

    public void Initialize(PatientTrainingSummary summary, Patient p)
    {
        m_trainingSummary = summary;
        m_patient = p;
        m_image_textIcon.sprite = Menu_Reports.Instance.GetTestIcon(summary.m_testType);
        m_image_textIcon.preserveAspect = true;
        m_text_name.text = summary.m_testName;
        m_text_date.text = summary.m_dateTime.Month + "/" + summary.m_dateTime.Day + "/" + summary.m_dateTime.Year;
        m_text_score.text = summary.m_shortScore;
    }

    public Patient GetPatient()
    {
        return m_patient;
    }

    private void Clicked_Main()
    {
        //when we click an entry, we will just recreate the test summary as if it had been clicked from the scoreboard
        Manager_SummaryGraphs.Instance.ShowSummary(m_trainingSummary);
    }

    private void Clicked_Trash()
    {
        Window_PatientDetail.Instance.ShowDeleteConfirmation(this);
    }

    private void Clicked_Export()
    {
        StartCoroutine(MakePdf());
    }

    private IEnumerator MakePdf()
    {
        m_pdf_fileName = m_patient.m_name_last + "_" +
                         m_trainingSummary.m_dateTime.Month + "_" +
                         m_trainingSummary.m_dateTime.Day + "_" +
                         m_trainingSummary.m_dateTime.Year + "_" +
                         m_trainingSummary.m_dateTime.Hour + "_" +
                         m_trainingSummary.m_dateTime.Minute + "_" +
                         m_trainingSummary.m_testName +
                         "_Report.pdf";

        var pdf = new pdfDocument("Test Report", "BVT", false);
        var page1 = pdf.addPage();


        //name		
        page1.addText("Name: " + m_patient.m_name_first + " " + m_patient.m_name_last,
            10, 660, predefinedFont.csHelveticaOblique, 30, new pdfColor(predefinedColor.csBlack));

        page1.addText("Operator: " + m_trainingSummary.m_operatorAccountName, 430, 775,
            predefinedFont.csHelveticaOblique, 12, new pdfColor(predefinedColor.csBlack));

        //start of normal text positions..
        m_currentTextPos = 630;

        //date
        AddText("Date: " + m_trainingSummary.m_dateTime.Month + "/" + m_trainingSummary.m_dateTime.Day + "/" +
                m_trainingSummary.m_dateTime.Year, page1);

        AddText("Time: " +
                (m_trainingSummary.m_dateTime.Hour < 10
                    ? "0"
                    : "")
                + m_trainingSummary.m_dateTime.Hour +
                ":" +
                (m_trainingSummary.m_dateTime.Minute < 10
                    ? "0"
                    : "")
                + m_trainingSummary.m_dateTime.Minute
                + " ", page1);

        //test
        AddText("Training: " + m_trainingSummary.m_testName, page1);
        //score
        AddText("Score: " + m_trainingSummary.m_shortScore, page1);
        EmptyLine();

        //test settings
        AddText("-Test Settings-", page1);
        WriteTestSettings(page1);
        EmptyLine();

        //Global settings
        AddText("-Global Settings-", page1);
        AddText("Background: " + m_trainingSummary.m_background, page1);
        AddText("Target: " + m_trainingSummary.m_target, page1);
        AddText("Touch Sensitivity: " + m_trainingSummary.m_sensitivity, page1);
        AddText("Grid Size: " + m_trainingSummary.m_gridSize, page1);
        AddText("Target Size: " + m_trainingSummary.m_targetSize, page1);
        AddText("Audio: " + m_trainingSummary.m_audio, page1);
        AddText("Countdown: " + m_trainingSummary.m_countdown, page1);
        AddText("Input Mode: " + m_trainingSummary.m_inputMode, page1);
        if (!m_trainingSummary.m_inputMode.Equals("Touch"))
        {
            AddText("Hover Time: " + m_trainingSummary.m_balance_hoverTime, page1);
            AddText("Decay Speed: " + m_trainingSummary.m_decayTime, page1);
        }

        path = Application.persistentDataPath + "/PDF";

        //make sure the pdf folder exists..
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //this works fine in the editor but won't work in a build since the paths don't stay this way..
        //yield return StartCoroutine(page1.newAddImage("FILE://" + Application.dataPath + "/Resources/PdfLogo128.jpg", 0,
        //	image_yPos));
        //loading the image must be done like this, must be a jpg, must be a perfect power of 2, must not be compressed...
        //the pdf.newAddImage just takes a byte array of a WWW request
        Texture2D tex;
        yield return tex = Resources.Load("logo_128") as Texture2D;
        var array = tex.EncodeToJPG();
        File.WriteAllBytes(Application.persistentDataPath + "/PDF/logo_128.jpg", array);

        //yield return tex = Resources.Load("PdfLogoBig") as Texture2D;
        //var newTex = Instantiate (tex);
        //TextureScale.Point(newTex, 64, 64);
        yield return new WaitForSeconds(2f);
        //page1.newAddImage(tex.EncodeToJPG(), 0, image_yPos, 64, 64);
        //page1.newAddImage(newTex.GetRawTextureData(), 0, image_yPos, 64, 64);
        //page1.newAddImage(File.ReadAllBytes(Application.dataPath+"/Resources/logo_128.jpg"), 0,
        //	image_yPos, 128, 128);

        page1.newAddImage(File.ReadAllBytes(Application.persistentDataPath + "/PDF/logo_128.jpg"), 0,
            image_yPos, 128, 128);


        pdf.createPDF(path + "/" + m_pdf_fileName);

        Debug.Log("Finished Making PDF");

        Application.OpenURL("file:///" + path + "/" + m_pdf_fileName);
    }

    private void AddText(string text, pdfPage page, int xPos = 10)
    {
        page.addText(text, xPos, m_currentTextPos, predefinedFont.csHelveticaOblique, 18,
            new pdfColor(predefinedColor.csBlack));

        m_currentTextPos -= m_reportSpacing;
    }

    private void EmptyLine()
    {
        m_currentTextPos -= m_reportSpacing;
    }

    private void WriteTestSettings(pdfPage page)
    {
        switch (m_trainingSummary.m_testType)
        {
            case TestType.Speed:
                AddText("Duration: " + m_trainingSummary.m_testDuration, page);
                AddText("Pacing: " + m_trainingSummary.m_paceMode, page);
                AddText("Auto Interval: " + m_trainingSummary.m_autopaceInterval.ToString("0.000"), page);
                break;
            case TestType.Peripheral:
                AddText("Duration: " + m_trainingSummary.m_testDuration, page);
                AddText("Pacing: " + m_trainingSummary.m_paceMode, page);
                AddText("Auto Interval: " + m_trainingSummary.m_autopaceInterval.ToString("0.000"), page);
                AddText("Target Area: " + m_trainingSummary.m_targetArea, page);
                AddText("Zone Size: " + m_trainingSummary.m_targetSize, page);
                AddText("Zone Distribution: " + m_trainingSummary.m_zoneDist, page);
                break;
            case TestType.Sequence:
                AddText("Display Time: " + m_trainingSummary.m_sequence_displayTime, page);
                AddText("Difficulty: " + m_trainingSummary.m_sequence_difficulty, page);
                break;
            case TestType.Reaction:
                //todo, the pacing here is probably a bit weird..
                AddText("Pacing: " + m_trainingSummary.m_reaction_autoPace, page);
                AddText("Auto Interval: " + m_trainingSummary.m_autopaceInterval.ToString("0.000"), page);
                AddText("Target Spacing: " + m_trainingSummary.m_reaciton_targetSpacing, page);
                AddText("Orientation: " + m_trainingSummary.m_reaction_orientation, page);
                break;
            case TestType.Balance:
                AddText("Duration: " + m_trainingSummary.m_testDuration, page);
                AddText("Tracking Mode: " + m_trainingSummary.m_balance_trackingMode, page);
                AddText("Tracking Direction: " + m_trainingSummary.m_balance_trackingDirection, page);
                AddText("Target Size: " + m_trainingSummary.m_balance_targetSize, page);
                if (m_trainingSummary.m_balance_trackingMode == TargetTrackingMode.Static)
                {
                    AddText("Hover Time: " + m_trainingSummary.m_balance_hoverTime, page);
                    AddText("Decay Speed: " + m_trainingSummary.m_balance_progressDecay, page);
                }

                break;
            case TestType.Flash:
                AddText("Starting Level: " + m_trainingSummary.m_flash_startingLevel, page);
                AddText("Display Time: " + m_trainingSummary.m_flash_displayTime, page);
                break;
            case TestType.GoNoGo:
                AddText("Duration: " + m_trainingSummary.m_testDuration, page);
                AddText("No-Go Frequency: " + m_trainingSummary.m_goNogo_frequency, page);
                AddText("Display Time: " + m_trainingSummary.m_goNogo_displayTime, page);
                break;
            case TestType.Contrast:
                AddText("Blend Rate: " + m_trainingSummary.m_contrast_blendSpeed, page);
                AddText("Pace Time: " + m_trainingSummary.m_contrast_paceTime, page);
                AddText("Oversize Targets: " + m_trainingSummary.m_contrast_oversizeTargets, page);
                AddText(
                    "Lowest Contrast % Achieved: " +
                    (m_trainingSummary.m_contrast_lowestAchieved * 100).ToString("0.000") +
                    "%", page); //the *100 here makes this look just like short score
                break;
            case TestType.Rhythm:
                AddText("Duration: " + m_trainingSummary.m_testDuration, page);
                AddText("Pacing: " + m_trainingSummary.m_paceMode, page);
                AddText("Auto Interval: " + m_trainingSummary.m_autopaceInterval.ToString("0.000"), page);
                break;
            case TestType.Multi:
                AddText("Target Sets: " + m_trainingSummary.m_multi_targetSets, page);
                AddText("Rotation Speed: " + m_trainingSummary.m_multi_rotationSpeed, page);
                AddText("Rotation Time: " + m_trainingSummary.m_multi_rotationTime, page);
                AddText("Trials: " + m_trainingSummary.m_multi_trials, page);
                break;
            case TestType.All:
            case TestType.None:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}