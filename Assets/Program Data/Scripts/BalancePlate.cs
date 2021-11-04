using System.Collections.Generic;
using System.IO;
using BertecDevice;
using Sirenix.OdinInspector;
using UnityEngine;

public enum DeviceType
{
    UsbForcePlate = 0,
    CdpSystemBase = 1,
    WebService = 2
}

public class BalancePlate : SerializedMonoBehaviour
{
    public static BalancePlate Instance;
    private static BertecUSBDevice m_forcePlate;

    private static bool firstOne = true;
    public bool m_autoReConnect;
    public float m_bpSpan_x, m_bpSpan_y;
    public bool m_connectOnStart;
    [HideInEditorMode] public double m_cop_x, m_cop_y, m_fz, m_fzL, m_fzR, m_mxR, m_mxL, m_myR, m_myL, m_mx, m_my;
    private double m_counter;

    private float m_currentTime;
    private string m_dataLocation;
    public bool m_debugCop;
    public DeviceType m_deviceToUse;
    private string m_devName;

    private bool m_isRecordingData;

    [InfoBox("If the device is null, how long should be wait to try and reconnect")]
    public float m_reconnectTime;

    [InfoBox("Should be try to reconnect while a test is running?  Should probably be turned off..")]
    public bool m_reconnectWhileRunning;

    private List<double>[] m_recordedData;

    [HideInEditorMode] public float m_screen_width, m_screen_height, m_usableScreen_x, m_usableScreen_y;

    private StreamWriter writer;


    // Use this for initialization
    private void Start()
    {
        Instance = this;
        m_currentTime = 0;
        if (m_connectOnStart) ConnectDevice();

        m_cop_x = 0;
        m_cop_y = 0;
        m_fz = 0;

        m_screen_width = GetComponent<RectTransform>().rect.width;
        m_screen_height = GetComponent<RectTransform>().rect.height;

        //Debug.Log("Screen: " + m_screen_width + ", " + m_screen_height);

        m_usableScreen_x = m_screen_width * (m_bpSpan_x / 100);
        m_usableScreen_y = m_screen_height * (m_bpSpan_y / 100);

        Debug.Log("Balance Plate usable screen: " + m_usableScreen_x + ", " + m_usableScreen_y);

        InvokeRepeating("GetFpInput", 2, .001f);
    }

    public void ConnectDevice()
    {
        m_devName = ParseCommandLine.Instance.queryString("device", "default").ToLower();
        Debug.Log("device key is: " + m_devName);

        if (m_devName == "usb" || m_devName == "forceplate")
            m_deviceToUse = DeviceType.UsbForcePlate;

        if (m_deviceToUse == DeviceType.UsbForcePlate)
        {
            Debug.Log("Instantiating BertecDevice.ForcePlate");
            m_forcePlate = new BertecUSBDevice();
        }

        Window_BalancePlate.Instance.SetDeviceName(m_forcePlate.State.ToString());
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_connectOnStart || m_forcePlate == null) return;

        if (m_forcePlate.State == PlateState.INITIAL_SETUP || m_forcePlate.State == PlateState.PLATE_NOT_SETUP)
        {
            if (!m_autoReConnect) return;

            m_currentTime += Time.deltaTime;
            if (m_currentTime < m_reconnectTime) return;

            m_currentTime = 0;
            if (!Manager_Test.Instance.IsTestRunning() ||
                Manager_Test.Instance.IsTestRunning() && m_reconnectWhileRunning)
                ConnectDevice();
        }


        //testing only
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            m_isRecordingData = !m_isRecordingData;
            Debug.Log("RecordingData: " + m_isRecordingData);
            if (m_isRecordingData)
            {
                m_counter = 0;
                //get things ready to record
                m_recordedData = new List<double>[12];
                for (int x = 0; x < m_recordedData.Length; x++)
                    m_recordedData[x] = new List<double>();

                m_dataLocation = ConstantDefinitions.Instance.m_dataPath + "/Temp_BPData/TestData.txt";
            }
            else
            {
                int size = 0;
                m_recordedData.ForEach(l => size += l.Count);
                Debug.Log("Recorded Data Size: " + size);
            }
        }*/


        //testing only
        //if (Input.GetKeyDown(KeyCode.W))
        //WriteRecordedData();

        //Debug.Log("GetFp: " + m_getFpCount + "  |  With Data: " + m_getfpWithDataCount);
    }

    public void ZeroPlate()
    {
        m_forcePlate.ZeroPlate();
    }

    /// <summary>
    ///     Get the position in canvas space of the bp cop based on the center of pressure, the screen size,
    ///     and the usable screen settings
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 GetAdjustedBpPosition()
    {
        //todo get device info from bp to figure out model

        //for now , just assume a 50x46 lol
        var cop_x_percentage = (float) m_cop_x / .250f;
        var cop_y_percentage = (float) m_cop_y / .230f;

        //now place the image that this location, as a percentage of the usable screen
        return new Vector2(m_usableScreen_x * cop_x_percentage,
            m_usableScreen_y * cop_y_percentage);
    }

    private void GetFpInput()
    {
        if (m_forcePlate == null || !m_forcePlate.HasGoodLoad) return;

        if (firstOne)
        {
            firstOne = false;
            var n = m_forcePlate.ChannelNames();
            Debug.Log("Channel Names: " + string.Join(" , ", n));
        }

        var buffer = new double[64];
        var timestamp = 0;

        var cop = m_forcePlate.LastCOPvalues();

        var x = cop.x;
        var y = cop.y;

        m_cop_x = x;
        m_cop_y = y;
        if (m_isRecordingData)
            while (m_forcePlate.GetForceData(64, ref buffer, ref timestamp) >= 0)
            {
                m_recordedData[0].Add(m_counter);
                m_recordedData[1].Add(buffer[0]);
                m_recordedData[2].Add(buffer[1]);
                m_recordedData[3].Add(buffer[2]);
                m_recordedData[4].Add(buffer[3]);
                m_recordedData[5].Add(buffer[4]);
                m_recordedData[6].Add(buffer[5]);
                m_recordedData[7].Add(buffer[6]);
                m_recordedData[8].Add(buffer[7]);
                m_recordedData[9].Add(buffer[8]);
                m_recordedData[10].Add(m_cop_x);
                m_recordedData[11].Add(m_cop_y);
                m_counter++;
            }
    }

    public void StartRecordingData(string dataLocation)
    {
        if (IsNull())
        {
            Debug.Log("Can't start writing data with null FP object..");
            return;
        }

        //get things ready to record
        m_isRecordingData = true;
        m_counter = 0;
        m_recordedData = new List<double>[12];
        for (var x = 0; x < m_recordedData.Length; x++)
            m_recordedData[x] = new List<double>();
        m_dataLocation = dataLocation;

        Debug.Log("Now recording BP data");
    }

    private void WriteRecordedData()
    {
        Debug.Log("Writing Recorded Data..");

        //make sure each list is of the same length
        var lengths = "Data List Lengths: ";
        var listLengths = new int[m_recordedData.Length];

        for (var x = 0; x < m_recordedData.Length; x++)
        {
            lengths += m_recordedData[x].Count + " | ";
            listLengths[x] = m_recordedData[x].Count;
        }

        //Debug.Log(lengths);

        var allEqual = true;
        var length = m_recordedData[0].Count;
        for (var x = 1; x < m_recordedData.Length; x++)
            if (m_recordedData[x].Count != length)
            {
                allEqual = false;
                break;
            }

        if (!allEqual)
        {
            Debug.Log("Length Mismatch");
            return;
        }

        var sw = File.CreateText(m_dataLocation);

        //write our header
        const string header = "//Recorded Balance Plate Data\n" +
                              "//Fz_Right  |  Mx_Right  |  My_Right  |  Fz_Left  |  Mx_Left  |  My_Left  |  " +
                              "Fz_Combined  |  Mx_Combined  |  My_Combined  |  CoP_X  |  CoP_Y";

        sw.WriteLine(header);
        sw.WriteLine("//");

        for (var x = 0; x < length; x++)
            sw.WriteLine(m_recordedData[0][x] + "  " +
                         m_recordedData[1][x] + "  " +
                         m_recordedData[2][x] + "  " +
                         m_recordedData[3][x] + "  " +
                         m_recordedData[4][x] + "  " +
                         m_recordedData[5][x] + "  " +
                         m_recordedData[6][x] + "  " +
                         m_recordedData[7][x] + "  " +
                         m_recordedData[8][x] + "  " +
                         m_recordedData[9][x] + "  " +
                         m_recordedData[10][x] + "  " +
                         m_recordedData[11][x] + "  ");

        sw.Close();

        Debug.Log("Finished Writing Recorded Data");
    }

    public void StopRecordingData()
    {
        //if we aren't recording, just return, no harm in making sure we're not recording when stopping a test..
        if (!m_isRecordingData) return;


        m_isRecordingData = false;
        WriteRecordedData();
    }

    public string GetFpStateName()
    {
        return m_forcePlate == null ? "No Device" : m_forcePlate.State.ToString();
    }

    public PlateState GetFpState()
    {
        return m_forcePlate.State;
    }

    /// <summary>
    ///     Returns true only if the state is have good load..
    /// </summary>
    public bool IsNull()
    {
        if (m_forcePlate == null) return true;
        if (m_forcePlate.State == PlateState.PLATE_NOT_SETUP) return true;

        return false;
    }

    public double[] GetSnapshot()
    {
        if (!m_forcePlate.HasGoodLoad) return new double[] {0, 0, 0, 0, 0, 0, 0, 0, 0};

        var buffer = new double[64];
        var timestamp = 0;
        m_forcePlate.GetForceData(64, ref buffer, ref timestamp);
        return buffer;
    }
}