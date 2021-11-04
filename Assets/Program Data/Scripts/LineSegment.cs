using UnityEngine;
using UnityEngine.UI;

public class LineSegment : MonoBehaviour
{
	/// <summary>
	///     This will properly position and rotate the line segment between the two given transforms
	/// </summary>
	public void Set(RectTransform r1, RectTransform r2, Color color)
    {
        var image = GetComponentInChildren<Image>();
        var rect = GetComponent<RectTransform>();

        //find the center between the two points
        var center = Vector2.Lerp(r1.anchoredPosition, r2.anchoredPosition, .5f);

        rect.anchoredPosition = center;
        //now set the width of the dot to the distance between the dots
        var width = Vector2.Distance(r1.anchoredPosition, r2.anchoredPosition);

        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        //now we need to set the rotation of the line to match up to the two vector 2s
        var anchoredPosition = r2.anchoredPosition;
        var position = r1.anchoredPosition;
        var angle = Mathf.Atan2(anchoredPosition.y - position.y,
                        anchoredPosition.x - position.x) * Mathf.Rad2Deg;

        rect.rotation = Quaternion.Euler(0, 0, angle);

        image.color = color;
    }
}