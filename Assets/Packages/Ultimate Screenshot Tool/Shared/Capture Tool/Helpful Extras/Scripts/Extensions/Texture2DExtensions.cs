using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class Texture2DExtensions
    {
        public static void CaptureCameraRenderTexture(this Texture2D texture, Camera camera, RenderTexture renderTexture, bool apply = true)
        {
            Rect captureRect = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);
            CaptureCameraRenderTexture(texture, camera, renderTexture, captureRect, apply);
        }

        public static void CaptureCameraRenderTexture(this Texture2D texture, Camera camera, RenderTexture renderTexture, Rect rect, bool apply = true)
        {
            RenderTexture originalRenderTexture = camera.targetTexture;

            camera.targetTexture = renderTexture;
            camera.Render();
            texture.ReadPixels(rect, 0, 0, false);
            if (apply)
                texture.Apply(false);

            camera.targetTexture = originalRenderTexture;
        }

        public static void CaptureRenderTexture(this Texture2D texture, RenderTexture renderTexture, bool apply = true)
        {
            Rect captureRect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            CaptureRenderTexture(texture, renderTexture, captureRect, apply);
        }

        public static void CaptureRenderTexture(this Texture2D texture, RenderTexture renderTexture, Rect rect, bool apply = true)
        {
            RenderTexture originalRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(rect, 0, 0, false);
            if (apply)
                texture.Apply(false);
            RenderTexture.active = originalRenderTexture;
        }

        public static Texture2D Cutout(this Texture2D original, Rect cutoutRect, bool apply = true, bool destroyOriginal = true)
        {
            Color[] pixels = original.GetPixels((int)cutoutRect.x, (int)cutoutRect.y, (int)cutoutRect.width, (int)cutoutRect.height);

            Texture2D finalTexture = new Texture2D((int)cutoutRect.width, (int)cutoutRect.height, original.format, original.mipmapCount > 1);
            finalTexture.SetPixels(pixels);
            if (apply)
                finalTexture.Apply(false);

            if (destroyOriginal)
                MonoBehaviourExtended.FlexibleDestroy(original);
            return finalTexture;
        }

        // Based on : https://answers.unity.com/questions/1008802/merge-multiple-png-images-one-on-top-of-the-other.html
        public static Texture2D AlphaBlend(this Texture2D background, Texture2D foreground, bool apply = true, bool destroyOriginals = true)
        {
            if (background.width != foreground.width || background.height != foreground.height)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
            var backgroundPixels = background.GetPixels();
            var foregroundPixels = foreground.GetPixels();

            int pixelCount = backgroundPixels.Length;
            var resultPixels = new Color[pixelCount];
            for (int i = 0; i < pixelCount; i++)
            {
                Color backgroundColor = backgroundPixels[i];
                Color foregroundColor = foregroundPixels[i];
                resultPixels[i] = backgroundColor.AlphaBlend(foregroundColor);
            }

            Texture2D result = new Texture2D(foreground.width, foreground.height, foreground.format, foreground.mipmapCount > 1);
            result.SetPixels(resultPixels);
            if (apply)
                result.Apply(false);

            if (destroyOriginals)
            {
                MonoBehaviourExtended.FlexibleDestroy(background);
                MonoBehaviourExtended.FlexibleDestroy(foreground);
            }
            return result;
        }

        public static void SetColor(this Texture2D texture, Color color, bool apply = true)
        {
            var resultPixels = texture.GetPixels();
            for (var i = 0; i < resultPixels.Length; ++i)
                resultPixels[i] = color;

            texture.SetPixels(resultPixels);
            if (apply)
                texture.Apply(false);
        }

        public static void Solidify(this Texture2D texture, bool apply = true)
        {
            var resultPixels = texture.GetPixels();
            for (var i = 0; i < resultPixels.Length; ++i)
                resultPixels[i].a = 1f;

            texture.SetPixels(resultPixels);
            if (apply)
                texture.Apply(false);
        }
    }
}