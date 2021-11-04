using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    public InfoButtonType infoType;

    // Use this for initialization
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonPushed);
    }


    private void ButtonPushed()
    {
        Manager_InfoButtons.Instance.ShowInfo(infoType);
    }
}