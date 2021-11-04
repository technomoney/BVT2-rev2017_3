using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    public class FramedScreenshotScript : MonoBehaviour
    {
        RawImage screenshotImage;

        // Scale of the frame relative to the screenshot.
        // Frame height is ~1.3 times the size of the screenshot for example.
        float frameScaleX;
        float frameScaleY;

        bool setup;
        bool textureIsCopy;

        void Start()
        {
            Setup();
        }

        void Setup()
        {
            if (setup)
                return;

            setup = true;
            screenshotImage = GetComponentInChildren<RawImage>();

            RectTransform screenshotRectTransform = (RectTransform)screenshotImage.transform;

            frameScaleX = 1f / (1f - (screenshotRectTransform.anchorMin.x + (1f - screenshotRectTransform.anchorMax.x)));
            frameScaleY = 1f / (1f - (screenshotRectTransform.anchorMin.y + (1f - screenshotRectTransform.anchorMax.y)));
        }

        public void SetTexture(Texture2D texture, bool copy = true)
        {
            if (!setup)
                Setup();

            if (textureIsCopy)
                Destroy(screenshotImage.texture);

            if (copy)
            {
                Texture2D textureCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
                Graphics.CopyTexture(texture, textureCopy);
                textureCopy.Apply(false);

                texture = textureCopy;
            }

            Vector2 fullScaleSize = new Vector2(texture.width * frameScaleX, texture.height * frameScaleY);
            ((RectTransform)transform).sizeDelta = fullScaleSize;
            screenshotImage.texture = texture;
            textureIsCopy = copy;
        }

        void OnDestroy()
        {
            if (textureIsCopy)
                Destroy(screenshotImage.texture);
        }
    }
}