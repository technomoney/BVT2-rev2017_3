using UnityEngine;
using UnityEngine.UI;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [RequireComponent(typeof(RectTransform))]
    public class CutoutScript : MonoBehaviour
    {
        [SerializeField]
        bool _preview;
        public bool preview
        {
            get { return _preview; }
            set { _preview = value; UpdateCutoutGraphic(); }
        }
        bool temporarilyHidden;

        public bool positionRelative = true;

        public Rect rect
        {
            get
            {
                transform.rotation = Quaternion.identity;
#if UNITY_2017_3_OR_NEWER
                ((RectTransform)transform).ForceUpdateRectTransforms();
#endif
                Rect resultRect = RectTransformToScreenSpace(((RectTransform)transform));
                Resolution resolution = ScreenExtensions.CurrentResolution();
                if (resultRect.x < 0 || resultRect.y < 0 || resultRect.x > resolution.width || resultRect.y > resolution.height)
                    Debug.LogError("Cutout Error: Position on cutout: " + gameObject.name + " is off screen for resolution: " + resolution);
                if (resultRect.x + resultRect.width > resolution.width || resultRect.y + resultRect.height > resolution.height)
                    Debug.LogWarning("Cutout Error: Cutout: " + gameObject.name + " is too large to fit on screen for resolution: " + resolution);

                return resultRect;
            }
        }

        static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Vector2 position = new Vector2(transform.position.x - transform.pivot.x * size.x,
                                           transform.position.y - transform.pivot.y * size.y);
            return new Rect(position, size);
        }

#if UNITY_EDITOR
        public bool clickToSelectPivot;
#endif

        Graphic cutoutGraphic;

        void Awake()
        {
            cutoutGraphic = GetComponent<Graphic>();
        }

        public void Show()
        {
            temporarilyHidden = false;
            UpdateCutoutGraphic();
        }

        public void Hide()
        {
            temporarilyHidden = true;
            UpdateCutoutGraphic();
        }

        void UpdateCutoutGraphic()
        {
            if (cutoutGraphic != null)
                cutoutGraphic.enabled = _preview && !temporarilyHidden;
        }

        void Update()
        {
            if (Input.GetMouseButton(0) && preview)// && clickToSelectPivot)
            {
                Resolution resolution = ScreenExtensions.CurrentResolution();
                RectTransform rectTransform = cutoutGraphic.rectTransform;
                if (positionRelative)
                {
                    float centerX = Input.mousePosition.x / resolution.width;
                    float centerY = Input.mousePosition.y / resolution.height;
                    float halfWidth = (rectTransform.anchorMax.x - rectTransform.anchorMin.x) / 2f;
                    float halfHeight = (rectTransform.anchorMax.y - rectTransform.anchorMin.y) / 2f;
                    rectTransform.anchoredPosition = new Vector2(0f, 0f);

                    if (rectTransform.anchorMin.x != rectTransform.anchorMax.x)
                    {
                        rectTransform.offsetMin = new Vector2(0f, rectTransform.offsetMin.y);
                        rectTransform.offsetMax = new Vector2(0f, rectTransform.offsetMax.y);
                    }
                    if (rectTransform.anchorMin.y != rectTransform.anchorMax.y)
                    {
                        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
                        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);
                    }

                    rectTransform.anchorMin = new Vector2(centerX - halfWidth, centerY - halfHeight);
                    rectTransform.anchorMax = new Vector2(centerX + halfWidth, centerY + halfHeight);
                }
                else
                {
                    if (rectTransform.anchorMin.x != rectTransform.anchorMax.x)
                        rectTransform.sizeDelta = new Vector2((rectTransform.anchorMax.x - rectTransform.anchorMin.x) * resolution.width, rectTransform.sizeDelta.y);
                    if (rectTransform.anchorMin.y != rectTransform.anchorMax.y)
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (rectTransform.anchorMax.y - rectTransform.anchorMin.y) * resolution.height);

                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    rectTransform.anchorMax = new Vector2(0f, 0f);
                    rectTransform.anchoredPosition = Input.mousePosition;
                }
            }
        }
    }
}