using UnityEngine;

public class Testing_ManualLines : MonoBehaviour
{
    public RectTransform line1, line2;

    public RectTransform pfb_dot;

    // Use this for initialization
    private void Start()
    {
        DrawLine();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DrawLine();
    }

    private void DrawLine()
    {
        //draw our line between the two points
        //find the center between the two points
        var center = Vector2.Lerp(line1.anchoredPosition, line2.anchoredPosition, .5f);
        Debug.Log(center);
        //now make a dot here
        var dot = Instantiate(pfb_dot, transform);
        dot.anchoredPosition = center;
        //now set the width of the dot to the distance bweteen the dots
        var width = Vector2.Distance(line1.anchoredPosition, line2.anchoredPosition);
        Debug.Log(width);
        dot.sizeDelta = new Vector2(width, dot.sizeDelta.y);
        //now we need to set the rotation of the line to match up to the two vector 2s
        //var angle = Vector2.Angle(line1.anchoredPosition, line2.anchoredPosition);
        var angle = Mathf.Atan2(line2.anchoredPosition.y - line1.anchoredPosition.y,
                        line2.anchoredPosition.x - line1.anchoredPosition.x) * Mathf.Rad2Deg;
        Debug.Log(angle);

        dot.rotation = Quaternion.Euler(0, 0, angle);
    }
}