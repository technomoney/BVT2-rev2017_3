using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Window_BalancePlate : BVT_Window
{
    public static Window_BalancePlate Instance;
    public Button m_button_back, m_button_zeroPlate;
    public string m_deviceName;
    public TextMeshProUGUI m_text_deviceName, m_text_cop, m_text_combined, m_text_left, m_text_right;

    // Use this for initialization
    public override void Start()
    {
        Instance = this;
        base.Start();
        SetDeviceName("No Device");
        m_button_back.onClick.AddListener(Hide);
        m_button_zeroPlate.onClick.AddListener(ZeroPlate);
    }

    // Update is called once per frame
    private void Update()
    {
        if (BalancePlate.Instance.IsNull()) return;
        if (!m_isShowing) return;

        var bpBuffer = BalancePlate.Instance.GetSnapshot();
        //just in case something messes up getting the data..
        if (bpBuffer == null || bpBuffer.Length < 9) return;
        m_text_cop.text =
            "CoP\n   X: " + BalancePlate.Instance.m_cop_x.ToString("0.000") +
            "\n   Y: " + BalancePlate.Instance.m_cop_y.ToString("0.000");

        m_text_left.text =
            "Left\n  Fz: " + bpBuffer[3].ToString("0.0") +
            "\n  Mx: " + bpBuffer[4].ToString("0.0") +
            "\n  My: " + bpBuffer[5].ToString("0.0");

        m_text_combined.text =
            "Combined\n  Fz: " + bpBuffer[6].ToString("0.0") +
            "\n  Mx: " + bpBuffer[7].ToString("0.0") +
            "\n  My: " + bpBuffer[8].ToString("0.0");

        m_text_right.text =
            "Right\n  Fz: " + bpBuffer[0].ToString("0.0") +
            "\n  Mx: " + bpBuffer[1].ToString("0.0") +
            "\n  My: " + bpBuffer[2].ToString("0.0");

        m_text_deviceName.text = "State: " + BalancePlate.Instance.GetFpStateName();
    }

    public void SetDeviceName(string name)
    {
        m_deviceName = name;
        m_text_deviceName.text = "State: " + m_deviceName;
    }

    private void ZeroPlate()
    {
        BalancePlate.Instance.ZeroPlate();

        Debug.Log("BP zeroed");
    }
}