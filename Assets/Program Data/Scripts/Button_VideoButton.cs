using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Button_VideoButton : MonoBehaviour
{
    public delegate void Event_VideoButton(Button_VideoButton videoButton, bool isTestSelection = false);

    public Event_VideoButton event_videoButtonPushed;
    public Button m_button;
    public VideoClip m_clip;
    public Image m_image_backing;

    public Image m_image_bg;
    public RectTransform m_rectTransform;
    public Vector3 m_vec3_videoScale, m_vec3_videoRotation;

    public void Initialize(Sprite sprite, VideoClip clip, Vector3 rotation, Vector3 scale)
    {
        m_image_bg.sprite = sprite;
        m_clip = clip;
        m_vec3_videoRotation = rotation;
        m_vec3_videoScale = scale;
        m_button.onClick.AddListener(Pushed);
    }

    private void Pushed()
    {
        event_videoButtonPushed(this);
    }
}