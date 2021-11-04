using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Countdown : SerializedMonoBehaviour
{
    public static Countdown Instance;

    /// <summary>
    ///     The time remaining on this countdown
    /// </summary>
    private float m_currentTime;

    private bool m_running;
    private bool m_showCountdownText;
    public TextMeshProUGUI m_text_display, m_text_backing;

    [PropertyTooltip("The offset for the backing text when we use the countdown on a solid white/black background")]
    public Vector2 m_vec2_backingPosition;

    // Use this for initialization
    private void Start()
    {
        Instance = this;

        //put it back in position..
        m_text_display.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        m_text_backing.GetComponent<RectTransform>().anchoredPosition = m_vec2_backingPosition;

        //hide the text initially
        m_text_display.enabled = false;
        m_text_backing.enabled = false;

        m_showCountdownText = true;
        m_running = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_running) return;

        m_currentTime -= Time.deltaTime;
        if (m_showCountdownText)
        {
            //casting the float to an int will truncate, then we add one to make the countdown look like:
            //3/2/1, instead of 3/2/1/0 with uneven timing display of the the 3..
            m_text_display.text = ((int) m_currentTime + 1).ToString("0");
            m_text_backing.text = ((int) m_currentTime + 1).ToString("0");
        }


        if (m_currentTime > 0) return;

        Finish();
    }

    public void StartCountdown(float time)
    {
        if (time <= 0)
        {
            DoInvisibleCountdown();
            return;
        }

        m_text_display.enabled = true;


        m_showCountdownText = true;
        m_currentTime = time;
        m_running = true;
    }

    private void DoInvisibleCountdown()
    {
        m_currentTime = ConstantDefinitions.Instance.countdown_invisibleTime;
        m_running = true;
        m_showCountdownText = false;
    }

    /// <summary>
    ///     This will hide the countdown and is identical to Finish() except it doesn't call
    ///     Manager_Test.Instance.CountdownFinished();
    ///     This should generally only be called if a test is stopped before the countdown begins
    /// </summary>
    public void HaltCountdown()
    {
        m_running = false;
        m_text_display.enabled = false;
        m_text_backing.enabled = false;
    }

    public void Finish()
    {
        m_running = false;
        m_text_display.enabled = false;
        m_text_backing.enabled = false;
        //now actually start the test
        Manager_Test.Instance.CountdownFinished();
    }
}