using UnityEngine;
using TMPro;

public class WMG_TMP_Text_Functions : MonoBehaviour {

	public enum WMGpivotTypes {Bottom, BottomLeft, BottomRight, Center, Left, Right, Top, TopLeft, TopRight};
	
	public void changeLabelText(GameObject obj, string aText) {
		TextMeshProUGUI theLabel = obj.GetComponent<TextMeshProUGUI>();
		theLabel.text = aText;
	}
	
	public void changeLabelFontSize(GameObject obj, int newFontSize) {
		TextMeshProUGUI theLabel = obj.GetComponent<TextMeshProUGUI>();
		theLabel.fontSize = newFontSize;
	}
	
	public Vector2 getTextSize (GameObject obj) {
		TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI> ();
		text.ForceMeshUpdate();
		return new Vector2 (text.preferredWidth, text.preferredHeight);
	}
	
	public void changeSpritePivot(GameObject obj, WMGpivotTypes theType) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		TextMeshProUGUI theText = obj.GetComponent<TextMeshProUGUI>();
		if (theSprite == null) return;
		if (theType == WMGpivotTypes.Bottom) {
			theSprite.pivot = new Vector2(0.5f, 0f);
			if (theText != null) theText.alignment = TextAlignmentOptions.Bottom;
		}
		else if (theType == WMGpivotTypes.BottomLeft) {
			theSprite.pivot = new Vector2(0f, 0f);
			if (theText != null) theText.alignment = TextAlignmentOptions.BottomLeft;
		}
		else if (theType == WMGpivotTypes.BottomRight) {
			theSprite.pivot = new Vector2(1f, 0f);
			if (theText != null) theText.alignment = TextAlignmentOptions.BottomRight;
		}
		else if (theType == WMGpivotTypes.Center) {
			theSprite.pivot = new Vector2(0.5f, 0.5f);
			if (theText != null) theText.alignment = TextAlignmentOptions.Center;
		}
		else if (theType == WMGpivotTypes.Left) {
			theSprite.pivot = new Vector2(0f, 0.5f);
			if (theText != null) theText.alignment = TextAlignmentOptions.Left;
		}
		else if (theType == WMGpivotTypes.Right) {
			theSprite.pivot = new Vector2(1f, 0.5f);
			if (theText != null) theText.alignment = TextAlignmentOptions.Right;
		}
		else if (theType == WMGpivotTypes.Top) {
			theSprite.pivot = new Vector2(0.5f, 1f);
			if (theText != null) theText.alignment = TextAlignmentOptions.Top;
		}
		else if (theType == WMGpivotTypes.TopLeft) {
			theSprite.pivot = new Vector2(0f, 1f);
			if (theText != null) theText.alignment = TextAlignmentOptions.TopLeft;
		}
		else if (theType == WMGpivotTypes.TopRight) {
			theSprite.pivot = new Vector2(1f, 1f);
			if (theText != null) theText.alignment = TextAlignmentOptions.TopRight;
		}
	}

	public void changeLabelColor(GameObject obj, Color newColor) {
		TextMeshProUGUI theLabel = obj.GetComponent<TextMeshProUGUI>();
		theLabel.color = newColor;
	}
	
	public void changeLabelFontStyle(GameObject obj, FontStyle newFontStyle) {
		TextMeshProUGUI theLabel = obj.GetComponent<TextMeshProUGUI>();
		if (newFontStyle == FontStyle.Bold) {
			theLabel.fontStyle = TMPro.FontStyles.Bold;
		}
		else if (newFontStyle == FontStyle.Italic) {
			theLabel.fontStyle = TMPro.FontStyles.Italic;
		}
		else {
			theLabel.fontStyle = TMPro.FontStyles.Normal;
		}
	}
	
	public void changeLabelFont(GameObject obj, Font newFont) {
		// Implementation up to user. 
		// 1. Change all instances of "Font" to "TMP_FontAsset". Places include: WMG_Axis_Graph, WMG_Axis_Graph_E, WMG_Series, and WMG_Series_E, and also argument to this function
		// 2. Uncomment the below lines, that's it.
		//TextMeshProUGUI theLabel = obj.GetComponent<TextMeshProUGUI>();
		//theLabel.font = newFont;
	}

}
