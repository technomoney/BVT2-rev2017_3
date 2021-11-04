using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class WMG_TMP_Prefab_Gen : MonoBehaviour {

	static GameObject theCanvas;

	static bool setup() {
		theCanvas = GameObject.Find("Canvas");
		if (theCanvas == null) return false;
		return true;
	}

	[MenuItem ("Assets/Graph Maker/UGUI -> TMP Prefabs")]
	static void uGUItoTMPPrefabs () {
		if (!setup()) return;
		UGUItoTMPPrefabs();
	}

	[MenuItem ("Assets/Graph Maker/TMP -> UGUI Prefabs")]
	static void tMPtoUGUIPrefabs () {
		if (!setup()) return;
		TMPtoUGUIPrefabs();
	}

	static void UGUItoTMPPrefabs() {
		// Values to copy over to TMP
		string defaultText = "";
		int fontSize = 0;
		Color fontColor = Color.white;
		TextAlignmentOptions textAlignment;

		string[] allPrefabPaths = Directory.GetFiles(Application.dataPath + "/Graph_Maker/Prefabs", "*.prefab", SearchOption.AllDirectories);
		for (int i = 0; i < allPrefabPaths.Length; i++) {
			string prefabPath =  allPrefabPaths[i].Remove(0, allPrefabPaths[i].IndexOf("Assets/"));
			prefabPath = prefabPath.Replace('\\', '/');
			GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			go.transform.SetParent(theCanvas.transform, false);
			go.name = prefab.name;
			Text[] uguiTexts = go.GetComponentsInChildren<Text>(true);
			if (uguiTexts.Length > 0) {
				foreach (Text uText in uguiTexts) {
					defaultText = uText.text;
					fontSize = uText.fontSize;
					fontColor = uText.color;
					textAlignment = getTMPalignment(uText.alignment);
					GameObject textGO = uText.gameObject;
					DestroyImmediate(uText);
					TextMeshProUGUI tmpText = textGO.AddComponent<TextMeshProUGUI>();
					tmpText.text = defaultText;
					tmpText.fontSize = fontSize;
					tmpText.color = fontColor;
					tmpText.alignment = textAlignment;
				}
				createPrefab(go, prefabPath);
				DestroyImmediate(go);
			}
			else {
				DestroyImmediate(go);
			}
		}
	}

	static void TMPtoUGUIPrefabs() {
		// Values to copy over to TMP
		string defaultText = "";
		int fontSize = 0;
		Color fontColor = Color.white;
		TextAnchor textAlignment;
		
		string[] allPrefabPaths = Directory.GetFiles(Application.dataPath + "/Graph_Maker/Prefabs", "*.prefab", SearchOption.AllDirectories);
		for (int i = 0; i < allPrefabPaths.Length; i++) {
			string prefabPath =  allPrefabPaths[i].Remove(0, allPrefabPaths[i].IndexOf("Assets/"));
			prefabPath = prefabPath.Replace('\\', '/');
			GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			go.transform.SetParent(theCanvas.transform, false);
			go.name = prefab.name;
			TextMeshProUGUI[] tmpTexts = go.GetComponentsInChildren<TextMeshProUGUI>(true);
			if (tmpTexts.Length > 0) {
				foreach (TextMeshProUGUI tmpText in tmpTexts) {
					defaultText = tmpText.text;
					fontSize = Mathf.RoundToInt(tmpText.fontSize);
					fontColor = tmpText.color;
					textAlignment = getUGUIalignment(tmpText.alignment);
					GameObject textGO = tmpText.gameObject;
					DestroyImmediate(tmpText);
					Text uText = textGO.AddComponent<Text>();
					uText.text = defaultText;
					uText.fontSize = fontSize;
					uText.color = fontColor;
					uText.alignment = textAlignment;
					uText.horizontalOverflow = HorizontalWrapMode.Overflow;
					uText.verticalOverflow = VerticalWrapMode.Overflow;
				}
				createPrefab(go, prefabPath);
				DestroyImmediate(go);
			}
			else {
				DestroyImmediate(go);
			}
		}
	}

	static TextAlignmentOptions getTMPalignment(TextAnchor uguiAnchor) {
		if (uguiAnchor == TextAnchor.LowerCenter) {
			return TextAlignmentOptions.Bottom;
		}
		else if (uguiAnchor == TextAnchor.LowerLeft) {
			return TextAlignmentOptions.BottomLeft;
		}
		else if (uguiAnchor == TextAnchor.LowerRight) {
			return TextAlignmentOptions.BottomRight;
		}
		else if (uguiAnchor == TextAnchor.MiddleCenter) {
			return TextAlignmentOptions.Center;
		}
		else if (uguiAnchor == TextAnchor.MiddleLeft) {
			return TextAlignmentOptions.Left;
		}
		else if (uguiAnchor == TextAnchor.MiddleRight) {
			return TextAlignmentOptions.Right;
		}
		else if (uguiAnchor == TextAnchor.UpperCenter) {
			return TextAlignmentOptions.Top;
		}
		else if (uguiAnchor == TextAnchor.UpperLeft) {
			return TextAlignmentOptions.TopLeft;
		}
		else if (uguiAnchor == TextAnchor.UpperRight) {
			return TextAlignmentOptions.TopRight;
		}
		return TextAlignmentOptions.Baseline;
	}

	static TextAnchor getUGUIalignment(TextAlignmentOptions tmpAnchor) {
		if (tmpAnchor == TextAlignmentOptions.Bottom) {
			return TextAnchor.LowerCenter;
		}
		else if (tmpAnchor == TextAlignmentOptions.BottomLeft) {
			return TextAnchor.LowerLeft;
		}
		else if (tmpAnchor == TextAlignmentOptions.BottomRight) {
			return TextAnchor.LowerRight;
		}
		else if (tmpAnchor == TextAlignmentOptions.Center) {
			return TextAnchor.MiddleCenter;
		}
		else if (tmpAnchor == TextAlignmentOptions.Left) {
			return TextAnchor.MiddleLeft;
		}
		else if (tmpAnchor == TextAlignmentOptions.Right) {
			return TextAnchor.MiddleRight;
		}
		else if (tmpAnchor == TextAlignmentOptions.Top) {
			return TextAnchor.UpperCenter;
		}
		else if (tmpAnchor == TextAlignmentOptions.TopLeft) {
			return TextAnchor.UpperLeft;
		}
		else if (tmpAnchor == TextAlignmentOptions.TopRight) {
			return TextAnchor.UpperRight;
		}
		return TextAnchor.MiddleCenter;
	}

	static void createPrefab(GameObject obj, string prefabPath) {
		// Create / overwrite prefab
		Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
		
		if (prefab != null) {
			PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ReplaceNameBased);
		}
		else {
			prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
			PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ReplaceNameBased);
		}
	}
}
