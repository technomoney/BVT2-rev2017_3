using UnityEngine;

public class InfoPopup : MonoBehaviour
{
    public InfoButtonType m_infoType;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}