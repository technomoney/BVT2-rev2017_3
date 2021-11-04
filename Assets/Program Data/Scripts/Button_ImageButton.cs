using UnityEngine;
using UnityEngine.UI;

public class Button_ImageButton : MonoBehaviour
{
    public delegate void Event_ImageButton(Button_ImageButton imageButton);

    public Event_ImageButton event_imageButtonPushed;
    public Button m_button;

    public Image m_image;
    public Image m_image_backing;


    private bool m_isDown;
    private float m_isDownCounter;

    public void Initialize(Sprite sprite)
    {
        m_image.sprite = sprite;
        m_button.onClick.AddListener(Pushed);
    }


    private void Pushed()
    {
        event_imageButtonPushed(this);
    }
}