using UnityEngine;
using UnityEngine.UI;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    public class LastTakenScreenshotScript : MonoBehaviour
    {
        RawImage rawImage;
        AspectFitScript aspectFitScript;

        Texture2D defaultTexture;

        public LastTakenScreenshotScript()
        {
            ScreenshotScript.ScreenshotTaken += ScreenshotTaken;
        }

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
            aspectFitScript = GetComponent<AspectFitScript>();
            if (defaultTexture != null)
                UpdateImage(defaultTexture);
        }

        void OnDestroy()
        {
            ScreenshotScript.ScreenshotTaken -= ScreenshotTaken;
        }

        void ScreenshotTaken(Texture2D texture)
        {
            if (rawImage == null)
            {
                defaultTexture = texture;
                return;
            }

            UpdateImage(texture);
        }

        void UpdateImage(Texture2D texture)
        {
            if (aspectFitScript != null)
                aspectFitScript.SetTexture(texture);
            else
            {
                Texture2D textureCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
                Graphics.CopyTexture(texture, textureCopy);
                textureCopy.Apply(false);
                rawImage.texture = textureCopy;
            }
        }
    }
}